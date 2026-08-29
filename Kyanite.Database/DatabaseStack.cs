namespace Kyanite.Database;

public abstract class DatabaseStack(IModuleRepository moduleRepository, IDataRepository dataRepository)
{
    public readonly IModuleRepository ModuleRepository = moduleRepository;
    public readonly IDataRepository DataRepository = dataRepository;

    public abstract string FriendlyName { get; }

    /// <summary>
    /// called to do every necessary preparation for the database to be ready to use
    /// </summary>
    /// <param name="data">the data taken from <see cref="DatabaseInitializationViewModelBase"/> or saved config</param>
    /// <returns>whenever data preparation was successful or not</returns>
    public abstract Task<bool> Prepare(Dictionary<string, string> data);
    public virtual Task OnWindowClosing()
    {
        return Task.CompletedTask;
    }

    public abstract DatabaseInitializationViewModelBase CreateInitializationViewModel();
}