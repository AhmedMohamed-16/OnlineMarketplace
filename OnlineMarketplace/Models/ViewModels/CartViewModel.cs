// File: Models/CartViewModel.cs
namespace OnlineMarketplace.Models;

public class CartViewModel
{
    public List<CartItemViewModel> Items { get; set; } = new List<CartItemViewModel>();

    // Read-only Total property
    public decimal Total => Items?.Sum(i => i.Price * i.Quantity) ?? 0;
}
