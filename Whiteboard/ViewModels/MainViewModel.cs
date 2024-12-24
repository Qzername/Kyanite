using Microsoft.AspNetCore.SignalR.Client;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Diagnostics;
using System.Threading.Tasks;
using Whiteboard.Modules;
using Whiteboard.Modules.Text;

namespace Whiteboard.ViewModels;

public class MainViewModel : ViewModelBase
{
    public RoutingState Router { get; }

    ModuleManager moduleMananger;
    TextModule textModule { get; set; }

    public MainViewModel()
    {
        Router = new RoutingState();
        _ = PrepareLoad();
    }

    public async Task ConnectionTest()
    {
        var connection = new HubConnectionBuilder().WithUrl("ws://localhost:5000/notification").Build();

        await connection.StartAsync();
        await connection.InvokeAsync("SendMessage", "test");
    }

    public async Task PrepareLoad()
    {
        moduleMananger = new();
        textModule = new();

        Router.Navigate.Execute(textModule);

        await moduleMananger.LoadModule(textModule);
    }
}
