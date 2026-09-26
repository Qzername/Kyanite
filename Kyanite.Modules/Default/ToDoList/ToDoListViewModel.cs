using Avalonia.Threading;
using CommunityToolkit.Mvvm.Input;
using Kyanite.Core.Dialogs;
using Kyanite.Database;
using Kyanite.Modules.Default.ToDoList.Dialogs;
using System.Collections.ObjectModel;

namespace Kyanite.Modules.Default.ToDoList;

internal partial class ToDoListViewModel(ModuleInformation moduleInformation, IDialogService dialogService) : Module(moduleInformation)
{
    [Synchronize] ObservableCollection<ToDoElement> _toDoElements = new();
    public ObservableCollection<ToDoElement> ToDoElements => _toDoElements;

    [RelayCommand]
    void ToggleComplete(ToDoElement element)
    {
        element.IsCompleted = !element.IsCompleted;
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

        dialogService.Show(dialog);
        ApplySorting();
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
            Name = (vm.ToDoElementName.Trim()),
            DueDate = dueDate.Value.Date + dueTime.Value,
            CreatedAt = DateTime.Now

        });
    }

    void ApplySorting()
    {
        var sorted = ToDoElements
            .OrderByDescending(e => e.IsPinned)
            .ThenBy(e => e.IsCompleted)
            .ThenByDescending(e => e.DueDate)
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