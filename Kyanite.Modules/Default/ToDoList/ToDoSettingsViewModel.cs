using CommunityToolkit.Mvvm.Input;
using Kyanite.Core.Dialogs;
using Kyanite.ViewModels;
using System;

namespace Kyanite.Modules.Default.ToDoList.Dialogs;

internal partial class ToDoSettingsViewModel : ViewModelBase
{
    readonly IDialogService _dialogService;
    readonly Action<ToDoSettings> _onSave;

    public ToDoSettings SettingsDraft { get; }

    public ToDoSettingsViewModel(IDialogService dialogService, ToDoSettings currentSettings, Action<ToDoSettings> onSave)
    {
        _dialogService = dialogService;
        _onSave = onSave;

        SettingsDraft = new ToDoSettings();
        SettingsDraft.CopyFrom(currentSettings);
    }

    [RelayCommand]
    void Save()
    {
        _onSave(SettingsDraft);
        _dialogService.Close();
    }

    [RelayCommand]
    void Cancel() => _dialogService.Close();
}
