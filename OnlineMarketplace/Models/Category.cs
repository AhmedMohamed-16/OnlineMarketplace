using OnlineMarketplace.Data.Repository;
using System.ComponentModel.DataAnnotations;

namespace OnlineMarketplace.Models;

public class Category : IEntityBase
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public string Description { get; set; }

    public virtual ICollection<Product> Products { get; set; }
}