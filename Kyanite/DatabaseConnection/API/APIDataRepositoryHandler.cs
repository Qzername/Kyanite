using Kyanite.Modules;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Kyanite.DatabaseConnection.API;

internal class APIDataRepositoryHandler(HttpClient client) : IDataRepositoryHandler
{
    public async Task<Data[]> GetFromModuleAsync(Guid moduleId)
    {
        var result = await client.GetFromJsonAsync<Data[]>($"data/GetFromModule?moduleId={moduleId}");
        return result ?? Array.Empty<Data>();
    }

    public async Task<Data> CreateAsync(Guid moduleId, string name, string value)
    {
        var response = await client.PostAsJsonAsync("data", new { 
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

        var response = await client.PatchAsJsonAsync("data", new
        {
            DataId = data.Id,
            NewValue = newValue
        });
    }
}