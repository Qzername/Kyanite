using Dapper;
using Microsoft.Data.Sqlite;

namespace Kyanite.Database.Default.Local;

public class LocalDatabaseStack()
    : DatabaseStack(new LocalModuleRepository(), new LocalDataRepository())
{
    protected const string DatabaseFilename = "./database.db";

    public override string FriendlyName => "Local";

    SqliteConnection connection;

    public override async Task<bool> Prepare(Dictionary<string, string> data)
    {
        const string createModuleListTableQuery = @"
              CREATE TABLE IF NOT EXISTS Modules (
                  Id TEXT PRIMARY KEY,
                  Name TEXT NOT NULL,
                  Type TEXT NOT NULL
              );";

        connection = new SqliteConnection($"Data Source={DatabaseFilename}");
        SqlMapper.AddTypeHandler(new GuidHandler());
        await connection.OpenAsync();

        ((LocalModuleRepository)ModuleRepository).Initialize(connection);
        ((LocalDataRepository)DataRepository).Initialize(connection);

        await connection.ExecuteAsync(createModuleListTableQuery);

        return true;
    }

    public override DatabaseInitializationViewModelBase CreateInitializationViewModel()
        => new LocalInitializationViewModel();
}