using Avalonia.Threading;
using CommunityToolkit.Mvvm.Input;
using Kyanite.Core.Dialogs;
using Kyanite.Database;
using Kyanite.Modules.Default.ToDoList.Dialogs;
using Kyanite.Services;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Kyanite.Modules.Default.ToDoList;

internal partial class ToDoListViewModel : Module
{
    readonly IDialogService _dialogService;
    readonly ToDoReminderService _reminderService;

    bool _isSorting;
    ObservableCollection<ToDoElement>? _hookedCollection;

    [Synchronize] ToDoSettings _settings = new();
    public ToDoSettings Settings => _settings;

    [Synchronize] ObservableCollection<ToDoElement> _toDoElements = [];
    public ObservableCollection<ToDoElement> ToDoElements => _toDoElements;

    public ToDoListViewModel(
        ModuleInformation moduleInformation,
        IDialogService dialogService,
        NotificationServiceProvider notificationServiceProvider)
        : base(moduleInformation)
    {
        _dialogService = dialogService;
        _reminderService = new ToDoReminderService(notificationServiceProvider);

        RehookElements();
        _reminderService.Start(_toDoElements, _settings);
    }

    public override void Refresh()
    {
        base.Refresh();

        RehookElements();
        _reminderService.Start(_toDoElements, _settings);
        RequestSorting();
    }

    public override void Dispose()
    {
        UnhookElements();
        _reminderService.Dispose();
        base.Dispose();
    }

    [RelayCommand]
    void OpenSettingsDialog()
    {
        _dialogService.CreateBuilder()
            .WithTitle("To-Do Settings")
            .WithSize(460, 340)
            .WithViewModel(new ToDoSettingsViewModel(_dialogService, _settings, OnSettingsSaved))
            .BuildAndShow();
    }

    [RelayCommand]
    void OpenAddNewDialog()
    {
        _dialogService.CreateBuilder()
            .WithTitle("Add new To-Do element")
            .WithSize(620, 440)
            .WithViewModel(new AddNewToDoElementViewModel(_dialogService, OnAddNewElement))
            .BuildAndShow();
    }

    [RelayCommand]
    void TogglePin(ToDoElement element)
    {
        element.IsPinned = !element.IsPinned;
        RequestSorting();
    }

    [RelayCommand]
    void RemoveToDoElement(ToDoElement element)
    {
        ToDoElements.Remove(element);
    }

    void OnSettingsSaved(ToDoSettings newSettings)
    {
        _settings.CopyFrom(newSettings);
    }

    void OnAddNewElement(ToDoElement element)
    {
        ToDoElements.Insert(0, element);
        RequestSorting();
    }

    void RehookElements()
    {
        UnhookElements();

        _hookedCollection = _toDoElements;
        _hookedCollection.CollectionChanged += OnToDoElementsCollectionChanged;

        foreach (var element in _hookedCollection)
            element.PropertyChanged += OnElementPropertyChanged;
    }

    void UnhookElements()
    {
        if (_hookedCollection is null)
            return;

        _hookedCollection.CollectionChanged -= OnToDoElementsCollectionChanged;

        foreach (var element in _hookedCollection)
            element.PropertyChanged -= OnElementPropertyChanged;

        _hookedCollection = null;
    }

    void OnToDoElementsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Move)
            return;

        if (e.OldItems is not null)
        {
            foreach (ToDoElement item in e.OldItems)
                item.PropertyChanged -= OnElementPropertyChanged;
        }

        if (e.NewItems is not null)
        {
            foreach (ToDoElement item in e.NewItems)
                item.PropertyChanged += OnElementPropertyChanged;
        }

        RequestSorting();
    }

    void OnElementPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (_isSorting)
            return;

        if (e.PropertyName is nameof(ToDoElement.IsCompleted)
            or nameof(ToDoElement.IsPinned)
            or nameof(ToDoElement.Priority)
            or nameof(ToDoElement.DisplayDate)
            or nameof(ToDoElement.DueDate))
        {
            RequestSorting();
        }
    }

    void RequestSorting()
    {
        Dispatcher.UIThread.Post(ApplySorting, DispatcherPriority.Background);
    }

    void ApplySorting()
    {
        if (_isSorting)
            return;

        _isSorting = true;

        try
        {
            var sorted = ToDoElements
                .OrderByDescending(element => element.IsPinned)
                .ThenBy(element => element.IsCompleted)
                .ThenByDescending(element => element.Priority)
                .ThenBy(element => element.DueDate ?? DateTime.MaxValue)
                .ThenByDescending(element => element.DisplayDate)
                .ToList();

            for (int targetIndex = 0; targetIndex < sorted.Count; targetIndex++)
            {
                var item = sorted[targetIndex];
                int currentIndex = ToDoElements.IndexOf(item);

                if (currentIndex != targetIndex)
                    ToDoElements.Move(currentIndex, targetIndex);
            }
        }
        finally
        {
            _isSorting = false;
        }
    }
}
