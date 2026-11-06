using Dapper;
using Microsoft.Data.Sqlite;

namespace Kyanite.Database.Default.Local;

public class LocalDataRepository : IDataRepository
{
    SqliteConnection? sqliteConnection;

    public void Initialize(SqliteConnection sqliteConnection)
    {
        this.sqliteConnection = sqliteConnection;
    }

    public async Task<DataInformation> AddAsync(DataInformation data, Guid moduleId)
    {
        string addQuery = @$"
            INSERT INTO ""{moduleId}"" (Id, Type, Value)
            VALUES (@Id, @Type, @Value);";

        CheckForInitialization();

        await sqliteConnection!.ExecuteAsync(addQuery, data);

        return data;
    }

    public async Task<IEnumerable<DataInformation>> GetAllAsync(Guid moduleId)
    {
        string getAllQuery = @$"
            SELECT Id, Type, Value
            FROM ""{moduleId}"";";

        CheckForInitialization();

        return await sqliteConnection!.QueryAsync<DataInformation>(getAllQuery);
    }

    public async Task<DataInformation> GetSingleAsync(string dataId, Guid moduleId)
    {
        string getSingleQuery = @$"
            SELECT Id, Type, Value
            FROM ""{moduleId}""
            WHERE Id = @DataId;";

        CheckForInitialization();

        return await sqliteConnection!.QuerySingleOrDefaultAsync<DataInformation>(
            getSingleQuery,
            new { DataId = dataId })
            ?? throw new InvalidOperationException($"Data entry '{dataId}' does not exist.");
    }

    public async Task<DataInformation> UpdateAsync(DataInformation data, Guid moduleId)
    {
        string updateQuery = @$"
            UPDATE ""{moduleId}""
            SET Type = @Type, Value = @Value
            WHERE Id = @Id;";

        CheckForInitialization();

        await sqliteConnection!.ExecuteAsync(updateQuery, data);
        return data;
    }

    public async Task<DataInformation> UpsertAsync(DataInformation data, Guid moduleId)
    {
        string upsertQuery = @$"
            INSERT INTO ""{moduleId}"" (Id, Type, Value)
            VALUES (@Id, @Type, @Value)
            ON CONFLICT(Id) DO UPDATE SET
                Type = excluded.Type,
                Value = excluded.Value;";

        CheckForInitialization();

        await sqliteConnection!.ExecuteAsync(upsertQuery, data);
        return data;
    }

    public async Task DeleteAsync(string dataId, Guid moduleId)
    {
        string deleteQuery = @$"
            DELETE FROM ""{moduleId}""
            WHERE Id = @DataId;";

        CheckForInitialization();

        await sqliteConnection!.ExecuteAsync(deleteQuery, new { DataId = dataId });
    }

    public async Task DeleteAllAsync(Guid moduleId)
    {
        string deleteQuery = @$"
            DELETE FROM ""{moduleId}"";";

        CheckForInitialization();

        await sqliteConnection!.ExecuteAsync(deleteQuery);
    }

    public async Task DeleteStorageAsync(Guid moduleId)
    {
        string dropQuery = @$"
            DROP TABLE IF EXISTS ""{moduleId}"";";

        CheckForInitialization();

        await sqliteConnection!.ExecuteAsync(dropQuery);
    }

    void CheckForInitialization()
    {
        if (sqliteConnection is null)
            throw new InvalidOperationException("SQLite connection is not initialized. Call Initialize() before using the repository.");
    }
}
