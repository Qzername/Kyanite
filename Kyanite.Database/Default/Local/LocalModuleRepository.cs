using Dapper;
using Microsoft.Data.Sqlite;

namespace Kyanite.Database.Default.Local;

public class LocalModuleRepository : IModuleRepository
{
    SqliteConnection? sqliteConnection;

    public void Initialize(SqliteConnection sqliteConnection)
    {
        this.sqliteConnection = sqliteConnection;
    }

    public async Task<ModuleInformation> AddAsync(ModuleInformation module)
    {
        const string insertQuery = @"
            INSERT INTO Modules (Id, Name, Type)
            VALUES (@Id, @Name, @Type);";

        const string createModuleTableQuery = @"
            CREATE TABLE IF NOT EXISTS M_@Id (
                Id TEXT PRIMARY KEY,
                Type TEXT NOT NULL,
                Value TEXT NOT NULL
            );";

        CheckForInitialization();

        var moduleWithId = module with { Id = Guid.NewGuid() };

        await sqliteConnection.ExecuteAsync(insertQuery, moduleWithId);
        await sqliteConnection.ExecuteAsync(createModuleTableQuery, moduleWithId);

        return moduleWithId;
    }

    public async Task<IEnumerable<ModuleInformation>> GetAllAsync()
    {
        CheckForInitialization();

        var modules = await sqliteConnection.QueryAsync<ModuleInformation>("SELECT * FROM Modules;");

        return modules;
    }

    public async Task<ModuleInformation> GetSingleAsync(Guid id)
    {
        CheckForInitialization();

        var module = await sqliteConnection.QuerySingleAsync<ModuleInformation>("SELECT * FROM Modules WHERE Id = @Id;", new { Id = id });

        return module;
    }

    public async Task<ModuleInformation> UpdateAsync(ModuleInformation module)
    {
        const string updateQuery = @"
            UPDATE Modules
            SET Name = @Name, Type = @Type
            WHERE Id = @Id;";

        CheckForInitialization();

        await sqliteConnection.ExecuteAsync(updateQuery, module);

        return module;
    }

    public async Task DeleteAsync(Guid id)
    {
        const string deleteQuery = @"
            DELETE FROM Modules
            WHERE Id = @Id;";

        CheckForInitialization();

        await sqliteConnection.ExecuteAsync(deleteQuery, id);
    }

    void CheckForInitialization()
    {
        if(sqliteConnection is null)
            throw new Exception("SQLite connection is not initialized. Call Initialize() before using the repository.");
    }
}