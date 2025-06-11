using Avalonia.Collections;
using Microsoft.AspNetCore.SignalR.Client;
using ReactiveUI;
using System.Diagnostics;
using System.Threading.Tasks;
using Whiteboard.Models;
using Whiteboard.Modules;
using Whiteboard.Modules.Reminder;
using Whiteboard.Modules.Text;
using Whiteboard.Services;
using Whiteboard.ViewModels.Popups;

namespace Whiteboard.ViewModels;

public class MainViewModel : ViewModelBase
{
    public AvaloniaList<ModuleInfo> modules { get; set; }
    public RoutingState Router { get; }

    IDataManager dataManager;
    ModuleManager moduleMananger;
    
    public MainViewModel()
    {
        Router = new RoutingState();

        dataManager = GetService<IDataManager>();
        moduleMananger = new(dataManager);

        modules = new AvaloniaList<ModuleInfo>();
        _ = GetModules();
    }

    async Task GetModules()
    {
        modules.AddRange(dataManager.GetModules());
    }

    public async Task SwitchModule(object moduleInfoObj)
    {
        ModuleInfo moduleInfo = (ModuleInfo)moduleInfoObj;

        Module module = null;

        if (moduleInfo.Type == nameof(TextModule))
        {
            module = new TextModule();
            Router.Navigate.Execute(module);
        }
        else if(moduleInfo.Type == nameof(ReminderModule))
        {
            module = new ReminderModule();
            Router.Navigate.Execute(module);
        }

        if (module is null)
            throw new System.Exception("Wrong module info type has been read from database");

        await moduleMananger.LoadModule(moduleInfo.ID!.Value, module);
    }

    public async void AddModule()
    {
        ModuleInfo moduleInfo = new ModuleInfo() 
        {
            Type = nameof(TextModule),
            Name = nameof(TextModule) 
        };

        dataManager.AddModule(moduleInfo);

        modules.Clear();
        await GetModules();
    }

    public async void DeleteModule(object moduleInfoObj)
    {
        ModuleInfo moduleInfo = (ModuleInfo)moduleInfoObj;
        dataManager.DeleteModule(moduleInfo.ID.Value);

        modules.Clear();
        await GetModules();
    }

    public void OpenRename(object moduleInfoObj)
    {
        ModuleInfo moduleInfo = (ModuleInfo)moduleInfoObj;
        PopupViewModel.Instance.Open(new RenameViewModel(moduleInfo));
    }
}
