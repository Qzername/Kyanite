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

        string json = File.ReadAllText(FullPathToSetingsFile);
        CurrentAppSettings = JsonSerializer.Deserialize<AppSettings>(json);
    }

    public void Save(AppSettings appSettings)
    {
        if (!Directory.Exists(AppDataDirectory))
            Directory.CreateDirectory(AppDataDirectory);

        string json = JsonSerializer.Serialize(appSettings);
        File.WriteAllText(FullPathToSetingsFile, json);

        CurrentAppSettings = appSettings;
    }
}
