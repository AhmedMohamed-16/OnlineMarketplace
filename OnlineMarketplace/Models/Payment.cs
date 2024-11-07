using OnlineMarketplace.Data.Repository;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineMarketplace.Models;

public class Payment : IEntityBase
{
    [Key]
    public int Id { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    public DateTime? PaymentDate { get; set; }

    public PaymentMethod Method { get; set; }

    public PaymentStatus Status { get; set; }

    public int OrderId { get; set; }
    public virtual Order Order { get; set; } = null!;

    public string ApplicationUserId { get; set; }
    [ForeignKey(nameof(ApplicationUserId))]
    public virtual ApplicationUser? ApplicationUser { get; set; }
}