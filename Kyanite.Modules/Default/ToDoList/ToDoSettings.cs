using CommunityToolkit.Mvvm.ComponentModel;

namespace Kyanite.Modules.Default.ToDoList;

public partial class ToDoSettings : ObservableObject
{
    [ObservableProperty] int _dueSoonThresholdMinutes = 30;
    [ObservableProperty] bool _enableNotifications = true;
    [ObservableProperty] bool _notifyOnOverdue = true;
}
