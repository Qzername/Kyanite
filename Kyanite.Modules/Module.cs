using CommunityToolkit.Mvvm.ComponentModel;
using Kyanite.Database;
using System;

namespace Kyanite.Modules;

public abstract class Module : ObservableObject, IDisposable
{
    public Guid ModuleId { get; }
    public string Name { get; }

    protected Module(ModuleInformation moduleInformation)
    {
        ModuleId = moduleInformation.Id;
        Name = moduleInformation.Name;
    }

    public virtual void Refresh()
    {
        OnPropertyChanged(string.Empty);
    }

    public virtual void Dispose()
    {
    }
}
