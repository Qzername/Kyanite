using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using Kyanite.Services;
using Kyanite.ViewModels;

namespace Kyanite.Dialogs.AppSettings;

internal partial class AppSettingsViewModel : ViewModelBase
{
    [ObservableProperty] int _dueSoonThresholdMinutes;
    [ObservableProperty] bool _enableNotifications;
    [ObservableProperty] bool _notifyOnOverdue;
    [ObservableProperty] bool _keepRunningInBackground;

    public bool IsDesktop => Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime;

    public AppSettingsViewModel(AppSettingsService settings)
    {
        DueSoonThresholdMinutes = settings.DueSoonThresholdMinutes;
        EnableNotifications = settings.EnableNotifications;
        NotifyOnOverdue = settings.NotifyOnOverdue;
        KeepRunningInBackground = settings.KeepRunningInBackground;
    }
}
