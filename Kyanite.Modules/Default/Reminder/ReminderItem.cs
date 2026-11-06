using CommunityToolkit.Mvvm.ComponentModel;
using System.Text.Json.Serialization;

namespace Kyanite.Modules.Default.Reminder;

internal partial class ReminderItem : ObservableObject
{
    [ObservableProperty] string _title = string.Empty;
    [ObservableProperty] string _message = string.Empty;
    [ObservableProperty] DateTime _remindAt = DateTime.Now.AddMinutes(5);
    [ObservableProperty] bool _isCompleted;

    [JsonIgnore]
    public bool Notified { get; set; }

    [JsonIgnore]
    public bool IsPastDue => !IsCompleted && RemindAt <= DateTime.Now;

    public void RefreshStatus() => OnPropertyChanged(nameof(IsPastDue));
}
