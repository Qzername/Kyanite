using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace Kyanite.Modules.Default.Kanban;

internal partial class ColumnViewModel : ObservableObject
{
    public string Name { get; set; } = string.Empty;
    public ObservableCollection<Item> Items { get; set; } = [];

    [ObservableProperty] string _newItemName = string.Empty;

    [RelayCommand]
    void AddTask()
    {
        Items.Add(new Item()
        {
            Name = NewItemName,
            Description = "desc"
        });

        NewItemName = string.Empty;
    }
}