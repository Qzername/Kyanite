using Kyanite.Database.Default.Local;
using Kyanite.Services;
using System.Text;
using System.Text.Json;

namespace Kyanite.Database.Default.GoogleDrive;

public class GoogleDriveDatabaseStack(NotificationServiceProvider notificationServiceProvider) : LocalDatabaseStack()
{
    public override string FriendlyName => "Google Drive";

    string _apiLink = string.Empty;

    public override async Task<bool> Prepare(Dictionary<string, string> data)
    {
        if (!data.TryGetValue("Link", out string? value) || string.IsNullOrWhiteSpace(value))
            throw new InvalidOperationException("Google Drive stack requires a valid Web API link.");

        _apiLink = value.Trim();

        await DownloadDatabaseFile(DatabaseFilename, GetFilePath());
        return await base.Prepare(data);
    }

    async Task<bool> DownloadDatabaseFile(string fileName, string savePath)
    {
        using var client = CreateClient();

        string requestUrl = $"{_apiLink}?filename={Uri.EscapeDataString(fileName)}";
        using var response = await client.GetAsync(requestUrl);

        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException($"Could not connect to Google Drive (HTTP {(int)response.StatusCode}).");

        string jsonResponse = await response.Content.ReadAsStringAsync();

        using var document = JsonDocument.Parse(jsonResponse);
        var root = document.RootElement;

        string? status = root.TryGetProperty("status", out var statusElement) ? statusElement.GetString() : null;
        string? message = root.TryGetProperty("message", out var messageElement) ? messageElement.GetString() : null;

        if (status == "success" &&
            root.TryGetProperty("content", out var contentElement) &&
            contentElement.GetString() is { Length: > 0 } base64Content)
        {
            byte[] fileBytes = Convert.FromBase64String(base64Content);

            var directory = Path.GetDirectoryName(savePath);
            if (!string.IsNullOrEmpty(directory))
                Directory.CreateDirectory(directory);

            await File.WriteAllBytesAsync(savePath, fileBytes);
            return true;
        }

        // The first synchronization starts with an empty folder on Google Drive.
        if (status == "error" && message?.Contains("not found", StringComparison.OrdinalIgnoreCase) == true)
            return false;

        throw new InvalidOperationException(message ?? "Could not download the database from Google Drive.");
    }

    public override async Task OnWindowClosing()
    {
        string path = GetFilePath();

        if (!File.Exists(path))
            return;

        // Release the SQLite lock before reading the file.
        await CloseConnectionAsync();
        IsPrepared = false;

        string tempFilePath = Path.Combine(Path.GetTempPath(), $"kyanite_database_{Guid.NewGuid():N}.db");

        try
        {
            File.Copy(path, tempFilePath, overwrite: true);
            byte[] fileBytes = await File.ReadAllBytesAsync(tempFilePath);
            string base64Content = Convert.ToBase64String(fileBytes);

            var payload = new
            {
                filename = Path.GetFileName(path),
                content = base64Content
            };

            string jsonPayload = JsonSerializer.Serialize(payload);

            using var client = CreateClient();
            using var response = await client.PostAsync(
                _apiLink,
                new StringContent(jsonPayload, Encoding.UTF8, "application/json"));

            string jsonResponse = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException($"Could not upload the database (HTTP {(int)response.StatusCode}).");

            using var document = JsonDocument.Parse(jsonResponse);
            var root = document.RootElement;
            string? status = root.TryGetProperty("status", out var statusElement) ? statusElement.GetString() : null;

            if (status != "success")
            {
                string? message = root.TryGetProperty("message", out var messageElement) ? messageElement.GetString() : null;
                throw new InvalidOperationException(message ?? "Google Drive rejected the upload.");
            }
        }
        finally
        {
            if (File.Exists(tempFilePath))
                File.Delete(tempFilePath);
        }

        // The local copy is only removed after a successful upload.
        await base.OnRemoved();

        notificationServiceProvider.Show(
            "Kyanite - Google Drive",
            "Database has been saved safely on Google Drive.");
    }

    static HttpClient CreateClient()
        => new() { Timeout = TimeSpan.FromSeconds(30) };

    public override DatabaseInitializationViewModelBase CreateInitializationViewModel()
        => new GoogleDriveInitializationViewModel();
}
