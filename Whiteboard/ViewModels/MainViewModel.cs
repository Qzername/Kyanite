using Microsoft.AspNetCore.SignalR.Client;
using ReactiveUI;
using System.Threading.Tasks;

namespace Whiteboard.ViewModels;

public class MainViewModel : ViewModelBase
{
    SynchronizedVariable<string> syncText;

    string text
    {
        get => syncText.Value;
        set
        {
            syncText.Value = value;
            this.RaisePropertyChanged(nameof(text));
        }
    }

    public MainViewModel()
    {
        _ = InitializeVariables();
    }

    async Task InitializeVariables()
    {
        syncText = new SynchronizedVariable<string>("text");
        await syncText.InitializeVariable();
        text = syncText.Value;
    }

    public async Task ConnectionTest()
    {
        var connection = new HubConnectionBuilder().WithUrl("ws://localhost:5000/notification").Build();

        await connection.StartAsync();
        await connection.InvokeAsync("SendMessage", "test");
    }

}
