using Google.Apis.Auth.OAuth2;
using Data = Google.Apis.Drive.v3.Data;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using System.IO;
using System;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using Google.Apis.Upload;
using Avalonia.Platform;

namespace Whiteboard.Services;

internal class GoogleDriveConnection
{
    DriveService driveService;
    Data.File folder;

    public event Action OnInitialized;
    bool _isInitialized = false;    
    public bool IsInitialized => _isInitialized;

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

        _isInitialized = true;
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

        var files = await GetFilesInFolder(driveService, file.Id);

        if (files.Length == 0)
            return;

        if (!Directory.Exists(pathDirectory))
            Directory.CreateDirectory(pathDirectory);

        if (File.Exists(pathFile))
            File.Delete(pathFile);

        await DownloadFile(driveService, files[0].Id, pathFile);
    }

    public async Task SaveDatabase()
    {
        if (!File.Exists(pathFile))
            throw new Exception("For some reason the database file does not exist.");

        if(File.Exists(pathFileCopy))
            File.Delete(pathFileCopy);

        File.Copy(pathFile, pathFileCopy);
        await UploadFileAsync(driveService, pathFileCopy, folder.Id);
        File.Delete(pathFileCopy);
    }

    async Task<Data.File> FindFolderByNameAsync(DriveService service, string folderName)
    {
        var request = service.Files.List();
        request.Q = $"mimeType='application/vnd.google-apps.folder' and name='{folderName}' and trashed=false";
        request.Fields = "files(id, name)";
        request.Spaces = "drive";

        var result = await request.ExecuteAsync();

        return result.Files.Count > 0 ? result.Files[0] : null!;
    }

    async Task<Data.File> CreateFolderAsync(DriveService service, string folderName, string? parentId = null)
    {
        var fileMetadata = new Data.File
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

    async Task<Data.File[]> GetFilesInFolder(DriveService service, string folderId)
    {
        var request = service.Files.List();
        request.Q = $"'{folderId}' in parents and trashed=false";
        request.Fields = "files(id, name, mimeType)";

        var result = await request.ExecuteAsync();
        
        return result.Files.ToArray();
    }

    async Task DownloadFile(DriveService service, string fileId, string downloadPath)
    {
        var request = service.Files.Get(fileId);
        using (var stream = new FileStream(downloadPath, FileMode.Create))
        {
            await request.DownloadAsync(stream);
        }
    }

    async Task<Data.File> GetFileByName(DriveService service, string fileName, string parentFolderId = null)
    {
        var request = service.Files.List();
        request.Q = $"name='{fileName}' and trashed=false";

        if (parentFolderId != null)
            request.Q += $" and '{parentFolderId}' in parents";

        request.Fields = "files(id, name, mimeType)";

        var result = await request.ExecuteAsync();
        return result.Files.FirstOrDefault()!;
    }

    async Task<string> UploadFileAsync(DriveService service, string filePath, string parentFolderId = null)
    {
        var fileName = Path.GetFileName(filePath);
        var existingFile = await GetFileByName(service, fileName, parentFolderId);

        Data.File fileMetadata;
        IUploadProgress response;

        if (existingFile != null)
            await service.Files.Delete(existingFile.Id).ExecuteAsync();

        fileMetadata = new Data.File()
        {
            Name = fileName,
            Parents = parentFolderId != null ? new List<string> { parentFolderId } : null
        };

        using (var stream = new FileStream(filePath, FileMode.Open))
        {
            var createRequest = service.Files.Create(fileMetadata, stream, "application/octet-stream");
            createRequest.Fields = "id, name, webViewLink";
            response = await createRequest.UploadAsync();

            if (response.Status == UploadStatus.Completed)
            {
                var uploadedFile = createRequest.ResponseBody;
                Debug.WriteLine($"Uploaded new file: {uploadedFile.Name} (ID: {uploadedFile.Id})");
                return uploadedFile.Id;
            }
            else
                throw new Exception($"Upload failed: {response.Exception?.Message}");
        }
    }

}