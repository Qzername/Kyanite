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
        element.CompletedAt = element.IsCompleted ? DateTime.Now : null;
        ApplySorting();
    }

    [RelayCommand]
    void OpenAddNewDialog()
    {
        dialogService.CreateBuilder()
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

    void OnAddDialogClosed(Dialog dialog)
    {
        if (dialog.ViewModel is not AddNewToDoElementViewModel vm ||
            string.IsNullOrEmpty(vm.ToDoElementName))
            return;

        if (string.IsNullOrWhiteSpace(vm.ToDoElementName))
            return;

        ToDoElements.Insert(0, new ToDoElement
        {
            Name = vm.ToDoElementName.Trim()
        });
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