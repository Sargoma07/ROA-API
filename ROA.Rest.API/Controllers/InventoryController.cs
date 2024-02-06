using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using ROA.Data.Contract;
using ROA.Data.Contract.Repositories;
using ROA.Model;

namespace ROA.Rest.API.Controllers
{
    [Produces(MediaTypeNames.Application.Json)]
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly IDataContextManager _dataContextManager;
        private readonly ILogger<InventoryController> _logger;

        public InventoryController(IDataContextManager dataContextManager, ILogger<InventoryController> logger)
        {
            _dataContextManager = dataContextManager;
            _logger = logger;
        }
        
        // GET: api/<Inventory>
        [HttpGet]
        public async Task<IEnumerable<Inventory>> Get()
        {
            var inventoryRepository = _dataContextManager.CreateRepository<IInventoryRepository>();
            var items = await inventoryRepository.GetAllAsync();
            return items;
        }

        // GET api/<Inventory>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value1as123df";
        }

        // POST api/<Inventory>
        [HttpPost]
        public async Task<OkResult> Post()
        {
            using var _ = _logger.BeginScope("Order {OrderId} for customer {CustomerId}", 54, 12345);
            _logger.LogInformation( "POST {test} {data} {@inventory}", 123, "terst", new Inventory{}.ToString());

            var inventoryRepository = _dataContextManager.CreateRepository<IInventoryRepository>();
            // inventoryRepository.AddOrUpdate(new Inventory());
            // await _dataContextManager.SaveAsync();

            return Ok();

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
