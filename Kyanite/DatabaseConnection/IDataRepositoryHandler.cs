using Kyanite.Modules;
using System;
using System.Threading.Tasks;

namespace Kyanite.DatabaseConnection;

public interface IDataRepositoryHandler
{
    Task<Data[]> GetFromModuleAsync(Guid moduleId);
    Task<Data> CreateAsync(Guid moduleId, string name, string value);
    Task<bool> ExistsAsync(Guid moduleId, string dataName);
    Task UpdateAsync(Guid moduleId, string dataName, string newValue);
}
