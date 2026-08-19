using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kyanite.Modules;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Kyanite.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    readonly ModuleManager _moduleManager;

    [ObservableProperty] bool _isLoaded;
    [ObservableProperty] Module _selectedModule;
    public ObservableCollection<Module> AllModules { get; } = [];

    [ObservableProperty] bool _isPaneOpen = true;

    public MainViewModel(ModuleManager moduleManager)
    {
        AllModules.Clear();
        _moduleManager = moduleManager;
        _ = PrepareModuleManager();
    }

    async Task PrepareModuleManager()
    {
        await _moduleManager.Prepare();

        AllModules.Clear();
        foreach (var module in _moduleManager.Modules)
            AllModules.Add(module);

        IsLoaded = true;
    }

    [RelayCommand]
    async Task CreateModule()
    {
        var createdModule = await _moduleManager.CreateModule("Note");

        AllModules.Clear();
        foreach (var module in _moduleManager.Modules)
            AllModules.Add(module);

        SelectedModule = createdModule;
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
