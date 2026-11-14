using Avalonia.Threading;
using CommunityToolkit.Mvvm.Input;
using Kyanite.Core.Dialogs;
using Kyanite.Database;
using Kyanite.Modules.Default.ToDoList.Dialogs;
using Kyanite.Services;
using System.Collections.ObjectModel;

namespace Kyanite.Modules.Default.ToDoList;

internal partial class ToDoListViewModel : Module
{
    readonly IDialogService _dialogService;
    readonly NotificationServiceProvider _notificationServiceProvider;
    readonly DispatcherTimer _overdueCheckTimer;

    [Synchronize] ToDoSettings _settings = new();
    public ToDoSettings Settings => _settings;

    [Synchronize] ObservableCollection<ToDoElement> _toDoElements = new();
    public ObservableCollection<ToDoElement> ToDoElements => _toDoElements;

    public ToDoListViewModel(
        ModuleInformation moduleInformation,
        IDialogService dialogService,
        NotificationServiceProvider notificationServiceProvider)
        : base(moduleInformation)
    {
        _dialogService = dialogService;
        _notificationServiceProvider = notificationServiceProvider;
        _overdueCheckTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMinutes(1),
            IsEnabled = true
        };

        _overdueCheckTimer.Tick += (_, _) =>
        {
            foreach (var element in _toDoElements)
            {
                element.RefreshOverdueStatus();

                if (!Settings.EnableNotifications || element.IsCompleted || element.DueDate is null)
                    continue;

                if (Settings.NotifyOnOverdue && !element.OverdueNotified && element.IsOverdue)
                {
                    ShowNotification("Overdue", $"\"{element.Name}\" is overdue.");
                    element.OverdueNotified = true;
                    element.DueSoonNotified = true; // Prevent due soon notification after overdue notification
                }

                if (!element.DueSoonNotified && element.IsDueSoon(Settings.DueSoonThresholdMinutes))
                {
                    ShowNotification("Reminder", $"\"{element.Name}\" is due in {Settings.DueSoonThresholdMinutes} minutes.");
                    element.DueSoonNotified = true;
                }
            }
        };
    }


    [RelayCommand]
    void OpenSettingsDialog()
    {
        _dialogService.CreateBuilder()
            .WithTitle("To-Do Settings")
            .WithSize(420, 260)
            .WithViewModel(new ToDoSettingsViewModel(Settings))
            .AddButton("Add", OnSettingsDialogClosed)
            .BuildAndShow();
    }

    [RelayCommand]
    void ToggleComplete(ToDoElement element)
    {
        element.CompletedAt = element.IsCompleted ? DateTime.Now : null;
        ApplySorting();
    }

    [RelayCommand]
    void OpenAddNewDialog()
    {
        _dialogService.CreateBuilder()
            .WithTitle("Add new ToDo element")
            .WithSize(1067, 200)
            .WithViewModel(new AddNewToDoElementViewModel())
            .AddButton("Close", OnAddDialogClosed)
            .BuildAndShow();
    }

    [RelayCommand]
    void TogglePin(ToDoElement element)
    {
        element.IsPinned = !element.IsPinned;
        ApplySorting();
    }

    [RelayCommand]
    void RemoveToDoElement(ToDoElement element)
    {
        ToDoElements.Remove(element);
    }

    /* This should work when you change .net version to proper one */
    void ShowNotification(string title, string message)
    {
        _notificationServiceProvider.ActiveService?.Show(title, message);
    }

    void OnSettingsDialogClosed(Dialog dialog)
    {
        if (dialog.ViewModel is not ToDoSettingsViewModel vm)
            return;
        Settings.EnableNotifications = vm.EnableNotifications;
        Settings.NotifyOnOverdue = vm.NotifyOnOverdue;
        Settings.DueSoonThresholdMinutes = vm.DueSoonThresholdMinutes;
    }

    void OnAddDialogClosed(Dialog dialog)
    {
        if (dialog.ViewModel is not AddNewToDoElementViewModel vm ||
            string.IsNullOrWhiteSpace(vm.ToDoElementName))
            return;

        var dueDate = vm.DueDate ?? DateTime.Today + TimeSpan.FromHours(24);
        var dueTime = vm.DueTime ?? TimeSpan.Zero;

        ToDoElements.Insert(0, new ToDoElement
        {
            Name = vm.ToDoElementName.Trim(),
            DueDate = dueDate.Date + dueTime,
            CreatedAt = DateTime.Now
        });

        ApplySorting();
    }

    void ApplySorting()
    {
    	var sorted = ToDoElements.OrderByDescending(e => e.IsPinned)
     		.ThenBy(e => e.IsCompleted)
       		.ThenByDescending(e => e.DisplayDate)
         	.ToList();

        ToDoElements.Clear();
        foreach (var item in sorted)
            ToDoElements.Add(item);
    }
}
