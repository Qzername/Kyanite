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


        [HttpDelete]
        public IActionResult Delete([FromQuery] int moduleId)
        {
            using (var db = new LiteDatabase(Paths.Database))
            {
                bool tableExist = db.CollectionExists("module" + moduleId.ToString());
                bool infoExist = db.GetCollection<ModuleInfo>("Modules").Exists("$._id = " + moduleId);

                if (!infoExist && !tableExist)
                    return BadRequest();
                
                if (infoExist) 
                {
                    var collection = db.GetCollection<ModuleInfo>("Modules");
                    collection.Delete(moduleId);
                }
               
                if(tableExist)
                    db.DropCollection("module" + moduleId.ToString());
            }

            return Ok();
        }
    }
}
