using Kyanite.Modules;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Kyanite.DatabaseConnection.API;

internal class APIModuleRepositoryHandler(HttpClient client) : IModuleRepositoryHandler
{
    public async Task<ModuleData[]> GetAllModulesAsync()
    {
        var modules = await client.GetFromJsonAsync<ModuleData[]>("api/modules");
        return modules ?? Array.Empty<ModuleData>();
    }

    public async Task<ModuleData> CreateAsync(string name)
    {
        var response = await client.PostAsJsonAsync("api/modules", new { Name = name });
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<ModuleData>();
    }
}
