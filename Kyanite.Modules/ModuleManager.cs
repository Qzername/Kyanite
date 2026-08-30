using Kyanite.Database;
using System.Reflection;

namespace Kyanite.Modules;

public class ModuleManager()
{
    const string DefaultModuleNamespace = "Kyanite.Modules.Default";

    readonly List<Module> modules = [];
    public Module[] Modules => [.. modules];

    public Dictionary<string, Type> ModuleTypes { get; private set; } = [];

    public bool IsStackPrepared { get; private set; } = false;

    DatabaseStack? databaseStack;
    Dictionary<string, string> databaseStackData = [];

    public async Task Prepare(DatabaseStack databaseStack, Dictionary<string, string> databaseStackData)
    {
        this.databaseStack = databaseStack;
        this.databaseStackData = databaseStackData;

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
        if (databaseStack is null)
            throw new Exception("Module manager needs to be prepared before usage");

        await databaseStack.Prepare(databaseStackData);

        var moduleInformations = await databaseStack.ModuleRepository.GetAllAsync();

        foreach (var moduleInformation in moduleInformations)
            modules.Add(ConvertInformationToModule(moduleInformation));
    }

    public async Task<Module> CreateModule(string name, string type)
    {
        if (databaseStack is null)
            throw new Exception("Module manager needs to be prepared before usage");

        type = type.Replace("ViewModel", string.Empty);

        if (!ModuleTypes.TryGetValue(type, out Type? value))
            throw new Exception($"Wrong module name: {type}, module is not registered in ModuleManager");

        var addedModule = await databaseStack.ModuleRepository.AddAsync(new ModuleInformation()
        {
            Name = name,
            Type = type
        });

        var properties = value.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
        var synchronizableProperties = properties.Where(p => p.GetCustomAttributes(typeof(SynchronizeAttribute), true).Length > 0);

        foreach (var synchronizableProperty in synchronizableProperties)
            await databaseStack.DataRepository.AddAsync(new DataInformation()
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
        if (databaseStack is null)
            throw new Exception("Module manager needs to be prepared before usage");

        var data = await databaseStack.DataRepository.GetAllAsync(module.ModuleId);

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
        if (databaseStack is null)
            throw new Exception("Module manager needs to be prepared before usage");

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

            await databaseStack.DataRepository.UpdateAsync(propertyData, module.ModuleId);
        }
    }

    public async Task DeleteModule(Module module)
    {
        if (databaseStack is null)
            throw new Exception("Module manager needs to be prepared before usage");

        if (!modules.Contains(module))
            throw new Exception("Module not found");

        await databaseStack.ModuleRepository.DeleteAsync(module.ModuleId);
        modules.Remove(module);
    }

    public void ClearData()
    {
        IsStackPrepared = false;
        modules.Clear();
        ModuleTypes.Clear();
    }

    Module ConvertInformationToModule(ModuleInformation moduleInformation)
    {
        var moduleType = ModuleTypes[moduleInformation.Type] ?? throw new Exception("Module type does not exist in ModuleManager registry");
        var instance = Activator.CreateInstance(moduleType, moduleInformation) ?? throw new Exception("Something went wrong with creating module instance");
        return (Module)instance;
    }
}