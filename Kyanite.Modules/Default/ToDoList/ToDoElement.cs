using CommunityToolkit.Mvvm.ComponentModel;

namespace Kyanite.Modules.Default.ToDoList;

internal partial class ToDoElement : ObservableObject
{
    public string Name { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DisplayDate))]
    private DateTime? _completedAt;

    [ObservableProperty]
    private bool _isCompleted;

    [ObservableProperty]
    private bool _isPinned;
    
    public DateTime DisplayDate => CompletedAt ?? CreatedAt;
}