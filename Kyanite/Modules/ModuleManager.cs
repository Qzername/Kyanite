using Kyanite.DatabaseConnection;
using Kyanite.Modules.Text;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Kyanite.Modules;

public class ModuleManager(ServerHandler serverHandler)
{
    public async Task<Module[]> GetModules()
    {
        var moduleDatas = await serverHandler.ModuleRepository.GetAllModulesAsync();

        Module[] modules = new Module[moduleDatas.Length];

        for(int i =0;i<moduleDatas.Length;i++)
        {
            var module = new TextModule(moduleDatas[i].Id);
            LoadModule(module);
            modules[i] = module;
        }

        return modules;
    }

    public async Task<Module> AddModule(string name)
    {
        var moduleInfo = await serverHandler.ModuleRepository.CreateAsync(name);

        var module = new TextModule(moduleInfo.Id);
        LoadModule(module);

        return module;
    }

    async void LoadModule(Module module)
    {
        var properties = module.GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        var synchronizedProperties = properties.Where(x => x.GetCustomAttribute(typeof(SynchronizeAttribute)) is not null);

        var datas = await serverHandler.DataRepository.GetFromModuleAsync(module.ModuleId);

        foreach (var property in synchronizedProperties)
        {
            var attribute = property.GetCustomAttribute<SynchronizeAttribute>();

            if (attribute is null)
                continue;

            Data currentData;

            if (!datas.Any(x => x.Name == attribute.VariableName))
                currentData = await serverHandler.DataRepository.CreateAsync(module.ModuleId, attribute.VariableName, attribute.DefaultValue);
            else
                currentData = datas.Single(x => x.Name == attribute.VariableName);

            property.SetValue(module, currentData.Information.ToString());
        }

        module.Initialize(this, synchronizedProperties.ToArray());
    }

    public async void SynchronizeVariable(Guid moduleID, string name, string value)
    {
        if(!await serverHandler.DataRepository.ExistsAsync(moduleID, name))
            await serverHandler.DataRepository.CreateAsync(moduleID, name, value);
        else
            await serverHandler.DataRepository.UpdateAsync(moduleID, name, value);
    }
}
