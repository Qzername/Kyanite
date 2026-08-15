using CommunityToolkit.Mvvm.ComponentModel;

namespace Kyanite.ModuleHandling;

public abstract class Module : ObservableObject
{
    public Guid ModuleId { get; private set; }

    protected Module(Guid moduleId)
    {
        ModuleId = moduleId;
    }
}
