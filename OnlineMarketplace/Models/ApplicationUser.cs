using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineMarketplace.Models;

public class ApplicationUser : IdentityUser
{
    public string? FirstName { set; get; }
    
    public string? LastName { set; get; }

    public string City { get; set; } = null!;

    public string PhoneNumber {  get; set; }

    [NotMapped]
    [DisplayName("Name")]
    public string FullName => $"{FirstName} {LastName}";

}
