using Microsoft.AspNetCore.Mvc;

namespace ROA.Rest.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        // GET: api/<Inventory>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<Inventory>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<Inventory>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<Inventory>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<Inventory>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
