using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kyanite.Database;
using System.Collections.ObjectModel;

namespace Kyanite.Modules.Default.Kanban;

internal partial class KanbanViewModel(ModuleInformation moduleInformation) : Module(moduleInformation)
{
    [Synchronize] ObservableCollection<ColumnViewModel> _columns = [];
    public ObservableCollection<ColumnViewModel> Columns => _columns;

    [ObservableProperty] string _newColumnName = string.Empty;

    [RelayCommand]
    void AddColumn()
    {
        if (string.IsNullOrWhiteSpace(NewColumnName))
            return;

        _columns.Add(new ColumnViewModel
        {
            Name = NewColumnName.Trim()
        });

        NewColumnName = string.Empty;
    }

    [RelayCommand]
    void RemoveColumn(ColumnViewModel column) => _columns.Remove(column);

    [RelayCommand]
    void RemoveTask(Item item)
    {
        foreach (var column in _columns)
        {
            if (column.Items.Remove(item))
                break;
        }
    }
}
