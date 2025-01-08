using LiteDB;
using Microsoft.AspNetCore.Mvc;
using Whiteboard.Models;

namespace WhiteboardServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get([FromQuery] int moduleId, [FromQuery] string name)
        {
            using (var db = new LiteDatabase(Paths.Database))
            {
                var valuesTable = db.GetCollection<DataItem>("module" + moduleId.ToString());

                var dataItem = valuesTable.Find(x => x.Name == name);

                if (dataItem.Count() == 0)
                    return BadRequest();

                return Ok(dataItem.First());
            }
        }

        [HttpPost]
        public IActionResult Post([FromQuery] int moduleId, [FromBody] DataItem item)
        {
            using (var db = new LiteDatabase(Paths.Database))
            {
                var valuesTable = db.GetCollection<DataItem>("module"+moduleId.ToString());

                if (!valuesTable.EnsureIndex(x => x.Name, true))
                    return BadRequest();

                valuesTable.Insert(item);

                return Ok();
            }
        }

        [HttpPut]
        public IActionResult Put([FromQuery] int moduleId, [FromBody] DataItem item)
        {
            using (var db = new LiteDatabase(Paths.Database))
            {
                var valuesTable = db.GetCollection<DataItem>("module" + moduleId.ToString());

                var dataItem = valuesTable.Find(x => x.Name == item.Name);

                if (dataItem.Count() == 0)
                    return BadRequest();

                item.Id = dataItem.First().Id;

                valuesTable.Update(item);

                return Ok();
            }
        }
    }
}
