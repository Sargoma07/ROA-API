using ROA.Data.Contract;
using ROA.Data.Contract.Repositories;
using ROA.Model;

namespace ROA.Data.Repositories;

public class InventoryRepository : AbstractRepository<Inventory>, IInventoryRepository
{
    public InventoryRepository(IDataContext context) : base(context)
    {
    }
    
    
}