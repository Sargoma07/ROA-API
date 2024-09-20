using Confluent.Kafka;
using ROA.Domain.Events;
using ROA.Infrastructure.Data;
using ROA.Infrastructure.EventBus.Kafka;
using ROA.Payment.API.Data.Repositories;
using ROA.Payment.API.Domain;

namespace ROA.Payment.API.EventBus;

public class UserCreatedConsumeStrategy(
    IDataContextManager dataContextManager,
    ILogger<UserCreatedConsumeStrategy> logger
)
    : IConsumeStrategy<Null, UserCreatedEvent>
{
    public async Task<ConsumeResult<Null, UserCreatedEvent>> ConsumeProcessAsync(
        ConsumeResult<Null, UserCreatedEvent> result, CancellationToken cancellationToken)
    {
        var message = result.Message.Value;

        if (message is null)
        {
            logger.LogWarning("Message is null");
            return result;
        }

        var accountRepository = dataContextManager.CreateRepository<IAccountRepository>();
        var account = await accountRepository.GetByIdAsync(message.UserId);

        using var _ = logger.BeginScope(new Dictionary<string, string> { { "AccountId", message.UserId } });

        if (account != null)
        {
            logger.LogWarning("Account already exists");
            return result;
        }

        account = new Account
        {
            Id = message.UserId,
        };

        account.Balances.Add(new Account.BalanceData()
        {
            Currency = CurrencyCode.GameGold,
            Amount = 0.0m
        });

        accountRepository.Add(account);

        logger.LogInformation("Account created");
        await dataContextManager.SaveAsync();

        return result;
    }
}