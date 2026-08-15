using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace Kyanite.Modules;

public abstract class Module : ObservableObject
{
    public Guid ModuleId { get; private set; }

    protected Module(Guid moduleId)
    {
        ModuleId = moduleId;
    }

    public void Refresh()
    {
        OnPropertyChanged();
    }
}
