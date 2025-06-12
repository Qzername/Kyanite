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
using Google.Apis.Drive.v3.Data;

namespace Whiteboard.Services;

internal class GoogleDriveConnection
{
    DriveService driveService;
    Google.Apis.Drive.v3.Data.File folder;

    public event Action OnInitialized;

    public GoogleDriveConnection()
    {
        _ = Connect();
    }

    async Task Connect()
    {
        UserCredential credential;

        using (var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
        {
            credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.FromStream(stream).Secrets,
                new[] { DriveService.Scope.Drive },
                "user",
                CancellationToken.None,
                new FileDataStore("token.json", true));
        }

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
        try{
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

            if (!Directory.Exists("./Database/"))
                Directory.CreateDirectory("./Database/");

            DownloadFile(driveService, files.First().Id, "./Database/database.db");
        }
        catch(Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
       
    }

    public async Task SaveDatabase()
    {
        System.IO.File.Copy("./Database/database.db", "./Database/database_copy.db");

        await UploadOrUpdateFileAsync(driveService, "./Database/database_copy.db", folder.Id );

        System.IO.File.Delete("./Database/database_copy.db");   
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
        try
        {

            var fileName = Path.GetFileName(filePath);
            var existingFile = GetFileByName(service, fileName, parentFolderId);

            Google.Apis.Drive.v3.Data.File fileMetadata;
            IUploadProgress response;

            if (existingFile != null)
            {
                // Update existing file
                fileMetadata = new Google.Apis.Drive.v3.Data.File() { Name = fileName };

                using (var stream = new FileStream(filePath, FileMode.Open))
                {
                    var updateRequest = service.Files.Update(fileMetadata, existingFile.Id, stream, "application/octet-stream");
                    updateRequest.Fields = "id, name, webViewLink";
                    response = await updateRequest.UploadAsync(); // <-- Changed to UploadAsync()

                    if (response.Status == UploadStatus.Completed)
                    {
                        var updatedFile = updateRequest.ResponseBody;
                        Debug.WriteLine($"Updated file: {updatedFile.Name} (ID: {updatedFile.Id})");
                        return updatedFile.Id;
                    }
                    else
                    {
                        throw new Exception($"Update failed: {response.Exception?.Message}");
                    }
                }
            }
            else
            {
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
        catch(Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }

        return "";

    }
}


