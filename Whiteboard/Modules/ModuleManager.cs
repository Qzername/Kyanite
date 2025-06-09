using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Whiteboard.Models;

namespace Whiteboard.ModuleManagement;

public class ModuleManager
{
    int _moduleId; 

    const string apiPath = Paths.ServerIP+"api/Data";
    static HttpClient client = new();

    Module _module;
    public Module Module => _module;

    public async Task LoadModule(int moduleId, Module module)
    {
        _moduleId = moduleId;

        var properties = module.GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

        var synchronizedProperties = properties.Where(x => x.GetCustomAttribute(typeof(SynchronizeAttribute)) is not null);

        foreach (var property in synchronizedProperties)
        {
            var attribute = property.GetCustomAttribute<SynchronizeAttribute>();
            var response = await client.GetAsync(apiPath + $"?moduleId={moduleId}&name={attribute!.VariableName}");
           
            //if module doesnt exist, create it
            if(response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                DataItem dataItem = new DataItem()
                {
                    Name = attribute.VariableName,
                    Value = [attribute.DefaultValue],
                    Type = typeof(string).Name
                };

                var jsonDataItem = JsonConvert.SerializeObject(dataItem);

                await client.PostAsync(apiPath + $"?moduleId={moduleId}", new StringContent(jsonDataItem, Encoding.UTF8, "application/json"));
                response = await client.GetAsync(apiPath + $"?moduleId={moduleId}&name={attribute!.VariableName}");
            }

            //setting the values from the server to the module's field
            var json = await response.Content.ReadAsStringAsync();
            var item = JsonConvert.DeserializeObject<DataItem>(json);
            property.SetValue(module, item.Value[0]); 
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
        var jsonItem = JsonConvert.SerializeObject(dataItem);
        await client.PutAsync(apiPath+ $"?moduleId={_moduleId}", new StringContent(jsonItem, Encoding.UTF8, "application/json"));
    }
}
