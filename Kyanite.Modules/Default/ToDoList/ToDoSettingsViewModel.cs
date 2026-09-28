using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kyanite.Core.Dialogs;
using Kyanite.ViewModels;
using System;

namespace Kyanite.Modules.Default.ToDoList.Dialogs;

internal partial class ToDoSettingsViewModel : ViewModelBase
{
    private readonly IDialogService _dialogService;
    private readonly Action<ToDoSettings> _onSave;

    public ToDoSettings SettingsDraft { get; }

    public ToDoSettingsViewModel(IDialogService dialogService, ToDoSettings currentSettings, Action<ToDoSettings> onSave)
    {
        _dialogService = dialogService;
        _onSave = onSave;
        SettingsDraft = new ToDoSettings();
        SettingsDraft.CopyFrom(currentSettings);
    }

    [RelayCommand]
    private void Save()
    {
        _onSave?.Invoke(SettingsDraft);
        _dialogService.Close();
    }

    [RelayCommand]
    private void Cancel()
    {
        _dialogService.Close();
    }
}