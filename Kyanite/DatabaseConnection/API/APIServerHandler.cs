using Kyanite.DatabaseConnection.API;
using System.Net.Http;

namespace Kyanite.DatabaseConnection.StandardAPI;

internal class APIServerHandler : ServerHandler
{
    HttpClient _httpClient;

    public override IDataRepositoryHandler DataRepository => _dataRepositoryHandler;
    APIDataRepositoryHandler _dataRepositoryHandler;

    public override IModuleRepositoryHandler ModuleRepository => _moduleRepositoryHandler;
    APIModuleRepositoryHandler _moduleRepositoryHandler;

    public APIServerHandler()
    {
        _httpClient = new HttpClient()
        {
            BaseAddress = new System.Uri("http://localhost:5000/api/")
        };

        _dataRepositoryHandler = new APIDataRepositoryHandler(_httpClient);
        _moduleRepositoryHandler = new APIModuleRepositoryHandler(_httpClient);
    }
}
