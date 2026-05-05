using KyaniteAPI.Modules;

namespace KyaniteAPI.Contexts.Repositories;

public class DataRepository(DatabaseContext _context)
{
    public Data[] GetDataFromModule(Guid moduleId) => _context.Database.GetCollection<Data>().Find(d => d.ModuleId == moduleId).ToArray();
    public void Create(Data data) => _context.Database.GetCollection<Data>().Insert(data);
    public void Update(Guid dataId, string newValue)
    {
        var collection = _context.Database.GetCollection<Data>();

        var data = collection.FindOne(x => x.Id == dataId);
        data.Information = newValue;

        collection.Update(data);
    }
}
