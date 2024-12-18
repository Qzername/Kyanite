using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Newtonsoft.Json;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Whiteboard.Models;

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

}
