using Microsoft.AspNetCore.Mvc;
using ROA.Infrastructure.Data;
using ROA.Payment.API.Mappers;

namespace ROA.Payment.API.Controllers;

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