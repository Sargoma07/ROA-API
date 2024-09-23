using Microsoft.AspNetCore.Mvc;
using ROA.Infrastructure.Data;
using ROA.Shop.API.Mappers;

namespace ROA.Shop.API.Controllers;

public abstract class AbstractController(
    IDataContextManager dataContextManager,
    IMapperFactory mapperFactory,
    ILogger logger)
    : ControllerBase
{
    protected IDataContextManager DataContextManager { get; } = dataContextManager;
    protected IMapperFactory MapperFactory { get; } = mapperFactory;
    protected ILogger Logger { get; private set; } = logger;
}