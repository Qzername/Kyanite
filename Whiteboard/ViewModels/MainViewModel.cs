using Avalonia.Collections;
using AvaloniaEdit.Highlighting;
using Microsoft.AspNetCore.SignalR.Client;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Whiteboard.Models;
using Whiteboard.Modules;
using Whiteboard.Modules.Reminder;
using Whiteboard.Modules.Text;

namespace Whiteboard.ViewModels;

public class MainViewModel : ViewModelBase
{
    public AvaloniaList<ModuleInfo> modules { get; set; }
    public RoutingState Router { get; }

    ModuleManager moduleMananger;

    public MainViewModel()
    {
        Router = new RoutingState();

        moduleMananger = new();
        modules = new AvaloniaList<ModuleInfo>(UserDataManager.Modules);
    }

    public async Task SwitchModule(object moduleInfoObj)
    {
        ModuleInfo moduleInfo = (ModuleInfo)moduleInfoObj;

        Module module = null;

        if (moduleInfo.Name == nameof(TextModule))
        {
            module = new TextModule();
            Router.Navigate.Execute(module);
        }
        else if(moduleInfo.Name == nameof(ReminderModule))
        {
            module = new ReminderModule();
            Router.Navigate.Execute(module);
        }

        await moduleMananger.LoadModule(module!);
    }

    public void AddModule()
    {
        ModuleInfo moduleInfo = new ModuleInfo() { Name = nameof(ReminderModule) };
        UserDataManager.AddModule(moduleInfo);
        modules.Add(moduleInfo);
    }

    public async Task ConnectionTest()
    {
        var connection = new HubConnectionBuilder().WithUrl("ws://localhost:5000/notification").Build();

        await connection.StartAsync();
        await connection.InvokeAsync("SendMessage", "test");
    }
}
