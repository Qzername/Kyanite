namespace Kyanite.Database;

public interface IDataRepository
{
     public Task<DataInformation> AddAsync(DataInformation data, Guid moduleId);
     public Task<IEnumerable<DataInformation>> GetAllAsync(Guid moduleId);
     public Task<DataInformation> GetSingleAsync(string dataId, Guid moduleId);
     public Task<DataInformation> UpdateAsync(DataInformation data, Guid moduleId);
     public Task DeleteAsync(string dataId, Guid moduleId);
}