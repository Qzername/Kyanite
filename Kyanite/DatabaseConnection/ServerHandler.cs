namespace Kyanite.DatabaseConnection;

public abstract class ServerHandler
{
    public abstract IDataRepositoryHandler DataRepository { get; }
    public abstract IModuleRepositoryHandler ModuleRepository { get; }
}
