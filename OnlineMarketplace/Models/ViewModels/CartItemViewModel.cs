// File: Models/CartItemViewModel.cs
namespace OnlineMarketplace.Models;

public class CartItemViewModel
{
    public int ProductId { get; set; }
    public string? ProductName { get; set; }
    public string? ImageURL { get; set; } 
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}
