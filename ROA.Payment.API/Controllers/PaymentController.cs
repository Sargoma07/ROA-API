using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ROA.Infrastructure.Data;
using ROA.Payment.API.Data;
using ROA.Payment.API.Data.Repositories;
using ROA.Payment.API.Domain;
using ROA.Payment.API.Domain.Statuses;
using ROA.Payment.API.Mappers;
using ROA.Payment.API.Models;
using ROA.Payment.API.ServiceAPI.ShopService;

namespace ROA.Payment.API.Controllers;

[Produces(MediaTypeNames.Application.Json)]
[Route("api/[controller]")]
[Authorize]
[ApiController]
public class PaymentController(
    IShopServiceApi shopServiceApi,
    IAccountContext accountContext,
    IDataContextManager dataContextManager,
    IMapperFactory mapperFactory,
    ILogger<PaymentController> logger)
    : AbstractController(dataContextManager, mapperFactory, logger)
{
    private static readonly List<PaymentStatus> AllowToCancelStatuses =
        [PaymentStatus.Canceled, PaymentStatus.Processing];

    private static readonly List<PaymentStatus> AllowToExecuteStatuses =
        [PaymentStatus.Completed, PaymentStatus.Processing];

    private static string ShopId => "Shop";

    [HttpGet("{paymentId}")]
    public async Task<ActionResult<PaymentModel>> GetPayment(string paymentId)
    {
        var accountId = accountContext.AccountId;

        var paymentRepository = DataContextManager.CreateRepository<IPaymentRepository>();
        var payment = await paymentRepository.GetPaymentByAccount(paymentId, accountId);

        if (payment == null)
        {
            return NotFound();
        }

        var mapper = MapperFactory.GetMapper<IPaymentMapper>();
        return mapper.MapToDto(payment);
    }

    [HttpPost("")]
    public async Task<ActionResult<PaymentModel>> CreatePayment([FromBody] PaymentModel request)
    {
        var accountId = accountContext.AccountId;

        using var _ = Logger.BeginScope(new Dictionary<string, object>
        {
            { "AccountId", accountId },
            { "CustomerId", request.CustomerId },
            { "MerchantId", request.MerchantId }
        });

        var paymentRepository = DataContextManager.CreateRepository<IPaymentRepository>();
        var mapper = MapperFactory.GetMapper<IPaymentMapper>();

        var payment = mapper.MapFromDto(request);
        payment.Status = PaymentStatus.Processing;
        payment.AccountId = accountId;

        await CalculatePrice(payment);

        paymentRepository.AddOrUpdate(payment);
        await DataContextManager.SaveAsync();

        Logger.LogInformation("Payment {PaymentId} created", payment.Id);
        return mapper.MapToDto(payment);
    }

    [HttpPut("{paymentId}")]
    public async Task<ActionResult<PaymentModel>> UpdatePayment(string paymentId, [FromBody] PaymentModel request)
    {
        var accountId = accountContext.AccountId;

        using var _ = Logger.BeginScope(new Dictionary<string, object>
        {
            { "AccountId", accountId },
            { "CustomerId", request.CustomerId },
            { "MerchantId", request.MerchantId }
        });

        var paymentRepository = DataContextManager.CreateRepository<IPaymentRepository>();
        var mapper = MapperFactory.GetMapper<IPaymentMapper>();

        var payment = await paymentRepository.GetPaymentByAccount(paymentId, accountId);

        if (payment is null)
        {
            return NotFound();
        }

        payment = mapper.MapFromDto(request, destination: payment);
        payment.Status = PaymentStatus.Processing;

        await CalculatePrice(payment);

        paymentRepository.AddOrUpdate(payment);
        await DataContextManager.SaveAsync();

        Logger.LogInformation("Payment {PaymentId} created", payment.Id);
        return mapper.MapToDto(payment);
    }

    [HttpPost("{paymentId}/execute")]
    public async Task<ActionResult<PaymentModel>> ExecutePayment(string paymentId)
    {
        var accountId = accountContext.AccountId;

        var dataLock = DataContextManager.CreateLock($"payment-{paymentId}-execute");
        var lease = await dataLock.AcquireAsync();

        try
        {
            var paymentRepository = DataContextManager.CreateRepository<IPaymentRepository>();
            var payment = await paymentRepository.GetPaymentByAccount(paymentId, accountId);

            if (payment == null)
            {
                return NotFound();
            }

            using var _ = Logger.BeginScope(new Dictionary<string, object>
            {
                { "AccountId", accountId },
                { "CustomerId", payment.CustomerId },
                { "MerchantId", payment.MerchantId }
            });
            
            if (!AllowToExecuteStatuses.Contains(payment.Status))
            {
                return NotFound();
            }
            
            payment.Status = PaymentStatus.Completed;
            payment.AmountDetails.Amount = payment.TotalDetails.Total;
            payment.AmountDetails.Currency = payment.TotalDetails.Currency;

            paymentRepository.AddOrUpdate(payment);

            if (payment.CustomerId != ShopId)
            {
                await UpdateAccount(payment.CustomerId, payment.AmountDetails.Currency, -payment.AmountDetails.Amount);
            }

            if (payment.MerchantId != ShopId)
            {
                await UpdateAccount(payment.MerchantId, payment.AmountDetails.Currency, payment.AmountDetails.Amount);
            }

            await DataContextManager.SaveAsync();
            
            Logger.LogInformation("Payment {PaymentId} processed", payment.Id);
            var mapper = MapperFactory.GetMapper<IPaymentMapper>();
            return mapper.MapToDto(payment);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception");
            return Problem("Unhandled error");
        }
        finally
        {
            await dataLock.ReleaseAsync(lease);
        }
    }

    [HttpPost("{paymentId}/cancel")]
    public async Task<ActionResult<PaymentModel>> CancelPayment(string paymentId)
    {
        var accountId = accountContext.AccountId;

        var paymentRepository = DataContextManager.CreateRepository<IPaymentRepository>();
        var payment = await paymentRepository.GetPaymentByAccount(paymentId, accountId);

        if (payment == null)
        {
            return NotFound();
        }

        if (!AllowToCancelStatuses.Contains(payment.Status))
        {
            return NotFound();
        }

        using var _ = Logger.BeginScope(new Dictionary<string, object>
        {
            { "AccountId", accountId },
            { "CustomerId", payment.CustomerId },
            { "MerchantId", payment.MerchantId }
        });

        payment.Status = PaymentStatus.Canceled;

        paymentRepository.AddOrUpdate(payment);
        await DataContextManager.SaveAsync();

        Logger.LogInformation("Payment {PaymentId} canceled", payment.Id);
        var mapper = MapperFactory.GetMapper<IPaymentMapper>();
        return mapper.MapToDto(payment);
    }

    private async Task CalculatePrice(Domain.Payment payment)
    {
        // TODO: rework Currency
        payment.TotalDetails.Currency = CurrencyCode.GameGold;
        
        var uniqueNames = payment.Order.Lines.Select(x => x.Name).Distinct();
        var itemPrices = await shopServiceApi.GetPrices(payment.TotalDetails.Currency, uniqueNames);

        foreach (var orderLine in payment.Order.Lines)
        {
            var itemPrice = itemPrices.FirstOrDefault(x => x.UniqueName == orderLine.Name);

            if (itemPrice is null)
            {
                continue;
            }

            orderLine.PricePerUnit = itemPrice.Price;
            orderLine.Name = itemPrice.Currency;
        }

        payment.TotalDetails.Total = payment.Order.Lines.Sum(x => x.PricePerUnit * x.Count);
    }

    private async Task UpdateAccount(string accountId, string currency, decimal amount)
    {
        var accountRepository = DataContextManager.CreateRepository<IAccountRepository>();

        var account = await accountRepository.GetByIdAsync(accountId);

        if (account is null)
        {
            throw new InvalidOperationException($"Incorrect account by {accountId}");
        }

        account.AddAmount(currency, amount);
        accountRepository.AddOrUpdate(account);
    }
}