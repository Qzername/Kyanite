using Newtonsoft.Json;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Whiteboard.Models;

namespace Whiteboard.Modules;

public class ModuleManager
{
    const string ip = "http://localhost:5000/api/DataManagement";
    static HttpClient client = new();

    Module _module;
    public Module Module => _module;

    public async Task LoadModule(Module module)
    {
        var fields = module.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

        var synchronizedFields = fields.Where(x => x.GetCustomAttribute(typeof(SynchronizeAttribute)) is not null);

        foreach (var field in synchronizedFields)
        {
            var attribute = field.GetCustomAttribute<SynchronizeAttribute>();

            var response = await client.GetStringAsync(ip + $"?name=" + attribute.VariableName);
            var item = JsonConvert.DeserializeObject<DataItem>(response);

            field.SetValue(module, item.Value[0]);
        }

        module.Initialize(this);
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
        await client.PutAsync(ip, new StringContent(jsonItem, Encoding.UTF8, "application/json"));
    }
}
