namespace Kyanite.Database;

public abstract class DatabaseStack(IModuleRepository moduleRepository, IDataRepository dataRepository)
{
    public readonly IModuleRepository ModuleRepository = moduleRepository;
    public readonly IDataRepository DataRepository = dataRepository;

    public abstract string FriendlyName { get; }

    /// <summary>
    /// Whether <see cref="Prepare"/> completed successfully for this stack instance.
    /// </summary>
    public bool IsPrepared { get; protected set; }

    /// <summary>
    /// Called to do every necessary preparation for the database to be ready to use.
    /// </summary>
    /// <param name="data">The data taken from <see cref="DatabaseInitializationViewModelBase"/> or saved config.</param>
    /// <returns>Whether data preparation was successful.</returns>
    public abstract Task<bool> Prepare(Dictionary<string, string> data);

    public virtual Task OnWindowClosing()
    {
        return Task.CompletedTask;
    }

    public virtual Task OnRemoved()
    {
        return Task.CompletedTask;
    }

    public abstract DatabaseInitializationViewModelBase CreateInitializationViewModel();
}
