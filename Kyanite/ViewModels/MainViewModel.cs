using Avalonia.Collections;
using DynamicData;
using Kyanite.Modules;
using Kyanite.Modules.Text;
using ReactiveUI;
using System.Collections.Generic;
using System.Linq;

namespace Kyanite.ViewModels;

public class MainViewModel : ViewModelBase
{
    public RoutingState Router { get; }
    ModuleManager _moduleManager;

    AvaloniaList<Module> modules;

    public MainViewModel()
    {
        Router = new RoutingState();
        modules = new AvaloniaList<Module>();

        LoadFromServer();
    }

    async void LoadFromServer()
    {
        _moduleManager = GetService<ModuleManager>();
        modules.AddRange(await _moduleManager.GetModules());
    }

    public async void InitializeExampleModule()
    {
        Module currentModule;

        if (!modules.Any(x => x.GetType() == typeof(TextModule)))
        {
            currentModule = await _moduleManager.AddModule("text_module");
            modules.Add(currentModule);
        }
        else
            currentModule = modules.First(x => x.GetType() == typeof(TextModule));

        Router.Navigate.Execute(currentModule);
    }
}
