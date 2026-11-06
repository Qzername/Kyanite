using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace Kyanite.Modules.Default.Kanban;

internal partial class ColumnViewModel : ObservableObject
{
    [ObservableProperty] string _name = string.Empty;
    [ObservableProperty] string _newItemName = string.Empty;
    [ObservableProperty] string _newItemDescription = string.Empty;

    public ObservableCollection<Item> Items { get; set; } = [];

    [RelayCommand]
    void AddTask()
    {
        if (string.IsNullOrWhiteSpace(NewItemName))
            return;

        Items.Add(new Item
        {
            Name = NewItemName.Trim(),
            Description = NewItemDescription.Trim()
        });

        NewItemName = string.Empty;
        NewItemDescription = string.Empty;
    }
}
