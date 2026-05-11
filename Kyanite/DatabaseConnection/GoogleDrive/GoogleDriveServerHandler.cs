using Kyanite.DatabaseConnection.Local;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Kyanite.DatabaseConnection.GoogleDrive;

internal class GoogleDriveServerHandler : LocalServerHandler
{
    const string DatabaseLocalization = "./Kyanite/Database.db";
    const string scriptUrl = "[INSERT HERE SCRIPT URL]";

    public override async void OnApplicationOpen()
    {
        await DownloadDatabaseFile("Database.db", DatabaseLocalization);
        base.OnApplicationOpen();
    }

    public override async void OnApplicationClose()
    {
        await UploadDatabaseFile(DatabaseLocalization);
    }

    async Task DownloadDatabaseFile(string fileName, string savePath)
    {
        using var client = new HttpClient();

        string requestUrl = $"{scriptUrl}?filename={fileName}";
        var response = await client.GetAsync(requestUrl);

        if (!response.IsSuccessStatusCode)
        {
            Debug.WriteLine($"[{nameof(GoogleDriveServerHandler)}] Connection error.");
            return;
        }

        string jsonResponse = await response.Content.ReadAsStringAsync();
        var result = JsonDocument.Parse(jsonResponse);
        var root = result.RootElement;

        if (root.GetProperty("status").GetString() == "success")
        {
            CloseDatabase();
            string base64Content = root.GetProperty("content").GetString();
            byte[] fileBytes = Convert.FromBase64String(base64Content);

            File.WriteAllBytes(savePath, fileBytes);
            Debug.WriteLine($"[{nameof(GoogleDriveServerHandler)}] File {fileName} was saved in {savePath}");
            OpenDatabase();
        }
        else
        {
            string message = root.GetProperty("message").GetString();

            if (message == "File not found")
                await UploadDatabaseFile(DatabaseLocalization);
            else
                Debug.WriteLine($"[{nameof(GoogleDriveServerHandler)}] Script error: {message}");
        }
    }

    async Task UploadDatabaseFile(string filePath)
    {
        try
        {
            Debug.WriteLine($"[{nameof(GoogleDriveServerHandler)}] Uploading database...");

            CloseDatabase();

            using var client = new HttpClient();

            byte[] fileBytes = File.ReadAllBytes(filePath);
            string base64Content = Convert.ToBase64String(fileBytes);

            OpenDatabase();

            var payload = new
            {
                filename = Path.GetFileName(filePath),
                content = base64Content
            };

            string jsonPayload = JsonSerializer.Serialize(payload);
            var response = await client.PostAsync(scriptUrl, new StringContent(jsonPayload, System.Text.Encoding.UTF8, "application/json"));

            string result = await response.Content.ReadAsStringAsync();

            Debug.WriteLine($"[{nameof(GoogleDriveServerHandler)}] Databased is uploaded.");
        }
        catch(Exception ex)
        {
            Debug.WriteLine($"[{nameof(GoogleDriveServerHandler)}] " + ex.Message);
        }
    }
}
