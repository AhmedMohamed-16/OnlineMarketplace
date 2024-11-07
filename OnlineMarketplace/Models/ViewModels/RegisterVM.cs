using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OnlineMarketplace.Models;

public class RegisterVM
{
    [StringLength(20)]
    [Display(Name = "First Name *")]
    [Required(ErrorMessage = "First name is required")]
    public string FirstName { get; set; }

    [StringLength(20)]
    [Display(Name = "Last Name *")]
    [Required(ErrorMessage = "Last name is required")]
    public string LastName { get; set; }

    [StringLength(100)]
    [Display(Name = "Email Address *")]
    [Required(ErrorMessage = "Email Address is required")]
    [EmailAddress]
    public string EmailAddress { get; set; }

    [StringLength(13, MinimumLength = 13, ErrorMessage = "Phone number length must equal 13")]
    [DisplayName("Phone *")]
    [Required(ErrorMessage = "Phone number is required")]
    [RegularExpression(@"^\+201[0125]\d*$", ErrorMessage = "Phone number must be digits only and start with +201")]
    public string PhoneNumber { get; set; }

    [DisplayName("City *")]
    [Required]
    public string City { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Display(Name = "Confirm Password")]
    [Required(ErrorMessage = "Confirm password is required")]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Passwords do not match")]
    public string ConfirmPassword { get; set; }
}