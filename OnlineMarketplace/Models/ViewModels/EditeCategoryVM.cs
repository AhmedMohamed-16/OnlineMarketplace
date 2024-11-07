using System.ComponentModel.DataAnnotations;

namespace OnlineMarketplace.Models;

public class EditeCategoryVM
{
    [Key]
    public int  Id { get; set; }
     
    [MaxLength(25)]
    [MinLength(3)]
     public string Name { get; set; }

    
    public string Description { get; set; }

}
