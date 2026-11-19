using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using Kyanite.Core.Services;
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

	public AppSettingsViewModel(IAppSettingsService settings)
	{
		DueSoonThresholdMinutes = settings.CurrentAppSettings.DueSoonThresholdMinutes;
		EnableNotifications = settings.CurrentAppSettings.EnableNotifications;
		NotifyOnOverdue = settings.CurrentAppSettings.NotifyOnOverdue;
		KeepRunningInBackground = settings.CurrentAppSettings.KeepRunningInBackground;
	}
}
