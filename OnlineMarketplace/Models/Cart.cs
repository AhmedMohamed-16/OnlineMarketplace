using OnlineMarketplace.Data.Repository;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineMarketplace.Models;

public class Cart : IEntityBase
{
    [Key] 
    public int Id { get; set; }

	public DateTime CreatedDate { get; set; }

    public string ApplicationUserId { get; set; }
    [ForeignKey(nameof(ApplicationUserId))]
    public virtual ApplicationUser? ApplicationUser { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = null!;

    [NotMapped]
    public decimal TotalPrice
    {
        get
        {
            decimal total = 0;
            if (CartItems != null)
            {
                foreach (var cartItem in CartItems)
                {
                    total += cartItem.Quantity * cartItem.Product.Price;
                }
            }
            return total;
        }
    }
}
