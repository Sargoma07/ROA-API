using ROA.Infrastructure.Data;
using ROA.Infrastructure.Data.Mongo;
using ROA.Payment.API.Domain;

namespace ROA.Payment.API.Data.Repositories;

public class AccountRepository(IDataContext context) : AbstractRepository<Account>(context), IAccountRepository
{
    public void Add(Account account)
    {
        AddInsertCommand(account);
    }
}