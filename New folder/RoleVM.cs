using System.ComponentModel.DataAnnotations;

namespace OnlineMarketplace.Models.ViewModel;

public class RoleVM
{
    [Display(Name ="Role NAme")]
    [Required]
    public string RoleName { get; set; }
}
