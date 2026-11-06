using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kyanite.Database;
using Kyanite.Services;
using System.Collections.ObjectModel;

namespace Kyanite.Modules.Default.Reminder;

internal partial class ReminderViewModel : Module
{
    readonly NotificationServiceProvider _notificationServiceProvider;
    readonly DispatcherTimer _timer;

    [Synchronize] ObservableCollection<ReminderItem> _reminders = [];
    public ObservableCollection<ReminderItem> Reminders => _reminders;

    [ObservableProperty] string _newTitle = string.Empty;
    [ObservableProperty] string _newMessage = string.Empty;
    [ObservableProperty] DateTime? _newDate;
    [ObservableProperty] TimeSpan? _newTime;

    public ReminderViewModel(
        ModuleInformation moduleInformation,
        NotificationServiceProvider notificationServiceProvider)
        : base(moduleInformation)
    {
        _notificationServiceProvider = notificationServiceProvider;
        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(30)
        };
        _timer.Tick += (_, _) => CheckReminders();
        _timer.Start();

        // Do not replay reminders that expired while the app was closed.
        foreach (var reminder in _reminders)
        {
            reminder.RefreshStatus();

            if (reminder.RemindAt <= DateTime.Now)
                reminder.Notified = true;
        }
    }

    public override void Refresh()
    {
        base.Refresh();

        foreach (var reminder in _reminders)
        {
            reminder.RefreshStatus();

            if (reminder.RemindAt <= DateTime.Now)
                reminder.Notified = true;
        }
    }

    public override void Dispose()
    {
        _timer.Stop();
        base.Dispose();
    }

    [RelayCommand]
    void AddReminder()
    {
        if (string.IsNullOrWhiteSpace(NewTitle))
            return;

        DateTime remindAt = NewDate.HasValue
            ? NewDate.Value.Date + (NewTime ?? TimeSpan.Zero)
            : DateTime.Now.AddMinutes(5);

        var reminder = new ReminderItem
        {
            Title = NewTitle.Trim(),
            Message = NewMessage.Trim(),
            RemindAt = remindAt
        };

        int index = 0;
        while (index < _reminders.Count && _reminders[index].RemindAt <= remindAt)
            index++;

        _reminders.Insert(index, reminder);

        NewTitle = string.Empty;
        NewMessage = string.Empty;
        NewDate = null;
        NewTime = null;
    }

    [RelayCommand]
    void RemoveReminder(ReminderItem reminder) => _reminders.Remove(reminder);

    [RelayCommand]
    void MarkCompleted(ReminderItem reminder)
    {
        reminder.IsCompleted = !reminder.IsCompleted;
        reminder.RefreshStatus();
    }

    void CheckReminders()
    {
        foreach (var reminder in _reminders)
        {
            reminder.RefreshStatus();

            if (reminder.IsCompleted || reminder.Notified || reminder.RemindAt > DateTime.Now)
                continue;

            _notificationServiceProvider.Show(reminder.Title, reminder.Message);
            reminder.Notified = true;
        }
    }
}
