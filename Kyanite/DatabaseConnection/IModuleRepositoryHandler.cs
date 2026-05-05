using Kyanite.Modules;
using System.Threading.Tasks;

namespace Kyanite.DatabaseConnection;

public interface IModuleRepositoryHandler
{
    Task<ModuleData[]> GetAllModulesAsync();
    Task<ModuleData> CreateAsync(string name);
}
