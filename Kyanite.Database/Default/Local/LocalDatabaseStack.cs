using Dapper;
using Microsoft.Data.Sqlite;
using System.Data;

namespace Kyanite.Database.Default.Local;

public class LocalDatabaseStack()
    : DatabaseStack(new LocalModuleRepository(), new LocalDataRepository())
{
    protected const string DatabaseFilename = "database.db";

    public override string FriendlyName => "Local";

    SqliteConnection? connection;

    public override async Task<bool> Prepare(Dictionary<string, string> data)
    {
        const string createModuleListTableQuery = @"
              CREATE TABLE IF NOT EXISTS Modules (
                  Id TEXT PRIMARY KEY,
                  Name TEXT NOT NULL,
                  Type TEXT NOT NULL
              );";

        await CloseConnectionAsync();

        var directory = Path.GetDirectoryName(GetFilePath());

        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);

        connection = new SqliteConnection($"Data Source={GetFilePath()}");

        await connection.OpenAsync();

        ((LocalModuleRepository)ModuleRepository).Initialize(connection);
        ((LocalDataRepository)DataRepository).Initialize(connection);

        await connection.ExecuteAsync(createModuleListTableQuery);

        IsPrepared = true;
        return true;
    }

    public override async Task OnRemoved()
    {
        await CloseConnectionAsync();

        SqliteConnection.ClearAllPools();

        IsPrepared = false;
        TryDelete(GetFilePath());
    }

    /// <summary>
    /// Closes the connection without deleting the database file. Used before uploading the file
    /// to a remote storage provider.
    /// </summary>
    protected async Task CloseConnectionAsync()
    {
        if (connection is null)
            return;

        if (connection.State == ConnectionState.Open)
            await connection.CloseAsync();

        await connection.DisposeAsync();
        connection = null;
    }

    public override DatabaseInitializationViewModelBase CreateInitializationViewModel()
        => new LocalInitializationViewModel();

    protected static string GetFilePath()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        return Path.Combine(appData, DatabaseFilename);
    }

    static void TryDelete(string path)
    {
        try
        {
            if (File.Exists(path))
                File.Delete(path);
        }
        catch (IOException)
        {
            // Another process still holds the file. It will be replaced on the next launch.
        }
    }
}
