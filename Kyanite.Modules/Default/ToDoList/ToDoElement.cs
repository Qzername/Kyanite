using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

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

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PriorityColor))]
    [NotifyPropertyChangedFor(nameof(PriorityBrush))]
    ToDoPriority _priority = ToDoPriority.Normal;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TagsDisplay))]
    [NotifyPropertyChangedFor(nameof(HasTags))]
    ObservableCollection<string> _tags = [];

    [JsonIgnore]
    public bool DueSoonNotified { get; set; }

    [JsonIgnore]
    public bool OverdueNotified { get; set; }

    [JsonIgnore]
    public DateTime DisplayDate => CompletedAt ?? CreatedAt;

    [JsonIgnore]
    public bool IsOverdue => !IsCompleted
                         && DueDate.HasValue
                         && DueDate.Value < DateTime.Now;

    [JsonIgnore]
    public string TagsDisplay => Tags.Count == 0
        ? string.Empty
        : string.Join("  ", Tags.Select(tag => $"#{tag}"));

    [JsonIgnore]
    public bool HasTags => Tags.Count > 0;

    [JsonIgnore]
    public string PriorityColor => Priority switch
    {
        ToDoPriority.Urgent => "#E53935",
        ToDoPriority.High => "#FB8C00",
        ToDoPriority.Low => "#90A4AE",
        _ => "#42A5F5"
    };

    [JsonIgnore]
    public IBrush PriorityBrush => new SolidColorBrush(Color.Parse(PriorityColor));

    partial void OnIsCompletedChanged(bool value)
    {
        if (value)
        {
            if (CompletedAt is null)
                CompletedAt = DateTime.Now;
        }
        else
        {
            CompletedAt = null;
            DueSoonNotified = false;
            OverdueNotified = false;
        }
    }

    partial void OnDueDateChanged(DateTime? value)
    {
        DueSoonNotified = false;
        OverdueNotified = false;
    }

    public void RefreshOverdueStatus() => OnPropertyChanged(nameof(IsOverdue));

    public bool IsDueSoon(int thresholdMinutes)
    {
        if (IsCompleted || !DueDate.HasValue)
            return false;

        DateTime now = DateTime.Now;
        DateTime thresholdTime = DueDate.Value.AddMinutes(-thresholdMinutes);

        return now >= thresholdTime && now < DueDate.Value;
    }
}
