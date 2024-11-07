using System.ComponentModel.DataAnnotations;

namespace OnlineMarketplace.Models.ViewModels;

public class PaymentViewModel
{
    [Required]
    public int OrderId { get; set; }
    
    [Required]
    public decimal Amount { get; set; }
    
    [Required]
    public PaymentMethod Method { get; set; }


    //public string CreditCardNumber { get; set; }
    //public string ExpirationDate { get; set; }
    //public string CVV { get; set; }
}

