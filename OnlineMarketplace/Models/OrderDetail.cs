using OnlineMarketplace.Data.Repository;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineMarketplace.Models;

public class OrderDetail : IEntityBase
{
    [Key]
    public int Id { get; set; }
    
    public int Quantity { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    public int OrderId { get; set; }
    public virtual Order Order { get; set; } = null!;
	
    public int ProductId { get; set; }
    public virtual Product Product { get; set; } = null!;
}