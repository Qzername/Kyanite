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

        int currentIndex = ToDoElements.IndexOf(element);
        if (currentIndex < 0) return;

        if (element.IsCompleted)
        {
            int lastIndex = ToDoElements.Count - 1;
            if (currentIndex != lastIndex)
            {
                ToDoElements.Move(currentIndex, lastIndex);
            }
        }
        else
        {
            if (currentIndex != 0)
            {
                ToDoElements.Move(currentIndex, 0);
            }
        }
    }

    [RelayCommand]
    void OpenAddNewDialog()
    {
        dialogService.Show(new DialogBuilder()
            .WithTitle("Add new ToDo element")
            .WithSize(1067, 200)
            .WithViewModel(new AddNewToDoElementViewModel())
            .SetOnClose(dialog =>
            {
                var vm = (AddNewToDoElementViewModel)dialog.ViewModel;

                if (string.IsNullOrWhiteSpace(vm.ToDoElementName))
                    return;

                var newElement = new ToDoElement()
                {
                    Name = vm.ToDoElementName
                };

                ToDoElements.Insert(0, newElement);
            })
            .Build());
    }

    [RelayCommand]
    void RemoveToDoElement(ToDoElement element)
    {
        ToDoElements.Remove(element);
    }
}