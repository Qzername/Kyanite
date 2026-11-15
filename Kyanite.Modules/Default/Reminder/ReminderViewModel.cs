using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kyanite.Database;
using Kyanite.Services;

namespace Kyanite.Modules.Default.Reminder;

public partial class ReminderViewModel(ModuleInformation moduleInformation, NotificationServiceProvider notificationServiceProvider) : Module(moduleInformation)
{
    [ObservableProperty] string _notificationTitle = string.Empty;
    [ObservableProperty] string _notificationDescription = string.Empty;

    [RelayCommand]
    void SendNotification()
    {
        var notificationService = notificationServiceProvider.ActiveService;
        notificationService.Show(NotificationTitle, NotificationDescription);
    }
}