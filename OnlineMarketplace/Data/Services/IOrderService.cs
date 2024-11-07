using OnlineMarketplace.Data.Repository;
using OnlineMarketplace.Models;

namespace OnlineMarketplace.Data.Services
{
    public interface IOrderService : IEntityBaseRepository<Order>
    {
        Task<List<Order>> GetOrdersByUserIdAndRoleAsync(string userId, string userRole);
        Task<Order?> GetOrderIncluded(int id); 
        Task AddOrderDetailsAsync(List<OrderDetail> items);
        Task AddOrderDetailsAsync(OrderDetail item);
        Task<int> CreatePendingOrder(string userId, CartViewModel cart, OrderAddress orderAddress);
    }
}
