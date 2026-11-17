using Kyanite.Core.Models;
using Kyanite.Core.Services;
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

        CurrentAppSettings.KeepRunningInBackground = value;
        
        Save(CurrentAppSettings);
    }

    public bool EnableNotifications => CurrentAppSettings?.EnableNotifications ?? true;
    public bool NotifyOnOverdue => CurrentAppSettings?.NotifyOnOverdue ?? true;
    public int DueSoonThresholdMinutes => CurrentAppSettings?.DueSoonThresholdMinutes ?? 30;

    public void SetNotificationDefaults(int dueSoonThresholdMinutes, bool enableNotifications, bool notifyOnOverdue)
    {
        if (CurrentAppSettings is null)
            return;
        
        CurrentAppSettings.DueSoonThresholdMinutes = dueSoonThresholdMinutes;
        CurrentAppSettings.EnableNotifications = enableNotifications;
        CurrentAppSettings.NotifyOnOverdue = notifyOnOverdue;
        
        Save(CurrentAppSettings);
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
