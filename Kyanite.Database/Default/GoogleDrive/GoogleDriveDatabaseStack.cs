using Kyanite.Database.Default.Local;
using System.Text.Json;

namespace Kyanite.Database.Default.GoogleDrive;

public class GoogleDriveDatabaseStack() : LocalDatabaseStack()
{
    const string scriptUrl = "[URL TO SCRIPT.JS]";

    public override async Task<bool> Prepare()
    {
        await DownloadDatabaseFile("database.db", currentDbFilename);
        return await base.Prepare();
    }

    static async Task DownloadDatabaseFile(string fileName, string savePath)
    {
        using var client = new HttpClient();

        string requestUrl = $"{scriptUrl}?filename={fileName}";
        var response = await client.GetAsync(requestUrl);

        if (!response.IsSuccessStatusCode)
            throw new Exception("Could not connect to google drive");

        string jsonResponse = await response.Content.ReadAsStringAsync();
        var result = JsonDocument.Parse(jsonResponse);
        var root = result.RootElement;

        if (root.GetProperty("status").GetString() != "success")
            return;

        string base64Content = root.GetProperty("content").GetString();
        byte[] fileBytes = Convert.FromBase64String(base64Content);

        File.WriteAllBytes(savePath, fileBytes);
    }

    public override async Task OnWindowClosing()
    {
        using var client = new HttpClient();

        string tempFilePath = Path.GetTempPath() + "kyanite_database.db";

        File.Copy(currentDbFilename, tempFilePath);
        byte[] fileBytes = File.ReadAllBytes(tempFilePath);
        string base64Content = Convert.ToBase64String(fileBytes);
        File.Delete(tempFilePath);

        var payload = new
        {
            filename = Path.GetFileName(currentDbFilename),
            content = base64Content
        };

        string jsonPayload = JsonSerializer.Serialize(payload);
        await client.PostAsync(scriptUrl, new StringContent(jsonPayload, System.Text.Encoding.UTF8, "application/json"));
    }
}