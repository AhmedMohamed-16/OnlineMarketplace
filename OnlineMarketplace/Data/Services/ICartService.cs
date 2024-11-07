using OnlineMarketplace.Models;

public interface ICartService
{
    Task<CartViewModel> GetCartAsync(HttpContext httpContext);
    Task AddToCartAsync(int productId, HttpContext httpContext);
    Task RemoveFromCartAsync(int productId, HttpContext httpContext);
    Task UpdateQuantityAsync(int productId, int quantity, HttpContext httpContext);
    void ClearCart(HttpContext httpContext);
}