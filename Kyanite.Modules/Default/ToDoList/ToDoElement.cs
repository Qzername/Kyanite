using CommunityToolkit.Mvvm.ComponentModel;

namespace Kyanite.Modules.Default.ToDoList;

internal partial class ToDoElement : ObservableObject
{
    public string Name { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DisplayDate))]
    DateTime? _completedAt;

    [ObservableProperty] bool _isCompleted;
    [ObservableProperty] bool _isPinned;

    public DateTime DisplayDate => CompletedAt ?? CreatedAt;
}