using Microsoft.AspNetCore.Mvc;
using ROA.Rest.API.Data;
using ROA.Rest.API.Mappers;

namespace ROA.Rest.API.Controllers;

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