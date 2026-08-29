namespace Kyanite.Database;

public abstract class DatabaseStack(IModuleRepository moduleRepository, IDataRepository dataRepository)
{
    public readonly IModuleRepository ModuleRepository = moduleRepository;
    public readonly IDataRepository DataRepository = dataRepository;

    public abstract string FriendlyName { get; }
    
    /// <summary>
    /// called to do every necessary preparation for the database to be ready to use
    /// </summary>
    /// <returns>whenever data preparation was successful or not</returns>
    public abstract Task<bool> Prepare();
    public virtual Task OnWindowClosing()
    {
        return Task.CompletedTask;
    }

    public abstract DatabaseInitializationViewModelBase InitializationViewModel { get; }
}