using OnlineMarketplace.Data.Repository;
using OnlineMarketplace.Models;

namespace OnlineMarketplace.Data.Services;

public class ProductService : EntityBaseRepository<Product>, IProductService
{
    public ProductService(ShopContext context) : base(context)
    {
    }
}
