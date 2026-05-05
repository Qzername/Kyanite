using KyaniteAPI.Contexts.Repositories;
using KyaniteAPI.Modules;
using Microsoft.AspNetCore.Mvc;

namespace KyaniteAPI.Controllers;

//support string only for now
public record struct CreateDataRequest(Guid ModuleId, string Name, string Value);

[ApiController]
[Route("api/[controller]")]
public class DataController(DataRepository dataRepository) : ControllerBase
{
    [HttpGet("[action]")]
    public ActionResult<Data[]> GetFromModule([FromQuery] Guid moduleId) => Ok(dataRepository.GetDataFromModule(moduleId));

    [HttpPost]
    public ActionResult<Data> Post([FromBody] CreateDataRequest request)
    {
        var data = new Data()
        {
            Id = Guid.NewGuid(),
            ModuleId = request.ModuleId,
            Name = request.Name,
            TypeName = typeof(string).AssemblyQualifiedName ?? "null",
            Information = request.Value
        };

        dataRepository.Create(data);
        return Ok(data);
    }

    [HttpPatch]
    public ActionResult Patch([FromBody] Guid dataId, [FromBody] string newValue)
    {
        dataRepository.Update(dataId, newValue);
        return Ok();
    }

    [HttpDelete]
    public void Delete()
    {
        throw new NotImplementedException();
    }
}