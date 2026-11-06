using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kyanite.Core.Dialogs;
using Kyanite.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kyanite.Modules.Default.ToDoList.Dialogs;

internal partial class AddNewToDoElementViewModel : ViewModelBase
{
    readonly IDialogService _dialogService;
    readonly Action<ToDoElement> _onAdd;

    [ObservableProperty] string? _toDoElementName;
    [ObservableProperty] DateTime? _dueDate;
    [ObservableProperty] TimeSpan? _dueTime;
    [ObservableProperty] ToDoPriority _priority = ToDoPriority.Normal;
    [ObservableProperty] string? _tagsInput;

    public IReadOnlyList<ToDoPriority> AvailablePriorities { get; } = Enum.GetValues<ToDoPriority>();

    public AddNewToDoElementViewModel(IDialogService dialogService, Action<ToDoElement> onAdd)
    {
        _dialogService = dialogService;
        _onAdd = onAdd;
    }

    [RelayCommand]
    void Add()
    {
        if (string.IsNullOrWhiteSpace(ToDoElementName))
            return;

        _onAdd(ToElement());
        _dialogService.Close();
    }

    [RelayCommand]
    void Cancel() => _dialogService.Close();

    ToDoElement ToElement()
    {
        DateTime? dueDate = null;

        if (DueDate.HasValue)
            dueDate = DueDate.Value.Date + (DueTime ?? TimeSpan.Zero);

        return new ToDoElement
        {
            Name = ToDoElementName!.Trim(),
            DueDate = dueDate,
            CreatedAt = DateTime.Now,
            Priority = Priority,
            Tags = ParseTags(TagsInput)
        };
    }

    static System.Collections.ObjectModel.ObservableCollection<string> ParseTags(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return [];

        var tags = input
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(tag => tag.Length > 0)
            .Distinct(StringComparer.OrdinalIgnoreCase);

        return new System.Collections.ObjectModel.ObservableCollection<string>(tags);
    }
}
