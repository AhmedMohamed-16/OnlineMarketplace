using OnlineMarketplace.Data.Repository;
using OnlineMarketplace.Models;

namespace OnlineMarketplace.Data.Services;

public interface ICustomerService : IEntityBaseRepository<Customer>
{
    Task<IList<string>> GetCustomerRolesAsync(string customerId);
}