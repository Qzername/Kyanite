using Kyanite.Database;
using System.Diagnostics;
using System.Reflection;

namespace Kyanite.Modules;

public class ModuleManager(DatabaseStack databaseStack)
{
    const string DefaultModuleNamespace = "Kyanite.Modules.Default";

    readonly DatabaseStack _databaseStack = databaseStack;
    readonly List<Module> modules = [];
    public Module[] Modules => [.. modules];

    public Dictionary<string, Type> ModuleTypes { get; private set; } = [];  

    public bool IsStackPrepared { get; private set; } = false;

    public async Task Prepare()
    {
        LoadModuleTypesFromAssembly();
        await LoadSavedModulesFromDatabase();

        IsStackPrepared = true;
    }

    void LoadModuleTypesFromAssembly()
    {
        var assembly = Assembly.GetExecutingAssembly();

        var foundModules = from t in Assembly.GetExecutingAssembly().GetTypes()
                           where typeof(Module).IsAssignableFrom(t) && 
                                 t != typeof(Module) && 
                                 t.Namespace is not null && 
                                 t.Namespace.StartsWith(DefaultModuleNamespace)
                           select t;

        foreach (var moduleType in foundModules)
            ModuleTypes[moduleType.Name.Replace("ViewModel", string.Empty)] = moduleType;
    }

    async Task LoadSavedModulesFromDatabase()
    {
        await _databaseStack.Prepare();

        var moduleInformations = await _databaseStack.ModuleRepository.GetAllAsync();

        foreach (var moduleInformation in moduleInformations)
            modules.Add(ConvertInformationToModule(moduleInformation));
    }

    public async Task<Module> CreateModule(string name, string type)
    {
        type = type.Replace("ViewModel", string.Empty);

        if (!ModuleTypes.TryGetValue(type, out Type? value))
            throw new Exception($"Wrong module name: {type}, module is not registered in ModuleManager");

        var addedModule = await _databaseStack.ModuleRepository.AddAsync(new ModuleInformation()
        {
            Name = name,
            Type = type
        });

        var properties = value.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
        var synchronizableProperties = properties.Where(p => p.GetCustomAttributes(typeof(SynchronizeAttribute), true).Length > 0);

        foreach (var synchronizableProperty in synchronizableProperties)
            await _databaseStack.DataRepository.AddAsync(new DataInformation()
            {
                Id = synchronizableProperty.Name,
                Type = "string", //TODO: add more supported types
                Value = string.Empty
            }, addedModule.Id);

        var module = ConvertInformationToModule(addedModule);
        modules.Add(module);
        return module;
    }

    public async Task LoadModule(Module module)
    {
        var data = await _databaseStack.DataRepository.GetAllAsync(module.ModuleId);

        var properties = module.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
        var synchronizableProperties = properties.Where(p => p.GetCustomAttributes(typeof(SynchronizeAttribute), true).Length > 0);

        foreach (var synchronizableProperty in synchronizableProperties)
        {
            var propertyData = data.FirstOrDefault(d => d.Id == synchronizableProperty.Name);

            if (propertyData is null)
                continue;

            synchronizableProperty.SetValue(module, propertyData.Value);
        }

        module.Refresh();
    }

    public async Task SaveModule(Module module)
    {
        var properties = module.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
        var synchronizableProperties = properties.Where(p => p.GetCustomAttributes(typeof(SynchronizeAttribute), true).Length > 0);

        foreach (var synchronizableProperty in synchronizableProperties)
        {
            DataInformation propertyData = new()
            {
                Id = synchronizableProperty.Name,
                Type = "string", //add more supported types
                Value = synchronizableProperty.GetValue(module)?.ToString() ?? string.Empty
            };

            await _databaseStack.DataRepository.UpdateAsync(propertyData, module.ModuleId);
        }
    }

    public async Task DeleteModule(Module module)
    {
        if (!modules.Contains(module))
            throw new Exception("Module not found");

        await _databaseStack.ModuleRepository.DeleteAsync(module.ModuleId);
        modules.Remove(module);
    }

    Module ConvertInformationToModule(ModuleInformation moduleInformation)
    {
        var moduleType = ModuleTypes[moduleInformation.Type] ?? throw new Exception("Module type does not exist in ModuleManager registry");
        var instance = Activator.CreateInstance(moduleType, moduleInformation) ?? throw new Exception("Something went wrong with creating module instance");
        return (Module)instance;
    }
}