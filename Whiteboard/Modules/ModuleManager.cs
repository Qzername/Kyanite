using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Whiteboard.Models;
using Whiteboard.Services;

namespace Whiteboard.Modules;

public class ModuleManager
{
    int _moduleId;

    IDataManager dataManager;

    public ModuleManager(IDataManager dataManager)
    {
        this.dataManager = dataManager;
    }

    public async Task LoadModule(int moduleId, Module module)
    {
        _moduleId = moduleId;

        var properties = module.GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

        var synchronizedProperties = properties.Where(x => x.GetCustomAttribute(typeof(SynchronizeAttribute)) is not null);

        foreach (var property in synchronizedProperties)
        {
            var attribute = property.GetCustomAttribute<SynchronizeAttribute>();
            

            //if date item doesnt exist, create it
            if (!dataManager.ExistDataItem(moduleId, attribute!.VariableName))
            {
                DataItem tempDataItem = new DataItem()
                {
                    Name = attribute.VariableName,
                    Value = [attribute.DefaultValue],
                    Type = typeof(string).Name
                };

                dataManager.AddDataItem(moduleId, tempDataItem);
            }

            DataItem dataItem = dataManager.GetDataItem(moduleId, attribute!.VariableName);

            //setting the values from the server to the module's field
            property.SetValue(module, dataItem.Value[0]);
        }

        module.Initialize(this, synchronizedProperties.ToArray());
    }

    public async Task SynchronizeVariable(string name, string value)
    {
        DataItem dataItem = new DataItem()
        {
            Name = name,
            Type = "string",
            Value = [value]
        };
        dataManager.UpdateDataItem(_moduleId, dataItem);
    }
}
