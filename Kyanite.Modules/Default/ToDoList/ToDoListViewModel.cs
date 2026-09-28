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

internal partial class ToDoListViewModel : Module, IDisposable
{
    readonly IDialogService _dialogService;
    readonly ToDoReminderService _reminderService;
    bool _isSorting;

    [Synchronize] readonly ToDoSettings _settings = new();
    public ToDoSettings Settings => _settings;

    [Synchronize] readonly ObservableCollection<ToDoElement> _toDoElements = [];
    public ObservableCollection<ToDoElement> ToDoElements => _toDoElements;

    public ToDoListViewModel(
        ModuleInformation moduleInformation,
        IDialogService dialogService,
        NotificationServiceProvider notificationServiceProvider)
        : base(moduleInformation)
    {
        _dialogService = dialogService;
        _reminderService = new ToDoReminderService(notificationServiceProvider);
        _reminderService.Start(_toDoElements, Settings);

        HookCollectionEvents(_toDoElements);
        ApplySorting();
    }

    public void Dispose()
    {
        UnhookCollectionEvents(_toDoElements);
        _reminderService.Dispose();
    }

    [RelayCommand]
    void OpenSettingsDialog()
    {
        _dialogService.CreateBuilder()
            .WithTitle("To-Do Settings")
            .WithSize(600, 350)
            .WithViewModel(new ToDoSettingsViewModel(_dialogService, Settings, OnSettingsSaved))
            .BuildAndShow();
    }

    [RelayCommand]
    void ToggleComplete() => RequestSorting();

    [RelayCommand]
    void OpenAddNewDialog()
    {
        _dialogService.CreateBuilder()
            .WithTitle("Add new ToDo element")
            .WithSize(1050, 350)
            .WithViewModel(new AddNewToDoElementViewModel(_dialogService, OnAddNewElement))
            .BuildAndShow();
    }

    void OnSettingsSaved(ToDoSettings newSettings)
    {
        Settings.CopyFrom(newSettings);
    }

    void OnAddNewElement(ToDoElement newElement)
    {
        ToDoElements.Insert(0, newElement);
        RequestSorting();
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

    void HookCollectionEvents(ObservableCollection<ToDoElement> collection)
    {
        collection.CollectionChanged += OnToDoElementsCollectionChanged;
        foreach (var item in collection)
        {
            item.PropertyChanged += OnElementPropertyChanged;
        }
    }

    void UnhookCollectionEvents(ObservableCollection<ToDoElement> collection)
    {
        collection.CollectionChanged -= OnToDoElementsCollectionChanged;
        foreach (var item in collection)
        {
            item.PropertyChanged -= OnElementPropertyChanged;
        }
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
        if (_isSorting) return;

        if (e.PropertyName is nameof(ToDoElement.IsCompleted)
            or nameof(ToDoElement.IsPinned)
            or nameof(ToDoElement.DisplayDate))
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
        if (_isSorting) return;
        _isSorting = true;

        try
        {
            var sorted = ToDoElements
                .OrderByDescending(e => e.IsPinned)
                .ThenBy(e => e.IsCompleted)
                .ThenByDescending(e => e.DisplayDate)
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