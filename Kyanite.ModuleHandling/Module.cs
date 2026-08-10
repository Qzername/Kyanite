namespace Kyanite.ModuleHandling;

public abstract class Module
{
    public Guid ModuleId { get; private set; }

    protected Module()
    {
        ModuleId = Guid.NewGuid();
    }

    protected Module(Guid moduleId)
    {
        ModuleId = moduleId;
    }
}
