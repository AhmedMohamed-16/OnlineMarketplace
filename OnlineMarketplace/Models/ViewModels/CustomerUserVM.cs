using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OnlineMarketplace.Models;

public class CustomerUserVM
{
    [DisplayName("Full Name")]
    [StringLength(80)]
    [Required]
    public string FullName { get; set; }

    [StringLength(150)]
    public string? Address { get; set; }

    [Range(16, 150)]
    public int? Age { get; set; }

    [DisplayName("City *")]
    [Required]
    public string City { get; set; } = null!;

    [StringLength(13, MinimumLength = 13, ErrorMessage = "Phone number length must equal 13")]
    [DisplayName("Phone *")]
    [Required(ErrorMessage = "Phone number is required")]
    [RegularExpression(@"^\+201[0125]\d*$", ErrorMessage = "Phone number must be digits only and start with +201")]
    public string PhoneNumber { get; set; }


    [DisplayName("Date Joined")]
    public DateTime DateJoined { get; set; }
}
