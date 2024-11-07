using System.ComponentModel.DataAnnotations;

namespace OnlineMarketplace.Models;

public class LoginVM
{
    [Display(Name = "Email address")]
    [EmailAddress]
    [Required(ErrorMessage = "Email address is required")]
    public string EmailAddress { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    public bool RememberMe { get; set; }
}
