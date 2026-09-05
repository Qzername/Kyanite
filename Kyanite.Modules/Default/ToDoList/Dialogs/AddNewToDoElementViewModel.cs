using CommunityToolkit.Mvvm.ComponentModel;
using Kyanite.ViewModels;

namespace Kyanite.Modules.Default.ToDoList.Dialogs;

internal partial class AddNewToDoElementViewModel : ViewModelBase
{
    [ObservableProperty] string? _toDoElementName;
}
