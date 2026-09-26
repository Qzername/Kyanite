using CommunityToolkit.Mvvm.ComponentModel;

namespace Kyanite.Modules.Default.ToDoList;

internal partial class ToDoElement : ObservableObject
{
    [ObservableProperty] string _name = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DisplayDate))]
    DateTime? _completedAt;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsOverdue))]
    bool _isCompleted;

    [ObservableProperty] bool _isPinned;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsOverdue))]
    DateTime? _dueDate;

    [ObservableProperty] DateTime _createdAt;

    public bool DueSoonNotified { get; set; }
    public bool OverdueNotified { get; set; }

    public DateTime DisplayDate => CompletedAt ?? CreatedAt;

    public bool IsOverdue => !IsCompleted
                         && DueDate.HasValue
                         && DueDate.Value < DateTime.Now;

    public void RefreshOverdueStatus() => OnPropertyChanged(nameof(IsOverdue));
}