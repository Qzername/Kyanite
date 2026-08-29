using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kyanite.Controls.ModulePicker;
using Kyanite.Dialogs;
using Kyanite.Exceptions;
using Kyanite.Modules;
using Kyanite.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Kyanite.ViewModels.Main;

internal partial class MainViewModel : ViewModelBase
{
    readonly IServiceProvider _serviceProvider;
    readonly ModuleManager _moduleManager;
    readonly DialogService _dialogService;

    [ObservableProperty] bool _isDataLoaded;
    [ObservableProperty] bool _isPaneOpen = true;

    public ObservableCollection<Module> AllModules { get; } = [];
    [ObservableProperty] Module? _selectedModule;

    public MainViewModel(IServiceProvider serviceProvider, ModuleManager moduleManager, DialogService dialogService)
    {
        AllModules.Clear();

        _serviceProvider = serviceProvider;
        _moduleManager = moduleManager;
        _dialogService = dialogService;

        _ = PrepareModuleManager();
    }

    async Task PrepareModuleManager()
    {
        DatabaseStackProvider stackProvider = _serviceProvider.GetRequiredService<DatabaseStackProvider>();
        AppSettingsService appSettingsService = _serviceProvider.GetRequiredService<AppSettingsService>();

        if (stackProvider.ActiveStack is null)
            throw new Exception("Stackproviders Active stack is needed to be selected before module manager can be prepared");

        if (!_moduleManager.IsStackPrepared)
            await _moduleManager.Prepare(stackProvider.ActiveStack, appSettingsService.CurrentAppSettings.DatabaseInformation);

        AllModules.Replace(_moduleManager.Modules);

        IsDataLoaded = true;
    }

    [RelayCommand]
    void CreateModule()
    {
        _dialogService.Show(new DialogBuilder()
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

    [RelayCommand]
    void ResetInitialization()
    {
        var appSettingsService = _serviceProvider.GetRequiredService<AppSettingsService>();

        if (appSettingsService.CurrentAppSettings is null)
            throw new AppSettingsNotInitializedException();

        appSettingsService.Save(appSettingsService.CurrentAppSettings with
        {
            DatabaseinformationInitialized = false,
            DatabaseStackType = null,
            DatabaseInformation = [],
        });

        var shellViewModel = _serviceProvider.GetRequiredService<ShellViewModel>();
        shellViewModel.UpdateView();
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
