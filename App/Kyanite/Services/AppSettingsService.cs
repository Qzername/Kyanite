using Kyanite.Models;
using System;
using System.IO;
using System.Text.Json;

namespace Kyanite.Services;

internal class AppSettingsService : IAppSettingsService
{
    readonly string AppDataDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Kyanite"
        );

    readonly string FullPathToSetingsFile = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Kyanite",
            "settings.json"
        );

    public AppSettings? CurrentAppSettings { get; private set; }

    public bool KeepRunningInBackground => CurrentAppSettings?.KeepRunningInBackground ?? true;

    public void SetKeepRunningInBackground(bool value)
    {
        if (CurrentAppSettings is null)
            return;

        Save(CurrentAppSettings with { KeepRunningInBackground = value });
    }

    public AppSettingsService()
    {
        Load();
    }

    void Load()
    {
        if (!Directory.Exists(AppDataDirectory) || !File.Exists(FullPathToSetingsFile))
        {
            CurrentAppSettings = new AppSettings();
            return;
        }

        try
        {
            string json = File.ReadAllText(FullPathToSetingsFile);
            CurrentAppSettings = JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
            CurrentAppSettings.DatabaseInformation ??= [];
        }
        catch (Exception e) when (e is JsonException or IOException or UnauthorizedAccessException)
        {
            try { File.Move(FullPathToSetingsFile, FullPathToSetingsFile + ".invalid", overwrite: true); } catch { }
            CurrentAppSettings = new AppSettings();
        }
    }

    public void Save(AppSettings appSettings)
    {
        if (!Directory.Exists(AppDataDirectory))
            Directory.CreateDirectory(AppDataDirectory);

        string tempPath = FullPathToSetingsFile + ".tmp";
        File.WriteAllText(tempPath, JsonSerializer.Serialize(appSettings));
        File.Move(tempPath, FullPathToSetingsFile, overwrite: true);

        CurrentAppSettings = appSettings;
    }
}
