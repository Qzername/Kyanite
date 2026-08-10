namespace Kyanite.ModuleHandling;

public class ModuleManager
{
    readonly List<Module> modules = [];

    public void LoadModule(Module module)
    {
        modules.Add(module);
    }
}