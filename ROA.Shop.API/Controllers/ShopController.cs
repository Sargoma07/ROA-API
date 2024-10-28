using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using ROA.Infrastructure.Data;
using ROA.Shop.API.Data.Repositories;
using ROA.Shop.API.Mappers;
using ROA.Shop.API.Models;

namespace ROA.Shop.API.Controllers;

[Produces(MediaTypeNames.Application.Json)]
[Route("api/[controller]")]
[ApiController]
public class ShopController(
    IDataContextManager dataContextManager,
    IMapperFactory mapperFactory,
    ILogger<ShopController> logger)
    : AbstractController(dataContextManager, mapperFactory, logger)
{
    [HttpGet("prices/{currency}")]
    public async Task<IEnumerable<ItemPriceModel>> GetPrices(string currency, [FromQuery] GetPricesRequest request)
    {
        var itemPriceRepository = DataContextManager.CreateRepository<IItemPriceRepository>();
        var itemPrices = await itemPriceRepository.GetPriceList(request.UniqueNames.Distinct());

        itemPrices = itemPrices.Select(x =>
            {
                x.Details = x.Details.Where(d => d.Currency == currency);
                return x;
            })
            .Where(x => x.Details.Any());
        
        var mapper = MapperFactory.GetMapper<IItemPriceMapper>();
        return mapper.MapCollectionToDto(itemPrices);
    }
}