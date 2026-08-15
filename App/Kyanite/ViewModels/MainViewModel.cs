using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kyanite.Modules;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Kyanite.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    readonly ModuleManager _moduleManager;

    [ObservableProperty] bool _isLoaded;
    [ObservableProperty] Module _currentModule;

    public MainViewModel(ModuleManager moduleManager)
    {
        _moduleManager = moduleManager;
        _ = PrepareModuleManager();
    }

    async Task PrepareModuleManager()
    {
        await _moduleManager.Prepare();
        IsLoaded = true;
    }

    [RelayCommand]
    async Task CreateModule()
    {
        await _moduleManager.CreateModule("Note");
    }

    [RelayCommand]
    async Task LoadModule()
    {
        var module = _moduleManager.Modules[0];
        await _moduleManager.LoadModule(module);
        CurrentModule = module;
    }

    [RelayCommand]
    async Task SaveModule()
    {
        await _moduleManager.SaveModule(CurrentModule);
    }
}
