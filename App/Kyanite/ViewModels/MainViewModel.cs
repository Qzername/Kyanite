using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kyanite.Controls.ModulePicker;
using Kyanite.Dialogs;
using Kyanite.Modules;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Kyanite.ViewModels;

internal partial class MainViewModel : ViewModelBase
{
    readonly ModuleManager _moduleManager;

    readonly DialogService _dialogService;
    internal DialogService DialogService => _dialogService;

    [ObservableProperty] bool _isDataLoaded;
    [ObservableProperty] bool _isPaneOpen = true;

    public ObservableCollection<Module> AllModules { get; } = [];
    [ObservableProperty] Module? _selectedModule;

    public MainViewModel(ModuleManager moduleManager, DialogService dialogService)
    {
        AllModules.Clear();
        _moduleManager = moduleManager;
        _dialogService = dialogService;

        _ = PrepareModuleManager();
    }

    async Task PrepareModuleManager()
    {
        if (!_moduleManager.IsStackPrepared)
            await _moduleManager.Prepare();

        AllModules.Replace(_moduleManager.Modules);

        IsDataLoaded = true;
    }

    [RelayCommand]
    void CreateModule()
    {
        DialogService.Show(new DialogBuilder()
            .WithTitle("Create dialog")
            .WithSize(400, 300)
            .WithViewModel(new ModulePickerViewModel(_moduleManager))
            .SetOnClose((dialog) =>
            {
                var modulePickerVM = (ModulePickerViewModel)dialog.ViewModel;
                _ = FinalizeCreateModule(modulePickerVM.ModuleName, modulePickerVM.SelectedModule);
            })
            .Build());
    }

    async Task FinalizeCreateModule(string moduleName, string selectedModule)
    {
        if (string.IsNullOrEmpty(selectedModule))
            return;

        var createdModule = await _moduleManager.CreateModule(moduleName, selectedModule);

        AllModules.Replace(_moduleManager.Modules);

        SelectedModule = createdModule;
    }

    [RelayCommand]
    async Task DeleteModule(object moduleObj)
    {
        if (moduleObj is not Module module)
            throw new Exception("Provided object is not of a module type");

        await _moduleManager.DeleteModule(module);
        AllModules.Replace(_moduleManager.Modules);
    }

    [RelayCommand] void TogglePane() => IsPaneOpen = !IsPaneOpen;

    [RelayCommand]
    void CloseDialog()
    {
        DialogService.Close();
    }

    partial void OnSelectedModuleChanging(Module? oldValue, Module? newValue)
    {
        _ = SaveOldAndLoadNew(oldValue, newValue);
    }

    async Task SaveOldAndLoadNew(Module? oldValue, Module? newValue)
    {
        if (oldValue is not null)
            await _moduleManager.SaveModule(oldValue);

        if (newValue is not null)
            await _moduleManager.LoadModule(newValue);
    }
}
