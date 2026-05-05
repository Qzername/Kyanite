using Kyanite.Modules;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Kyanite.DatabaseConnection.StandardAPI;

internal class APIDataRepositoryHandler(HttpClient client) : IDataRepositoryHandler
{
    public async Task<Data[]> GetFromModuleAsync(Guid moduleId)
    {
        var result = await client.GetFromJsonAsync<Data[]>($"api/Data/GetFromModule?moduleId={moduleId}");
        return result ?? Array.Empty<Data>();
    }

    public async Task<Data> CreateAsync(Guid moduleId, string name, string value)
    {
        var response = await client.PostAsJsonAsync("api/Data", new { 
            ModuleId = moduleId,
            Name = name,
            Value = value
        });
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<Data>();
    }

    public async Task<bool> ExistsAsync(Guid moduleId, string dataName)
    {
        var datas = await GetFromModuleAsync(moduleId);

        return datas.Any(x => x.Name == dataName);
    }

    public async Task UpdateAsync(Guid moduleId, string dataName, string newValue)
    {
        var datas = await GetFromModuleAsync(moduleId);
        var data = datas.Single(x => x.Name == dataName);

        await client.PatchAsJsonAsync("api/Data", new
        {
            DataId = data.Id,
            NewValue = newValue
        });
    }
}