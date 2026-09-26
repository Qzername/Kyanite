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

                if (element.IsCompleted || element.DueDate is null)
                    continue;

                var remaining = element.DueDate.Value - DateTime.Now;

                /*if (!element.DueSoonNotified && remaining > TimeSpan.Zero && remaining <= TimeSpan.FromMinutes(15))
                {
                    ShowNotification("Reminder", $"\"{element.Name}\" is due in 15 minutes.");
                    element.DueSoonNotified = true;
                }

                if (!element.OverdueNotified && remaining <= TimeSpan.Zero)
                {
                    ShowNotification("Overdue", $"\"{element.Name}\" is overdue.");
                    element.OverdueNotified = true;
                }*/
            }
        };
    }

    /* This should work but idk why it doesnt show popup maybe some api stuff that i preffer not get into right now you can check it 
    void ShowNotification(string title, string message)
    {
        _notificationServiceProvider.ActiveService.Show(title, message);
    }*/

    [RelayCommand]
    void ToggleComplete(ToDoElement element)
    {
        element.CompletedAt = element.IsCompleted ? DateTime.Now : null;
        ApplySorting();
    }

    [RelayCommand]
    void OpenAddNewDialog()
    {
        var dialog = new DialogBuilder()
            .WithTitle("Add new Taks todo")
            .WithSize(1050, 200)
            .WithViewModel(new AddNewToDoElementViewModel())
            .SetOnClose(OnAddDialogClosed)
            .Build();

        _dialogService.Show(dialog);
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

    void OnAddDialogClosed(Dialog dialog)
    {
        if (dialog.ViewModel is not AddNewToDoElementViewModel vm ||
            string.IsNullOrWhiteSpace(vm.ToDoElementName))
            return;

        var dueDate = vm.DueDate;
        var dueTime = vm.DueTime;

        if (dueDate == null) dueDate = DateTime.Today + TimeSpan.FromHours(24);
        if (dueTime == null) dueTime = TimeSpan.Zero;

        ToDoElements.Insert(0, new ToDoElement
        {
            Name = vm.ToDoElementName.Trim(),
            DueDate = dueDate.Value.Date + dueTime.Value,
            CreatedAt = DateTime.Now
        });

        ApplySorting();
    }

    void ApplySorting()
    {
        var sorted = ToDoElements
            .OrderByDescending(e => e.IsPinned)
            .ThenBy(e => e.IsCompleted)
            .ThenByDescending(e => e.DisplayDate)
            .ToList();

        for (int targetIndex = 0; targetIndex < sorted.Count; targetIndex++)
        {
            var item = sorted[targetIndex];
            int currentIndex = ToDoElements.IndexOf(item);

            if (currentIndex != targetIndex)
                ToDoElements.Move(currentIndex, targetIndex);
        }
    }
}