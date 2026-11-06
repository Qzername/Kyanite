using CommunityToolkit.Mvvm.ComponentModel;
using System.Text.Json.Serialization;

namespace Kyanite.Modules.Default.Kanban;

internal partial class Item : ObservableObject
{
    [ObservableProperty] string _name = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasDescription))]
    string _description = string.Empty;

    [JsonIgnore]
    public bool HasDescription => !string.IsNullOrWhiteSpace(Description);
}
