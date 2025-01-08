using LiteDB;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Whiteboard.Models;

namespace WhiteboardServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModuleController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            using (var db = new LiteDatabase(Paths.Database))
            {
                var collection = db.GetCollection<ModuleInfo>("Modules");

                return Ok(JsonConvert.SerializeObject(collection.FindAll().ToList()));
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody]ModuleInfo moduleInfo)
        {
            moduleInfo.ID = 0;

            using(var db = new LiteDatabase(Paths.Database))
            {
                var collection = db.GetCollection<ModuleInfo>("Modules");

                collection.Insert(moduleInfo);

                return Ok();
            }
        }
    }
}
