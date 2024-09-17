using Microsoft.AspNetCore.Mvc;
using ROA.Identity.API.Mappers;
using ROA.Infrastructure.Data;

namespace ROA.Identity.API.Controllers;

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