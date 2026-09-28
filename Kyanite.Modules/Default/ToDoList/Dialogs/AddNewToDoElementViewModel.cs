using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kyanite.Core.Dialogs;
using Kyanite.ViewModels;

namespace Kyanite.Modules.Default.ToDoList.Dialogs;

internal partial class AddNewToDoElementViewModel(IDialogService dialogService, Action<ToDoElement> onAdd) : ViewModelBase
{
    [ObservableProperty] string? _toDoElementName;
    [ObservableProperty] DateTimeOffset? _dueDate;
    [ObservableProperty] TimeSpan? _dueTime;

    public ToDoElement ToToDoElement()
    {
        DateTime? finalDueDate = null;
        if (DueDate.HasValue)
        {
            finalDueDate = DueDate.Value.DateTime.Date + (DueTime ?? TimeSpan.Zero);
        }

        return new()
        {
            Name = ToDoElementName?.Trim() ?? string.Empty,
            DueDate = finalDueDate,
            CreatedAt = DateTime.Now,
        };
    }

    [RelayCommand]
    private void Add()
    {
        if (string.IsNullOrWhiteSpace(ToDoElementName)) return;
        onAdd?.Invoke(ToToDoElement());
        dialogService.Close();
    }

    [RelayCommand]
    private void Cancel()
    {
        dialogService.Close();
    }
}