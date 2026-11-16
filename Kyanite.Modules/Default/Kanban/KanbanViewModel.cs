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
        _columns.Add(new ColumnViewModel()
        {
            Name = NewColumnName
        });

        NewColumnName = string.Empty;
    }

    public void MoveColumn(ColumnViewModel column, ColumnViewModel target, bool insertBefore)
    {
        if (column == target || _columns.IndexOf(target) < 0 || !_columns.Remove(column))
            return;

        int index = _columns.IndexOf(target);

        _columns.Insert(insertBefore ? index : index + 1, column);
    }

    [RelayCommand]
    void DeleteTask(Item item){
    	foreach(var column in _columns){
     		if(column.Items.Remove(item)){
       			break;
       		}
     	}
    }

    ColumnViewModel? pendingDeleteColumn;

    [RelayCommand]
    void RequestDeleteColumn(ColumnViewModel column)
    {
        if (pendingDeleteColumn == column)
        {
            _columns.Remove(column);
            pendingDeleteColumn = null;
            return;
        }

        pendingDeleteColumn?.IsPendingDelete = false;
        pendingDeleteColumn = column;
        column.IsPendingDelete = true;
    }

    public void CancelPendingDelete()
    {
        if (pendingDeleteColumn is null)
            return;

        pendingDeleteColumn.IsPendingDelete = false;
        pendingDeleteColumn = null;
    }
}
