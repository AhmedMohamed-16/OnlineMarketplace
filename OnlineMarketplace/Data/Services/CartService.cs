using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using OnlineMarketplace.Data;
using OnlineMarketplace.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace OnlineMarketplace.Data.Services;

public class CartService : ICartService
{
    private readonly ShopContext _context;

    public CartService(ShopContext context)
    {
        _context = context;
    }

    public async Task<CartViewModel> GetCartAsync(HttpContext httpContext)
    {
        var cartJson = httpContext.Session.GetString("Cart");
        var cart = string.IsNullOrEmpty(cartJson) ? new CartViewModel() : JsonConvert.DeserializeObject<CartViewModel>(cartJson);

        // Optionally load additional product details here if needed
        if (cart.Items != null && cart.Items.Any())
        {
            var productIds = cart.Items.Select(item => item.ProductId).ToList();
            var products = await _context.Products.Where(p => productIds.Contains(p.Id)).ToListAsync();
            foreach (var item in cart.Items)
            {
                var product = products.FirstOrDefault(p => p.Id == item.ProductId);
                if (product != null)
                {
                    item.ProductName = product.Name;
                    item.Price = product.Price;
                    item.ImageURL = product.ImageURL;
                }
            }
        }

        return cart;
    }

    public async Task AddToCartAsync(int productId, HttpContext httpContext)
    {
        var cart = await GetCartAsync(httpContext);
        var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);

        if (existingItem != null)
        {
            existingItem.Quantity++;
        }
        else
        {
            cart.Items.Add(new CartItemViewModel { ProductId = productId, Quantity = 1 });
        }

        httpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cart));
    }

    public async Task RemoveFromCartAsync(int productId, HttpContext httpContext)
    {
        var cart = await GetCartAsync(httpContext);
        var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);

        if (item != null)
        {
            cart.Items.Remove(item);
        }

        httpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cart));
    }

    public async Task UpdateQuantityAsync(int productId, int quantity, HttpContext httpContext)
    {
        var cart = await GetCartAsync(httpContext);
        var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);

        if (item != null)
        {
            item.Quantity = quantity > 0 ? quantity : 1; // Ensures quantity is at least 1
        }

        httpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cart));
    }

    public void ClearCart(HttpContext httpContext) => httpContext.Session.Remove("Cart");
}
