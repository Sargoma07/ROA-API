using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ROA.Infrastructure.Data;
using ROA.Payment.API.Data;
using ROA.Payment.API.Data.Repositories;
using ROA.Payment.API.Mappers;
using ROA.Payment.API.Models;

namespace ROA.Payment.API.Controllers;

[Produces(MediaTypeNames.Application.Json)]
[Route("api/payment/[controller]")]
[Authorize]
[ApiController]
public class AccountController(
    IAccountContext accountContext,
    IDataContextManager dataContextManager,
    IMapperFactory mapperFactory,
    ILogger<AccountController> logger)
    : AbstractController(dataContextManager, mapperFactory, logger)
{
    [HttpGet("")]
    public async Task<ActionResult<AccountModel>> GetAccount()
    {
        var accountId = accountContext.AccountId;
        
        var accountRepository = DataContextManager.CreateRepository<IAccountRepository>();
        var account = await accountRepository.GetByIdAsync(accountId);

        if (account == null)
        {
            Logger.LogError("Account {AccountId} not found", accountId);
            return NotFound();
        }

        var mapper = MapperFactory.GetMapper<IAccountMapper>();
        return mapper.MapToDto(account);
    }

}