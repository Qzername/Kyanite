using KyaniteAPI.Contexts.Repositories;
using KyaniteAPI.Modules;
using LiteDB;
using Microsoft.AspNetCore.Mvc;

namespace KyaniteAPI.Controllers;

public record struct CreateModuleRequest(string Name);

[ApiController]
[Route("api/[controller]")]
public class ModuleController(ModuleRepository moduleRepository) : ControllerBase
{
    [HttpGet("[action]")]
    public async Task<ActionResult<Module>> GetSingle([FromQuery] Guid moduleId)
    {
        if(!moduleRepository.Exists(moduleId))
            return NotFound();

        return Ok(moduleRepository.GetSingle(moduleId));
    }
    
    [HttpGet]
    public async Task<ActionResult<Module[]>> Get() => Ok(moduleRepository.GetAll());

    [HttpPost]
    public ActionResult Post([FromBody] CreateModuleRequest request)
    {
        var module = new Module
        {
            Id = Guid.NewGuid(),
            Name = request.Name
        };

        moduleRepository.Create(module);
        return Ok(module);
    }

    [HttpPut]
    public void Put()
    {
        throw new NotImplementedException();
    }

    [HttpDelete]
    public void Delete()
    {
        throw new NotImplementedException();
    }
}
