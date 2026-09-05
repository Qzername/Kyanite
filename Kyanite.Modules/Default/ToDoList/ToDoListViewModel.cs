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
    void OpenAddNewDialog()
    {
        dialogService.Show(new DialogBuilder()
            .WithTitle("Add new ToDo element")
            .WithSize(1050, 200)
            .WithViewModel(new AddNewToDoElementViewModel())
            .SetOnClose(dialog =>
            {
                var vm = (AddNewToDoElementViewModel)dialog.ViewModel;

                if (vm.ToDoElementName is null)
                    return;

                ToDoElements.Add(new ToDoElement() { 
                    Name = vm.ToDoElementName 
                });
            })
            .Build());
    }

    [RelayCommand]
    void RemoveToDoElement(ToDoElement element)
    {
        ToDoElements.Remove(element);
    }
}