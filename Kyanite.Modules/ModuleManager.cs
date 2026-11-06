using Kyanite.Database;
using System.Reflection;
using System.Text.Json;

namespace Kyanite.Modules;

public class ModuleManager()
{
    const string DefaultModuleNamespace = "Kyanite.Modules.Default";

    static readonly Dictionary<Type, FieldInfo[]> SynchronizableFieldsCache = [];

    readonly List<Module> modules = [];
    public Module[] Modules => [.. modules];

    public Dictionary<string, Type> ModuleTypes { get; private set; } = [];

    public bool IsStackPrepared { get; private set; }

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

        var foundModules = from type in assembly.GetTypes()
                           where typeof(Module).IsAssignableFrom(type) &&
                                 type != typeof(Module) &&
                                 !type.IsAbstract &&
                                 type.Namespace is not null &&
                                 type.Namespace.StartsWith(DefaultModuleNamespace)
                           select type;

        foreach (var moduleType in foundModules)
            ModuleTypes[moduleType.Name.Replace("ViewModel", string.Empty)] = moduleType;
    }

    async Task LoadSavedModulesFromDatabase()
    {
        if (databaseStack is null)
            throw new InvalidOperationException("Module manager needs to be prepared before usage.");

        if (!databaseStack.IsPrepared && !await databaseStack.Prepare(databaseStackData))
            throw new InvalidOperationException("Database stack could not be prepared.");
        var moduleInformations = await databaseStack.ModuleRepository.GetAllAsync();

        foreach (var moduleInformation in moduleInformations)
        {
            // Ignore modules that were created by an older version and are no longer registered.
            if (!ModuleTypes.ContainsKey(moduleInformation.Type))
                continue;

            modules.Add(ConvertInformationToModule(moduleInformation));
        }
    }

    public async Task<Module> CreateModule(string name, string type)
    {
        EnsurePrepared();

        type = type.Replace("ViewModel", string.Empty);

        if (!ModuleTypes.TryGetValue(type, out Type? moduleType))
            throw new InvalidOperationException($"Wrong module type: {type}. The module is not registered in ModuleManager.");

        var addedModule = await databaseStack!.ModuleRepository.AddAsync(new ModuleInformation()
        {
            Name = string.IsNullOrWhiteSpace(name) ? type : name.Trim(),
            Type = type
        });

        var module = ConvertInformationToModule(addedModule);

        foreach (var synchronizableField in GetSynchronizableFields(module.GetType()))
        {
            var defaultValue = synchronizableField.GetValue(module);
            var dataInformation = CreateDataInformation(synchronizableField.Name, defaultValue);
            await databaseStack.DataRepository.AddAsync(dataInformation, addedModule.Id);
        }

        modules.Add(module);
        return module;
    }

    public async Task LoadModule(Module module)
    {
        EnsurePrepared();

        var data = (await databaseStack!.DataRepository.GetAllAsync(module.ModuleId)).ToList();

        foreach (var synchronizableField in GetSynchronizableFields(module.GetType()))
        {
            var fieldData = data.FirstOrDefault(d => d.Id == synchronizableField.Name);

            if (fieldData is null)
            {
                // Backfill fields added in a newer version of the module.
                var defaultValue = synchronizableField.GetValue(module);
                await databaseStack.DataRepository.AddAsync(
                    CreateDataInformation(synchronizableField.Name, defaultValue),
                    module.ModuleId);

                continue;
            }

            synchronizableField.SetValue(module, CreateObjectFromDataInformation(fieldData));
        }

        module.Refresh();
    }

    public async Task SaveModule(Module module)
    {
        EnsurePrepared();

        foreach (var synchronizableField in GetSynchronizableFields(module.GetType()))
        {
            var value = synchronizableField.GetValue(module);
            var fieldData = CreateDataInformation(synchronizableField.Name, value);
            await databaseStack!.DataRepository.UpsertAsync(fieldData, module.ModuleId);
        }
    }

    public async Task DeleteModule(Module module)
    {
        EnsurePrepared();

        if (!modules.Remove(module))
            throw new InvalidOperationException("The module was not found.");

        await databaseStack!.DataRepository.DeleteStorageAsync(module.ModuleId);
        await databaseStack.ModuleRepository.DeleteAsync(module.ModuleId);

        module.Dispose();
    }

    public void ClearData()
    {
        foreach (var module in modules)
            module.Dispose();

        IsStackPrepared = false;
        modules.Clear();
        ModuleTypes.Clear();
        databaseStack = null;
        databaseStackData = [];
    }

    Module ConvertInformationToModule(ModuleInformation moduleInformation)
    {
        if (_serviceProvider is null)
            throw new InvalidOperationException("Service provider is not available.");

        if (!ModuleTypes.TryGetValue(moduleInformation.Type, out var moduleType))
            throw new InvalidOperationException($"Module type '{moduleInformation.Type}' is not registered in ModuleManager.");

        var instance = NativeActivator.CreateInstance(moduleType, _serviceProvider, moduleInformation)
            ?? throw new InvalidOperationException("Could not create the module instance.");

        return (Module)instance;
    }

    static FieldInfo[] GetSynchronizableFields(Type moduleType)
    {
        if (SynchronizableFieldsCache.TryGetValue(moduleType, out var cached))
            return cached;

        var fields = moduleType
            .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
            .Where(field => field.GetCustomAttribute<SynchronizeAttribute>(inherit: true) is not null)
            .ToArray();

        SynchronizableFieldsCache[moduleType] = fields;
        return fields;
    }

    static DataInformation CreateDataInformation(string id, object? value)
        => new()
        {
            Id = id,
            Type = value?.GetType().AssemblyQualifiedName ?? typeof(object).AssemblyQualifiedName!,
            Value = JsonSerializer.Serialize(value, value?.GetType() ?? typeof(object))
        };

    public object? CreateObjectFromDataInformation(DataInformation dataInfo)
    {
        if (dataInfo is null || string.IsNullOrEmpty(dataInfo.Type))
            return null;

        Type? type = Type.GetType(dataInfo.Type);

        if (type is null)
            throw new InvalidOperationException($"Could not find type '{dataInfo.Type}'.");

        return JsonSerializer.Deserialize(dataInfo.Value, type);
    }

    void EnsurePrepared()
    {
        if (databaseStack is null || !IsStackPrepared)
            throw new InvalidOperationException("Module manager needs to be prepared before usage.");
    }
}
