using LiteDB;
using System;
using System.IO;

namespace Kyanite.DatabaseConnection.Local;

internal class LocalServerHandler : ServerHandler
{
    const string LocalDatabasePath = "./Kyanite/Database.db";

    public event Action OnDatabaseDirty;

    protected LiteDatabase _liteDatabase;

    public override IDataRepositoryHandler DataRepository => _localDataRepositoryHandler;
    LocalDataRepositoryHandler _localDataRepositoryHandler;

    public override IModuleRepositoryHandler ModuleRepository => _localModuleRepositoryHandler;
    LocalModuleRepositoryHandler _localModuleRepositoryHandler;

    public LocalServerHandler()
    {
        if(!File.Exists(LocalDatabasePath))
            Directory.CreateDirectory(Path.GetDirectoryName(LocalDatabasePath)!);

        OpenDatabase();

        OnApplicationOpen();//call ready
    }

    protected void OpenDatabase()
    {
        _liteDatabase = new LiteDatabase(LocalDatabasePath);

        _localModuleRepositoryHandler = new LocalModuleRepositoryHandler();
        _localDataRepositoryHandler = new LocalDataRepositoryHandler();

        _localDataRepositoryHandler.InitializeCollection(_liteDatabase);
        _localModuleRepositoryHandler.InitializeCollection(_liteDatabase);

        _localDataRepositoryHandler.OnRepositoryDirty += OnDatabaseDirty;
        _localModuleRepositoryHandler.OnRepositoryDirty += OnDatabaseDirty;
    }

    protected void CloseDatabase()
    {
        _liteDatabase.Checkpoint();
        _liteDatabase.Dispose();
    }
}