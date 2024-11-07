using OnlineMarketplace.Data.Repository;

namespace OnlineMarketplace.Models;

public class CartItem : IEntityBase
{
    public int Id { get; set; }

    public int Quantity { get; set; } // Quantity of the product added to the cart
    
    public int CartId { get; set; }
    public virtual Cart Cart { get; set; } = null!;

    public int ProductId { get; set; }
    public virtual Product Product { get; set; } = null!;
}
