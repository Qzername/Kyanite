using Kyanite.Database;
using Kyanite.Modules.Note;
using System.Reflection;

namespace Kyanite.Modules;

public class ModuleManager
{
    readonly DatabaseStack _databaseStack;
    readonly List<Module> modules = [];
    public Module[] Modules => [.. modules];

    public bool IsStackPrepared { get; private set; } = false;

    public ModuleManager(DatabaseStack databaseStack)
    {
        _databaseStack = databaseStack;
    }

    public async Task Prepare()
    {
        await _databaseStack.Prepare();

        var moduleInformations = await _databaseStack.ModuleRepository.GetAllAsync();

        foreach (var moduleInformation in moduleInformations)
        {
            var module = ConvertTypeToModule(moduleInformation.Type, moduleInformation);
            modules.Add(module);
        }

        IsStackPrepared = true;
    }

    public async Task<Module> CreateModule(string type)
    {
        var addedModule = await _databaseStack.ModuleRepository.AddAsync(new ModuleInformation()
        {
            Name = type,
            Type = type
        });

        var properties = typeof(NoteViewModel).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
        var synchronizableProperties = properties.Where(p => p.GetCustomAttributes(typeof(SynchronizeAttribute), true).Length > 0);

        foreach (var synchronizableProperty in synchronizableProperties)
            await _databaseStack.DataRepository.AddAsync(new DataInformation()
            {
                Id = synchronizableProperty.Name,
                Type = "string", //add more supported types
                Value = string.Empty
            }, addedModule.Id);

        var module = ConvertTypeToModule(type, addedModule);
        modules.Add(module);
        return module;
    }

    public async Task LoadModule(Module module)
    {
        var data = await _databaseStack.DataRepository.GetAllAsync(module.ModuleId);

        var properties = typeof(NoteViewModel).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
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
        var properties = typeof(NoteViewModel).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
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

    //TODO: support custom modules
    static Module ConvertTypeToModule(string type, ModuleInformation moduleInformation) => type switch
    {
        "Note" => new NoteViewModel(moduleInformation),
        _ => throw new NotImplementedException($"Module type {type} is not implemented.")
    };
}