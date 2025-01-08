using ReactiveUI;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Whiteboard.Modules;

public abstract class Module : ReactiveObject, INotifyPropertyChanged, IRoutableViewModel
{
    ModuleManager manager;

    //IRoutableViewModel
    public IScreen HostScreen { get; }
    public string UrlPathSegment { get; } = Guid.NewGuid().ToString().Substring(0, 5);

    public void Initialize(ModuleManager moduleManager) 
    {
        manager = moduleManager;

        var properties = GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

        foreach (var property in properties)
            OnPropertyChanged(property.Name);
    }

    protected async Task SendChangesToServer(string name, string value) => await manager.SynchronizeVariable(name, value);

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}