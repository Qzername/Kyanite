using Avalonia.Threading;
using Kyanite.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Kyanite.Modules.Default.ToDoList;

internal class ToDoReminderService : IDisposable
{
    readonly NotificationServiceProvider _notificationServiceProvider;
    readonly DispatcherTimer _timer;
    ObservableCollection<ToDoElement>? _elements;
    ToDoSettings? _settings;

    public ToDoReminderService(NotificationServiceProvider notificationServiceProvider)
    {
        _notificationServiceProvider = notificationServiceProvider;

        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMinutes(1)
        };
        _timer.Tick += OnTimerTick;
    }

    public void Start(ObservableCollection<ToDoElement> elements, ToDoSettings settings)
    {
        _elements = elements;
        _settings = settings;

        foreach (var element in _elements)
        {
            element.RefreshOverdueStatus();
            if (element.IsOverdue)
            {
                element.OverdueNotified = true;
                element.DueSoonNotified = true;
            }
        }

        CheckReminders();

        _timer.Start();
    }

    public void Stop() => _timer.Stop();

    public void Dispose()
    {
        _timer.Stop();
        _timer.Tick -= OnTimerTick;
    }

    public void CheckReminders()
    {
        if (_elements is null || _settings is null)
            return;

        foreach (var element in _elements.ToList())
        {
            element.RefreshOverdueStatus();

            if (!_settings.EnableNotifications || element.IsCompleted || element.DueDate is null)
                continue;

            if (_settings.NotifyOnOverdue && !element.OverdueNotified && element.IsOverdue)
            {
                ShowNotification("Overdue", $"\"{element.Name}\" is overdue.");
                element.OverdueNotified = true;
                element.DueSoonNotified = true;
            }

            if (!element.DueSoonNotified && element.IsDueSoon(_settings.DueSoonThresholdMinutes))
            {
                ShowNotification("Reminder", $"\"{element.Name}\" is due in {_settings.DueSoonThresholdMinutes} minutes.");
                element.DueSoonNotified = true;
            }
        }
    }

    void OnTimerTick(object? sender, EventArgs e) => CheckReminders();

    void ShowNotification(string title, string message)
    {
        Debug.Print($"{message} - {title}");
        _notificationServiceProvider.ActiveService?.Show(title, message);
    }
}