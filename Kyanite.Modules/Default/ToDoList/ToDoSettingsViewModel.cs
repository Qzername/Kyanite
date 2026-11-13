using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using Kyanite.ViewModels;

namespace Kyanite.Modules.Default.ToDoList.Dialogs;


public partial class ToDoSettingsViewModel : ViewModelBase
{
    [ObservableProperty] int _dueSoonThresholdMinutes;
    [ObservableProperty] bool _enableNotifications;
    [ObservableProperty] bool _notifyOnOverdue;
    [ObservableProperty] bool _keepRunningInBackground;

    public bool IsDesktop => Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime;

    public ToDoSettingsViewModel(ToDoSettings currentSettings, bool keepRunningInBackground)
    {
        DueSoonThresholdMinutes = currentSettings.DueSoonThresholdMinutes;
        EnableNotifications = currentSettings.EnableNotifications;
        NotifyOnOverdue = currentSettings.NotifyOnOverdue;
        KeepRunningInBackground = keepRunningInBackground;
    }
}