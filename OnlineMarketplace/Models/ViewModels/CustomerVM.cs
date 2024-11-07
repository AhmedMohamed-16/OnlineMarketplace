using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OnlineMarketplace.Models;

public class CustomerVM
{
    public int Id { get; set; }

    [DisplayName("Full Name")]
    [StringLength(80)]
    [Required]
    public string FullName { get; set; }

    [StringLength(150)]
    public string? Address { get; set; }

    [Range(16, 150)]
    public int? Age { get; set; }

    [DisplayName("Date Joined")]
    public DateTime DateJoined { get; set; }
}
