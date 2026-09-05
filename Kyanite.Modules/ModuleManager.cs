using Kyanite.Database;
using System.Reflection;
using System.Text.Json;

namespace Kyanite.Modules;

public class ModuleManager()
{
    const string DefaultModuleNamespace = "Kyanite.Modules.Default";

    readonly List<Module> modules = [];
    public Module[] Modules => [.. modules];

    public Dictionary<string, Type> ModuleTypes { get; private set; } = [];

    public bool IsStackPrepared { get; private set; } = false;

    IServiceProvider? _serviceProvider;
    DatabaseStack? databaseStack;
    Dictionary<string, string> databaseStackData = [];

    public async Task Prepare(IServiceProvider serviceProvider, DatabaseStack databaseStack, Dictionary<string, string> databaseStackData)
    {
        _serviceProvider = serviceProvider;
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

        var module = ConvertInformationToModule(addedModule);

        var fields = value.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
        var synchronizableFields = fields.Where(p => p.GetCustomAttributes(typeof(SynchronizeAttribute), true).Length > 0);

        foreach (var synchronizableField in synchronizableFields)
        {
            var defaultFieldValue = synchronizableField.GetValue(module);
            var dataInformation = CreateDataInformation(synchronizableField.Name, defaultFieldValue);
            await databaseStack.DataRepository.AddAsync(dataInformation, addedModule.Id);
        }
        modules.Add(module);
        return module;
    }

    public async Task LoadModule(Module module)
    {
        if (databaseStack is null)
            throw new Exception("Module manager needs to be prepared before usage");

        var data = await databaseStack.DataRepository.GetAllAsync(module.ModuleId);

        var fields = module.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
        var synchronizableFields = fields.Where(p => p.GetCustomAttributes(typeof(SynchronizeAttribute), true).Length > 0);

        foreach (var synchronizableField in synchronizableFields)
        {
            var fieldData = data.FirstOrDefault(d => d.Id == synchronizableField.Name);

            if (fieldData is null)
                continue;

            synchronizableField.SetValue(module, CreateObjectFromDataInformation(fieldData));
        }

        module.Refresh();
    }

    public async Task SaveModule(Module module)
    {
        if (databaseStack is null)
            throw new Exception("Module manager needs to be prepared before usage");

        var fields = module.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
        var synchronizableFields = fields.Where(p => p.GetCustomAttributes(typeof(SynchronizeAttribute), true).Length > 0);

        foreach (var synchronizableField in synchronizableFields)
        {
            var value = synchronizableField.GetValue(module);
            DataInformation fieldData = CreateDataInformation(synchronizableField.Name, value);
            await databaseStack.DataRepository.UpdateAsync(fieldData, module.ModuleId);
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
        if (_serviceProvider is null)
            throw new Exception("Service provider is not available");

        var moduleType = ModuleTypes[moduleInformation.Type] ?? throw new Exception("Module type does not exist in ModuleManager registry");
        var instance = NativeActivator.CreateInstance(moduleType, _serviceProvider, moduleInformation) ?? throw new Exception("Something went wrong with creating module instance");
        return (Module)instance;
    }

    DataInformation CreateDataInformation(string id, object? value)
        => new()
        {
            Id = id,
            Type = value?.GetType().AssemblyQualifiedName ?? typeof(object).AssemblyQualifiedName!,
            Value = JsonSerializer.Serialize(value, value?.GetType() ?? typeof(object))
        };

    public object? CreateObjectFromDataInformation(DataInformation dataInfo)
    {
        if (dataInfo is null)
            return null;

        if (string.IsNullOrEmpty(dataInfo.Type))
            return null;

        Type? type = Type.GetType(dataInfo.Type);

        if (type is null)
            throw new InvalidOperationException($"Could not find type '{dataInfo.Type}'.");

        return JsonSerializer.Deserialize(dataInfo.Value, type);
    }
}