using OnlineMarketplace.Data.Repository;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineMarketplace.Models;

public class Order : IEntityBase
{
    [Key]
    public int Id { get; set; }

    public DateTime OrderPlaced { get; set; }

    public DateTime? OrderFulfilled { get; set; }

    public OrderStatus Status { get; set; }

    public string UserId { get; set; }
    [ForeignKey(nameof(UserId))]
    public ApplicationUser User { get; set; }

    public virtual OrderAddress Address { get; set; } = null!;
    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = null!;
    public virtual Payment Payment { get; set; } = null!;

    [NotMapped]
    public decimal TotalPrice
    {
        get
        {
            if (OrderDetails?.Count > 0)
            {
                decimal total = 0;
                foreach (var item in OrderDetails)
                    total += item.Price;
                return total;
            }
            return -1;
        }
    }
}
