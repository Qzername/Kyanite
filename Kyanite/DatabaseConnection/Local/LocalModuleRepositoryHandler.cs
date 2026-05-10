using Kyanite.Modules;
using LiteDB;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Kyanite.DatabaseConnection.Local;

internal class LocalModuleRepositoryHandler : IModuleRepositoryHandler
{
    ILiteCollection<ModuleData> _collection;

    public event Action OnRepositoryDirty;

    public void InitializeCollection(LiteDatabase database) => _collection = database.GetCollection<ModuleData>();

    public async Task<ModuleData> CreateAsync(string name)
    {
        var module = new ModuleData
        {
            Id = Guid.NewGuid(),
            Name = name
        };

        _collection.Insert(module);

        OnRepositoryDirty?.Invoke();

        return module;
    }

    public async Task<ModuleData[]> GetAllModulesAsync() => [.. _collection.FindAll()];
}