using OnlineMarketplace.Data.Repository;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineMarketplace.Models;

public class Review : IEntityBase
{
    public int Id { get; set; }

    [Length(1, 5)]
    public int Rating { get; set; }

    public string Comment { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime UpdatedDate { get; set; }

    public string ApplicationUserId { get; set; }
    [ForeignKey(nameof(ApplicationUserId))]
    public virtual ApplicationUser? ApplicationUser { get; set; }

    public int ProductId { get; set; }
    public virtual Product Product { get; set; } = null!;
}