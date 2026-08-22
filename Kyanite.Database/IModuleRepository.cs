namespace Kyanite.Database;

public interface IModuleRepository
{
     public Task<ModuleInformation> AddAsync(ModuleInformation module);
     public Task<IEnumerable<ModuleInformation>> GetAllAsync();
     public Task<ModuleInformation> GetSingleAsync(Guid id);
     public Task<ModuleInformation> UpdateAsync(ModuleInformation module);
     public Task DeleteAsync(Guid id);
}