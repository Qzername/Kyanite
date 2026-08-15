using Kyanite.Database;
using Kyanite.ModuleHandling.Modules.Note;

namespace Kyanite.ModuleHandling;

public class ModuleManager
{
    readonly DatabaseStack _databaseStack;
    readonly List<Module> modules = [];
    public Module[] Modules => [..modules];

    public ModuleManager(DatabaseStack databaseStack)
    {
        _databaseStack = databaseStack;
    }

    public async Task Prepare()
    {
        await _databaseStack.Prepare();

        var moduleInformations = await _databaseStack.ModuleRepository.GetAllAsync();
    
        foreach(var moduleInformation in moduleInformations)
        {
            var module = ConvertTypeToModule(moduleInformation.Type, moduleInformation.Id);
            modules.Add(module);
        }
    }

    public async Task<Module> CreateModule(string type)
    {
        var addedModule = await _databaseStack.ModuleRepository.AddAsync(new ModuleInformation()
        {
            Name = type,
            Type = type
        });

        var synchronizableProperties = addedModule.GetType().GetProperties().Where(p => p.GetCustomAttributes(typeof(SynchronizeAttribute), true).Any());

        foreach(var synchronizableProperty in synchronizableProperties)
            await _databaseStack.DataRepository.AddAsync(new DataInformation()
            {
                Id = synchronizableProperty.Name,
                Type = "string", //add more supported types
                Value = synchronizableProperty.GetValue(addedModule)?.ToString() ?? string.Empty
            }, addedModule.Id);

        return ConvertTypeToModule(type, addedModule.Id);
    }

    public async Task LoadModule(Module module)
    {
        var data = await _databaseStack.DataRepository.GetAllAsync(module.ModuleId);

        var synchronizableProperties = module.GetType().GetProperties().Where(p => p.GetCustomAttributes(typeof(SynchronizeAttribute), true).Any());

        foreach (var synchronizableProperty in synchronizableProperties)
        {
            var propertyData = data.FirstOrDefault(d => d.Id == synchronizableProperty.Name);
            
            if (propertyData is null)
                continue;
            
            synchronizableProperty.SetValue(module, propertyData.Value);
        }   
    }

    public async Task SaveModule(Module module)
    {
        var synchronizableProperties = module.GetType().GetProperties().Where(p => p.GetCustomAttributes(typeof(SynchronizeAttribute), true).Any());
     
        foreach (var synchronizableProperty in synchronizableProperties)
        {
            DataInformation propertyData = new DataInformation()
            {
                Id = synchronizableProperty.Name,
                Type = "string", //add more supported types
                Value = synchronizableProperty.GetValue(module)?.ToString() ?? string.Empty
            };

            await _databaseStack.DataRepository.UpdateAsync(propertyData, module.ModuleId);
        }
    }

    //TODO: support custom modules
    Module ConvertTypeToModule(string type, Guid moduleId) => type switch
    {
        "Note" => new NoteViewModel(moduleId),
        _ => throw new NotImplementedException($"Module type {type} is not implemented.")
    };
}