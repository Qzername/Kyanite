using Kyanite.Modules;
using System;
using System.Threading.Tasks;
using LiteDB;
using System.Linq;

namespace Kyanite.DatabaseConnection.Local;

internal class LocalDataRepositoryHandler : IDataRepositoryHandler
{
    ILiteCollection<Data> _collection;

    public event Action OnRepositoryDirty;

    public void InitializeCollection(LiteDatabase database) => _collection = database.GetCollection<Data>();

    public async Task<Data> CreateAsync(Guid moduleId, string name, string value)
    {
        var data = new Data
        {
            ModuleId = moduleId,
            Name = name,
            Information = value
        };

        _collection.Insert(data);
        OnRepositoryDirty?.Invoke();
        return data;
    }

    public async Task<bool> ExistsAsync(Guid moduleId, string dataName) => _collection.Exists(x => x.ModuleId == moduleId && x.Name == dataName);
    public async Task<Data[]> GetFromModuleAsync(Guid moduleId) => _collection.Find(x => x.ModuleId == moduleId).ToArray();

    public async Task UpdateAsync(Guid moduleId, string dataName, string newValue)
    {
        var record = _collection.FindOne(x => x.ModuleId == moduleId && x.Name == dataName);
        record.Information = newValue;
        _collection.Update(record);

        OnRepositoryDirty?.Invoke();
    }
}