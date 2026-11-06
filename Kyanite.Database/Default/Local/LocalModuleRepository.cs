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

        CheckForInitialization();

        var moduleWithId = module with { Id = module.Id == Guid.Empty ? Guid.NewGuid() : module.Id };

        string createModuleTableQuery = $@"
            CREATE TABLE IF NOT EXISTS ""{moduleWithId.Id}"" (
            Id TEXT PRIMARY KEY,
            Type TEXT NOT NULL,
            Value TEXT NOT NULL
        );";

        await sqliteConnection!.ExecuteAsync(insertQuery, new
        {
            Id = moduleWithId.Id.ToString(),
            moduleWithId.Name,
            moduleWithId.Type
        });

        await sqliteConnection!.ExecuteAsync(createModuleTableQuery);

        return moduleWithId;
    }

    public async Task<IEnumerable<ModuleInformation>> GetAllAsync()
    {
        CheckForInitialization();

        var rows = await sqliteConnection!.QueryAsync<ModuleRow>(
            "SELECT Id, Name, Type FROM Modules;");

        return rows.Select(ToModuleInformation).ToArray();
    }

    public async Task<ModuleInformation> GetSingleAsync(Guid id)
    {
        CheckForInitialization();

        var row = await sqliteConnection!.QuerySingleOrDefaultAsync<ModuleRow>(
            "SELECT Id, Name, Type FROM Modules WHERE Id = @Id;",
            new { Id = id.ToString() })
            ?? throw new InvalidOperationException($"Module '{id}' does not exist.");

        return ToModuleInformation(row);
    }

    public async Task<ModuleInformation> UpdateAsync(ModuleInformation module)
    {
        string updateQuery = @"
            UPDATE Modules
            SET Name = @Name, Type = @Type
            WHERE Id = @Id;";

        CheckForInitialization();

        await sqliteConnection!.ExecuteAsync(updateQuery, new
        {
            Id = module.Id.ToString(),
            module.Name,
            module.Type
        });

        return module;
    }

    public async Task DeleteAsync(Guid id)
    {
        const string deleteQuery = @"
            DELETE FROM Modules
            WHERE Id = @Id;";

        CheckForInitialization();

        await sqliteConnection!.ExecuteAsync(deleteQuery, new { Id = id.ToString() });
    }

    void CheckForInitialization()
    {
        if (sqliteConnection is null)
            throw new InvalidOperationException("SQLite connection is not initialized. Call Initialize() before using the repository.");
    }

    static ModuleInformation ToModuleInformation(ModuleRow row)
        => new()
        {
            Id = Guid.Parse(row.Id),
            Name = row.Name,
            Type = row.Type
        };

    sealed class ModuleRow
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
    }
}
