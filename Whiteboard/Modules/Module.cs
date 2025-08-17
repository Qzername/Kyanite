using ReactiveUI;
using Splat;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

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

        if (!properties.Any((x) => x.Name == e.PropertyName))
            return;

        var value = properties.Single(x => x.Name == e.PropertyName).GetValue(this)!;

        manager.SynchronizeVariable(e.PropertyName, (string)value);
    }

    protected T GetService<T>()
    {
        var service = Locator.Current.GetService<T>();

        if (service is null)
            throw new System.Exception("Unknown service type: " + typeof(T).FullName);

        return service;
    }
}