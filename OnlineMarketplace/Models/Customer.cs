using OnlineMarketplace.Data.Repository;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineMarketplace.Models;

public class Customer : IEntityBase
{
    [Key]
    public int Id { get; set; }
    
    [DisplayName("Full Name")]
    [StringLength(80)]
    [Required]
    public string FullName { get; set; }

    public string? Address { get; set; }

    public int? Age { get; set; }

    public DateTime DateJoined { get; set; }

    public string ApplicationUserId { get; set; }
    [ForeignKey(nameof(ApplicationUserId))]
    public virtual ApplicationUser? ApplicationUser { get; set; }
}
