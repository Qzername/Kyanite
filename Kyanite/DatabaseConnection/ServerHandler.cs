using System;

namespace Kyanite.DatabaseConnection;

public abstract class ServerHandler
{
    public event Action OnReady;

    public abstract IDataRepositoryHandler DataRepository { get; }
    public abstract IModuleRepositoryHandler ModuleRepository { get; }

    /// <summary>
    /// On closed window, application is still running in background
    /// </summary>
    public virtual void OnApplicationClose()
    {
    }

    public virtual void OnApplicationOpen()
    {
        OnReady?.Invoke();
    }

    public virtual void OnModuleChange()
    {
    }
}
