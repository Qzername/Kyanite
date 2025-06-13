using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.IO;
using System.Threading;
using System;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Google.Apis.Upload;
using Avalonia.Platform;
using Avalonia;

namespace Whiteboard.Services;

internal class GoogleDriveConnection
{
    DriveService driveService;
    Google.Apis.Drive.v3.Data.File folder;

    public event Action OnInitialized;

    readonly string pathDirectory;
    readonly string pathFile;
    readonly string pathFileCopy;

    public GoogleDriveConnection()
    {
        string personalDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

        pathDirectory = personalDirectory + "/Database/";

        pathFile = pathDirectory + "database.db";
        pathFileCopy = pathDirectory + "database_copy.db";

        _ = Connect();
    }

    async Task Connect()
    {
        var uri = new Uri("avares://Whiteboard/Assets/service_account.json");
        using var stream = AssetLoader.Open(uri);

        var credential = GoogleCredential.FromStream(stream)
            .CreateScoped(DriveService.ScopeConstants.Drive);

        driveService = new DriveService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = "Whiteboard",
        });

        await GetDatabase();

        OnInitialized?.Invoke();
    }

    async Task GetDatabase()
{
        var file = await FindFolderByNameAsync(driveService, "WhiteboardDatabase");

        if (file is null)
        {
            file = await CreateFolderAsync(driveService, "WhiteboardDatabase");
            return;
        }
        folder = file;

        var files = GetFilesInFolder(driveService, file.Id);

        if (files.Count == 0)
            return;

        if (!Directory.Exists(pathDirectory))
            Directory.CreateDirectory(pathDirectory);

        if (File.Exists(pathFile))
            File.Delete(pathFile);

        DownloadFile(driveService, files.First().Id, pathFile);
    }

    public async Task SaveDatabase()
    {
        System.IO.File.Copy(pathFile, pathFileCopy);

        await UploadOrUpdateFileAsync(driveService, pathFileCopy, folder.Id );

        System.IO.File.Delete(pathFileCopy);   
    }

    async Task<Google.Apis.Drive.v3.Data.File> FindFolderByNameAsync(DriveService service, string folderName)
    {
        var request = service.Files.List();
        request.Q = $"mimeType='application/vnd.google-apps.folder' and name='{folderName}' and trashed=false";
        request.Fields = "files(id, name)";
        request.Spaces = "drive";

        var result = await request.ExecuteAsync();

        var folder = result.Files.Count > 0 ? result.Files[0] : null;
        return folder;
    }

    async Task<Google.Apis.Drive.v3.Data.File> CreateFolderAsync(DriveService service, string folderName, string? parentId = null)
    {
        var fileMetadata = new Google.Apis.Drive.v3.Data.File
        {
            Name = folderName,
            MimeType = "application/vnd.google-apps.folder",
            Parents = parentId != null ? new[] { parentId } : null
        };

        var request = service.Files.Create(fileMetadata);
        request.Fields = "id, name";

        var folder = await request.ExecuteAsync();
        return folder;
    }

    IList<Google.Apis.Drive.v3.Data.File> GetFilesInFolder(DriveService service, string folderId)
    {
        var request = service.Files.List();
        request.Q = $"'{folderId}' in parents and trashed=false";
        request.Fields = "files(id, name, mimeType)";

        return request.Execute().Files;
    }

    void DownloadFile(DriveService service, string fileId, string downloadPath)
    {
        var request = service.Files.Get(fileId);
        using (var stream = new FileStream(downloadPath, FileMode.Create))
        {
            request.Download(stream);
        }
    }

    Google.Apis.Drive.v3.Data.File GetFileByName(DriveService service, string fileName, string parentFolderId = null)
    {
        var request = service.Files.List();
        request.Q = $"name='{fileName}' and trashed=false";
        if (parentFolderId != null)
        {
            request.Q += $" and '{parentFolderId}' in parents";
        }
        request.Fields = "files(id, name, mimeType)";

        var result = request.Execute();
        return result.Files.FirstOrDefault(); // Returns null if not found
    }

    async Task<string> UploadOrUpdateFileAsync(DriveService service, string filePath, string parentFolderId = null)
{
        var fileName = Path.GetFileName(filePath);
        var existingFile = GetFileByName(service, fileName, parentFolderId);

        Google.Apis.Drive.v3.Data.File fileMetadata;
        IUploadProgress response;

        if (existingFile != null)
        {
            service.Files.Delete(existingFile.Id).Execute();
        }
            // Upload new file
        fileMetadata = new Google.Apis.Drive.v3.Data.File()
        {
            Name = fileName,
            Parents = parentFolderId != null ? new List<string> { parentFolderId } : null
        };

        using (var stream = new FileStream(filePath, FileMode.Open))
        {
            var createRequest = service.Files.Create(fileMetadata, stream, "application/octet-stream");
            createRequest.Fields = "id, name, webViewLink";
            response = await createRequest.UploadAsync(); // <-- Changed to UploadAsync()

            if (response.Status == UploadStatus.Completed)
            {
                var uploadedFile = createRequest.ResponseBody;
                Debug.WriteLine($"Uploaded new file: {uploadedFile.Name} (ID: {uploadedFile.Id})");
                return uploadedFile.Id;
            }
            else
            {
                throw new Exception($"Upload failed: {response.Exception?.Message}");
            }
        }
    }
}


