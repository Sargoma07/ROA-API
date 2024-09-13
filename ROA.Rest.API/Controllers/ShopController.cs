using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using ROA.Rest.API.Data;
using ROA.Rest.API.Data.Repositories;
using ROA.Rest.API.Domain;
using ROA.Rest.API.Domain.Statuses;
using ROA.Rest.API.Mappers;
using ROA.Rest.API.Models;

namespace ROA.Rest.API.Controllers;

[Produces(MediaTypeNames.Application.Json)]
[Route("api/[controller]")]
[ApiController]
public class ShopController(
    IDataContextManager dataContextManager,
    IMapperFactory mapperFactory,
    ILogger<PlayerController> logger)
    : AbstractController(dataContextManager, mapperFactory, logger)
{
    private static readonly List<PaymentStatus> AllowToCancelStatuses =
        [PaymentStatus.Canceled, PaymentStatus.Processing];

    private static readonly List<PaymentStatus> AllowToExecuteStatuses =
        [PaymentStatus.Completed, PaymentStatus.Processing];
    
    private static string ShopId => "Shop";

    [HttpGet("payment/{paymentId}")]
    public async Task<ActionResult<PaymentModel>> GetPayment(string paymentId)
    {
        var paymentRepository = DataContextManager.CreateRepository<IPaymentRepository>();
        var payment = await paymentRepository.GetByIdAsync(paymentId);

        if (payment == null)
        {
            return NotFound();
        }

        var mapper = MapperFactory.GetMapper<IPaymentMapper>();
        return mapper.MapToDto(payment);
    }

    [HttpPost("payment")]
    public async Task<ActionResult<PaymentModel>> CreatePayment([FromBody] PaymentModel request)
    {
        using var _ = Logger.BeginScope(new Dictionary<string, object>
        {
            { "CustomerId", request.CustomerId },
            { "MerchantId", request.MerchantId }
        });

        var paymentRepository = DataContextManager.CreateRepository<IPaymentRepository>();
        var mapper = MapperFactory.GetMapper<IPaymentMapper>();

        var payment = mapper.MapFromDto(request);
        payment.Status = PaymentStatus.Processing;

        await CalculatePrice(payment);

        paymentRepository.AddOrUpdate(payment);
        await DataContextManager.SaveAsync();

        Logger.LogInformation("Payment {paymentId} created", payment.Id);
        return mapper.MapToDto(payment);
    }

    [HttpPut("payment/{paymentId}")]
    public async Task<ActionResult<PaymentModel>> UpdatePayment(string paymentId, [FromBody] PaymentModel request)
    {
        using var _ = Logger.BeginScope(new Dictionary<string, object>
        {
            { "CustomerId", request.CustomerId },
            { "MerchantId", request.MerchantId }
        });

        var paymentRepository = DataContextManager.CreateRepository<IPaymentRepository>();
        var mapper = MapperFactory.GetMapper<IPaymentMapper>();

        var payment = await paymentRepository.GetByIdAsync(paymentId);

        if (payment is null)
        {
            return NotFound();
        }

        payment = mapper.MapFromDto(request, destination: payment);
        payment.Status = PaymentStatus.Processing;
        
        await CalculatePrice(payment);

        paymentRepository.AddOrUpdate(payment);
        await DataContextManager.SaveAsync();

        Logger.LogInformation("Payment {paymentId} created", payment.Id);
        return mapper.MapToDto(payment);
    }

    [HttpPost("payment/{paymentId}/execute")]
    public async Task<ActionResult<PaymentModel>> ExecutePayment(string paymentId)
    {
        var paymentRepository = DataContextManager.CreateRepository<IPaymentRepository>();
        var payment = await paymentRepository.GetByIdAsync(paymentId);

        if (payment == null)
        {
            return NotFound();
        }

        if (!AllowToExecuteStatuses.Contains(payment.Status))
        {
            return NotFound();
        }

        using var _ = Logger.BeginScope(new Dictionary<string, object>
        {
            { "CustomerId", payment.CustomerId },
            { "MerchantId", payment.MerchantId }
        });

        var dataLock = DataContextManager.CreateLock($"payment-{payment.Id}-execute");
        var lease = await dataLock.AcquireAsync();

        try
        {
            payment.Status = PaymentStatus.Completed;
            payment.AmountDetails.Amount = payment.TotalDetails.Total;

            paymentRepository.AddOrUpdate(payment);

            if (payment.CustomerId != ShopId)
            {
                await UpdateAccount(payment.CustomerId, -payment.AmountDetails.Amount);
            }

            if (payment.MerchantId != ShopId)
            {
                await UpdateAccount(payment.MerchantId, payment.AmountDetails.Amount);
            }

            await DataContextManager.SaveAsync();
        }
        finally
        {
            await dataLock.ReleaseAsync(lease);
        }

        Logger.LogInformation("Payment {paymentId} processed", payment.Id);
        var mapper = MapperFactory.GetMapper<IPaymentMapper>();
        return mapper.MapToDto(payment);
    }

    [HttpPost("payment/{paymentId}/cancel")]
    public async Task<ActionResult<PaymentModel>> CancelPayment(string paymentId)
    {
        var paymentRepository = DataContextManager.CreateRepository<IPaymentRepository>();
        var payment = await paymentRepository.GetByIdAsync(paymentId);

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
            { "CustomerId", payment.CustomerId },
            { "MerchantId", payment.MerchantId }
        });

        payment.Status = PaymentStatus.Canceled;

        paymentRepository.AddOrUpdate(payment);
        await DataContextManager.SaveAsync();

        Logger.LogInformation("Payment {paymentId} canceled", payment.Id);
        var mapper = MapperFactory.GetMapper<IPaymentMapper>();
        return mapper.MapToDto(payment);
    }

    private async Task CalculatePrice(Payment payment)
    {
        var itemPriceRepository = DataContextManager.CreateRepository<IItemPriceRepository>();
        var itemPrice = await itemPriceRepository.GetPriceList();

        foreach (var orderLine in payment.Order.Lines)
        {
            orderLine.PricePerUnit =
                itemPrice.PriceDetails.FirstOrDefault(x => x.DataSpec == orderLine.DataSpec)?.Price
                ?? 0;
        }

        payment.TotalDetails.Total = payment.Order.Lines.Sum(x => x.PricePerUnit * x.Count);
    }
    
    private async Task UpdateAccount(string accountId, decimal amount)
    {
        var playerRepository = DataContextManager.CreateRepository<IPlayerRepository>();
        
        var account = await playerRepository.GetByIdAsync(accountId);
        
        if (account is null)
        {
            throw new InvalidOperationException($"Incorrect account by {accountId}");
        }
        
        account.AddAmount(amount);
        playerRepository.AddOrUpdate(account);
    }
}