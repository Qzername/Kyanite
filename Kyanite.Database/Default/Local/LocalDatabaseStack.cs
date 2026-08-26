using Dapper;
using Microsoft.Data.Sqlite;

namespace Kyanite.Database.Default.Local;

public class LocalDatabaseStack(string databaseFilename = "./database.db")
    : DatabaseStack(new LocalModuleRepository(), new LocalDataRepository())
{
     protected readonly string currentDbFilename = databaseFilename;
     
     SqliteConnection connection;
     
     public override async Task<bool> Prepare()
     {
          const string createModuleListTableQuery = @"
              CREATE TABLE IF NOT EXISTS Modules (
                  Id TEXT PRIMARY KEY,
                  Name TEXT NOT NULL,
                  Type TEXT NOT NULL
              );";
          
          connection = new SqliteConnection($"Data Source={currentDbFilename}");
          SqlMapper.AddTypeHandler(new GuidHandler());
          await connection.OpenAsync();
          
          ((LocalModuleRepository)ModuleRepository).Initialize(connection);
          ((LocalDataRepository)DataRepository).Initialize(connection);
          
          await connection.ExecuteAsync(createModuleListTableQuery);
          
          return true;
     }
}