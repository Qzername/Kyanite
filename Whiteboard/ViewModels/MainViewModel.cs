using Avalonia.Collections;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using ReactiveUI;
using System.Net.Http;
using System.Text;
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

    HttpClient client;

    public MainViewModel()
    {
        Router = new RoutingState();

        moduleMananger = new();

        client = new();

        _ = GetModules();

        modules = new AvaloniaList<ModuleInfo>();
    }

    async Task GetModules()
    {
        var response = await client.GetAsync(Paths.ServerIP + "api/Module");

        var json = await response.Content.ReadAsStringAsync();
        var moduleInfos = JsonConvert.DeserializeObject<ModuleInfo[]>(json);

        modules.AddRange(moduleInfos);
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

        await moduleMananger.LoadModule(moduleInfo.ID!.Value, module!);
    }

    public async void AddModule()
    {
        ModuleInfo moduleInfo = new ModuleInfo() 
        {
            Type = nameof(TextModule),
            Name = nameof(TextModule) 
        };

        var json = JsonConvert.SerializeObject(moduleInfo);
        var response = await client.PostAsync(Paths.ServerIP + "api/Module", new StringContent(json, Encoding.UTF8, "application/json"));

        modules.Clear();
        await GetModules();
    }

    public async Task ConnectionTest()
    {
        var connection = new HubConnectionBuilder().WithUrl("ws://localhost:5000/notification").Build();

        await connection.StartAsync();
        await connection.InvokeAsync("SendMessage", "test");
    }
}
