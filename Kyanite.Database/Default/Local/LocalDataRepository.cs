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
        const string addQuery = @"
            INSERT INTO M_@ModuleId (Id, Type, Value, ModuleId)
            VALUES (@Id, @Type, @Value, @ModuleId);";

        CheckForInitialization();

        await sqliteConnection.ExecuteAsync(addQuery, new { data.Id, data.Type, data.Value, ModuleId = moduleId });

        return data;
    }

    public async Task<IEnumerable<DataInformation>> GetAllAsync(Guid moduleId)
    {
        const string getAllQuery = @"
            SELECT Id, Type, Value
            FROM M_@ModuleId;";

        CheckForInitialization();

        return await sqliteConnection.QueryAsync<DataInformation>(getAllQuery, new { ModuleId = moduleId });
    }

    public async Task<DataInformation> GetSingleAsync(string dataId, Guid moduleId)
    {
        const string getSingleQuery = @"
            SELECT Id, Type, Value
            FROM M_@ModuleId
            WHERE Id = @DataId;";

        CheckForInitialization();

        return await sqliteConnection.QuerySingleOrDefaultAsync<DataInformation>(getSingleQuery, new { DataId = dataId, ModuleId = moduleId });
    }

    public async Task<DataInformation> UpdateAsync(DataInformation data, Guid moduleId)
    {
        const string updateQuery = @"
            UPDATE M_@ModuleId
            SET Type = @Type, Value = @Value
            WHERE Id = @Id;";

        CheckForInitialization();

        await sqliteConnection.ExecuteAsync(updateQuery, new { data.Id, data.Type, data.Value, ModuleId = moduleId });
        return data;
    }

    public Task DeleteAsync(string dataId, Guid moduleId)
    {
        const string deleteQuery = @"
            DELETE FROM M_@ModuleId
            WHERE Id = @DataId;";

        CheckForInitialization();

        return sqliteConnection.ExecuteAsync(deleteQuery, new { DataId = dataId, ModuleId = moduleId });
    }

    void CheckForInitialization()
    {
        if (sqliteConnection is null)
            throw new Exception("SQLite connection is not initialized. Call Initialize() before using the repository.");
    }
}