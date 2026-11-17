using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace Kyanite.Modules.Default.Kanban;

internal partial class ColumnViewModel : ObservableObject
{
    public string Name { get; set; } = string.Empty;
    public ObservableCollection<Item> Items { get; set; } = [];

    [ObservableProperty] string _newItemName = string.Empty;

    double _opacity = 1;

    [JsonIgnore]
    public double Opacity { get => _opacity; set => SetProperty(ref _opacity, value); }

    bool _isPendingDelete;

    [JsonIgnore]
    public bool IsPendingDelete
    {
        get => _isPendingDelete;
        set
        {
            if (!SetProperty(ref _isPendingDelete, value))
                return;

            OnPropertyChanged(nameof(DeleteButtonText));
        }
    }

    [JsonIgnore]
    public string DeleteButtonText => IsPendingDelete ? "Delete?" : "✕";

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
