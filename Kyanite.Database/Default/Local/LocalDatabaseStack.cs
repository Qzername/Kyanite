using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.Sqlite;

namespace Kyanite.Database.Default.Local;

public class LocalDatabaseStack(string databaseFilename = "./database.db") 
    : DatabaseStack(new LocalModuleRepository(), new LocalDataRepository())
{
    readonly string currentDbFilename = databaseFilename;

    SqliteConnection connection;

    public override async Task<bool> Prepare()
    {
        const string createModuleListTableQuery = @"
            CREATE TABLE IF NOT EXISTS Modules (
                Id TEXT PRIMARY KEY,
                Name TEXT NOT NULL,
                Type TEXT NOT NULL
            );";

        bool newDatabase = !File.Exists(currentDbFilename);

        connection = new SqliteConnection($"Data Source={currentDbFilename}");
        await connection.OpenAsync();

        if (newDatabase)
        {
            ((LocalModuleRepository)ModuleRepository).Initialize(connection);
            ((LocalDataRepository)DataRepository).Initialize(connection);

            await connection.ExecuteAsync(createModuleListTableQuery);
        }

        return true;
    }
}