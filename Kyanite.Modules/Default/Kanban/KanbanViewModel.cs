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
}