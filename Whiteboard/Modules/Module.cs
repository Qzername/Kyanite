using ReactiveUI;
using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
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

    PropertyInfo[] properties;

    public void Initialize(ModuleManager moduleManager, PropertyInfo[] properties)
    {
        manager = moduleManager;
        this.properties = properties;

        PropertyChanged += Module_PropertyChanged;
    }

    //Synchronize value whenever property changes
    async void Module_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is null)
            throw new Exception("Property name cannot be null");

        var value = properties.Single(x => x.Name == e.PropertyName).GetValue(this)!;

        await manager.SynchronizeVariable(e.PropertyName, (string)value);
    }
}