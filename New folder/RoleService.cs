using OnlineMarketplace.Data.Repository;
using OnlineMarketplace.Models.ViewModel;

namespace OnlineMarketplace.Data.Services;

public class RoleService : EntityBaseRepository<RoleViewModel>, IRoleService
{
    public RoleService(ShopContext context) : base(context)
    {
        
    }
}
