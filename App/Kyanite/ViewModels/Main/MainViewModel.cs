using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kyanite.Controls.ErrorInformation;
using Kyanite.Controls.ModulePicker;
using Kyanite.Core.Dialogs;
using Kyanite.Database;
using Kyanite.Exceptions;
using Kyanite.Modules;
using Kyanite.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace Kyanite.ViewModels.Main;

internal partial class MainViewModel : ViewModelBase
{
    readonly IServiceProvider _serviceProvider;
    readonly ModuleManager _moduleManager;
    readonly IDialogService _dialogService;
    readonly DatabaseStackProvider _databaseStackProvider;
    readonly SemaphoreSlim _moduleSwitchLock = new(1, 1);

    [ObservableProperty] bool _isDataLoaded;
    [ObservableProperty] bool _isPaneOpen = true;

    public ObservableCollection<Module> AllModules { get; } = [];
    [ObservableProperty] Module? _selectedModule;

    public MainViewModel(
        IServiceProvider serviceProvider,
        ModuleManager moduleManager,
        IDialogService dialogService,
        DatabaseStackProvider databaseStackProvider)
    {
        AllModules.Clear();

        _serviceProvider = serviceProvider;
        _moduleManager = moduleManager;
        _dialogService = dialogService;
        _databaseStackProvider = databaseStackProvider;

        _ = PrepareModuleManager();
    }

    [RelayCommand]
    async Task ManuallySynchronizeDatabase()
    {
        if (_databaseStackProvider.ActiveStack is null)
            return;

        try
        {
            await _databaseStackProvider.ActiveStack.OnWindowClosing();
        }
        catch (Exception exception)
        {
            ShowError(exception.Message);
        }
    }

    [RelayCommand]
    void CreateModule()
    {
        _dialogService.CreateBuilder()
            .WithTitle("Create module")
            .WithSize(420, 300)
            .WithViewModel(new ModulePickerViewModel(_moduleManager))
            .AddButton("Create", dialog =>
            {
                if (dialog.ViewModel is not ModulePickerViewModel modulePicker)
                    return;

                _ = FinalizeCreateModuleSafe(modulePicker.ModuleName, modulePicker.SelectedModule);
            })
            .AddButton("Cancel", _ => { })
            .BuildAndShow();
    }

    [RelayCommand]
    async Task ResetInitialization()
    {
        try
        {
            var appSettingsService = _serviceProvider.GetRequiredService<AppSettingsService>();

            if (appSettingsService.CurrentAppSettings is null)
                throw new AppSettingsNotInitializedException();

            if (_databaseStackProvider.ActiveStack is not null)
                await _databaseStackProvider.ActiveStack.OnRemoved();

            appSettingsService.Save(appSettingsService.CurrentAppSettings with
            {
                DatabaseinformationInitialized = false,
                DatabaseStackType = null,
                DatabaseInformation = [],
            });

            _databaseStackProvider.ClearStack();
            AllModules.Clear();
            _moduleManager.ClearData();

            var shellViewModel = _serviceProvider.GetRequiredService<ShellViewModelProvider>().CreateOrGetShell();
            shellViewModel.UpdateView();
        }
        catch (Exception exception)
        {
            ShowError(exception.Message);
        }
    }

    [RelayCommand]
    async Task DeleteModule(object moduleObj)
    {
        if (moduleObj is not Module module)
            throw new InvalidOperationException("Provided object is not of a module type.");

        try
        {
            await _moduleManager.DeleteModule(module);
            AllModules.Replace(_moduleManager.Modules);

            if (SelectedModule == module)
                SelectedModule = null;
        }
        catch (Exception exception)
        {
            ShowError(exception.Message);
        }
    }

    [RelayCommand]
    void TogglePane() => IsPaneOpen = !IsPaneOpen;

    partial void OnSelectedModuleChanging(Module? oldValue, Module? newValue)
    {
        _ = SaveOldAndLoadNewSafe(oldValue, newValue);
    }

    async Task FinalizeCreateModuleSafe(string moduleName, string selectedModule)
    {
        try
        {
            if (string.IsNullOrEmpty(selectedModule))
            {
                ShowError("Select a module type first.");
                return;
            }

            var createdModule = await _moduleManager.CreateModule(moduleName, selectedModule);

            AllModules.Replace(_moduleManager.Modules);
            SelectedModule = createdModule;
        }
        catch (Exception exception)
        {
            ShowError(exception.Message);
        }
    }

    async Task SaveOldAndLoadNewSafe(Module? oldValue, Module? newValue)
    {
        await _moduleSwitchLock.WaitAsync();

        try
        {
            if (oldValue is not null)
                await _moduleManager.SaveModule(oldValue);

            if (newValue is not null)
                await _moduleManager.LoadModule(newValue);
        }
        catch (Exception exception)
        {
            ShowError(exception.Message);
        }
        finally
        {
            _moduleSwitchLock.Release();
        }
    }

    async Task PrepareModuleManager()
    {
        try
        {
            var appSettingsService = _serviceProvider.GetRequiredService<AppSettingsService>();

            if (_databaseStackProvider.ActiveStack is null)
                throw new ActiveStackNotInitializedException();

            if (_moduleManager.IsStackPrepared)
            {
                AllModules.Replace(_moduleManager.Modules);
                IsDataLoaded = true;
                return;
            }

            if (appSettingsService.CurrentAppSettings is null)
                throw new AppSettingsNotInitializedException();

            await _moduleManager.Prepare(
                _serviceProvider,
                _databaseStackProvider.ActiveStack,
                appSettingsService.CurrentAppSettings.DatabaseInformation);

            AllModules.Replace(_moduleManager.Modules);
            IsDataLoaded = true;
        }
        catch (Exception exception)
        {
            ShowError(exception.Message);
        }
    }

    void ShowError(string message)
    {
        _dialogService.CreateBuilder()
            .WithTitle("Error")
            .WithSize(460, 240)
            .WithViewModel(new ErrorInformationViewModel(message))
            .AddCloseButton()
            .BuildAndShow();
    }
}
