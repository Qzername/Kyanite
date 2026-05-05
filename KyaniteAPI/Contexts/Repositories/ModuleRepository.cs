using KyaniteAPI.Modules;

namespace KyaniteAPI.Contexts.Repositories;

public class ModuleRepository(DatabaseContext _context)
{
    public bool Exists(Guid moduleId) => _context.Database.GetCollection<Module>().Exists(x => x.Id == moduleId);
    public Module GetSingle(Guid moduleId) => _context.Database.GetCollection<Module>().FindOne(x => x.Id == moduleId);
    public Module[] GetAll() => _context.Database.GetCollection<Module>().FindAll().ToArray();
    public void Create(Module module) => _context.Database.GetCollection<Module>().Insert(module);
}
