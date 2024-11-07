using OnlineMarketplace.Data.Repository;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineMarketplace.Models;

public class Product : IEntityBase
{
    [Key] 
    public int Id { get; set; }
    
    [Required]
    public string Name { get; set; } = null!;
    
    public string? Description { get; set; }

    [DisplayName("In Stock")]
    [Range(0, int.MaxValue, ErrorMessage = "Units in stock must be 0 or greater.")]
    public int UnitsInStock { get; set; }
    
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    public string? ImageURL { get; set; }

    public int CategoryId { get; set; }
    public virtual Category? Category { get; set; }

    public virtual ICollection<Review>? Reviews { get; set; }
}