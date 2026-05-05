using Kyanite.DatabaseConnection;
using System;
using System.Linq;
using System.Reflection;

namespace Kyanite.Modules;

public class ModuleManager(ServerHandler serverHandler)
{
    public async void LoadModule(Module module)
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

            property.SetValue(module, (string)currentData.Information);
        }

        module.Initialize(this, synchronizedProperties.ToArray());
    }

    public async void SynchronizeVariable(Guid moduleID, string name, string value)
    {
        await serverHandler.DataRepository.UpdateAsync(moduleID, name, value);
    }

}
