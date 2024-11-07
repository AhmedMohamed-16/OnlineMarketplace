using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OnlineMarketplace.Models;

// Owned Table to Order
public class OrderAddress
{
    [Required(ErrorMessage = "Street is required")]
    [StringLength(100, ErrorMessage = "Street can't be longer than 100 characters")]
    public string Street { get; set; } = null!;

    [Required(ErrorMessage = "City is required")]
    [StringLength(100, ErrorMessage = "City can't be longer than 100 characters")]
    public string City { get; set; } = null!;

    [Required(ErrorMessage = "District is required")]
    [StringLength(100, ErrorMessage = "District can't be longer than 100 characters")]
    public string District { get; set; } = null!;

    [DisplayName("Postal Code (Optionally)")]
    [Range(10000, int.MaxValue, ErrorMessage = "Postal Code must is wrong")]
    public int? ZibCode { get; set; }
}
