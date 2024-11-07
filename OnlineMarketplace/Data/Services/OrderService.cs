using Microsoft.EntityFrameworkCore;
using OnlineMarketplace.Data.Repository;
using OnlineMarketplace.Models;

namespace OnlineMarketplace.Data.Services;

public class OrderService : EntityBaseRepository<Order>, IOrderService
{
    private readonly ShopContext _context;
    private readonly IProductService _productService;

    public OrderService(ShopContext context, IProductService productService) : base(context)
    {
        _context = context;
        _productService = productService;
    }

    public async Task<List<Order>> GetOrdersByUserIdAndRoleAsync(string userId, string userRole)
    {
        if (userRole != UserRoles.Admin)
            return await _context.Orders.Include(o => o.User).Where(n => n.UserId == userId).ToListAsync();

        return await _context.Orders.Include(o => o.User).ToListAsync();
    }

    public async Task AddOrderDetailsAsync(List<OrderDetail> items) => await _context.OrderDetails.AddRangeAsync(items);
    public async Task AddOrderDetailsAsync(OrderDetail item) => await _context.OrderDetails.AddAsync(item);

    public async Task<Order?> GetOrderIncluded(int id)
    {
        var order = await _context.Orders.Include(o => o.User)
                                         .Include(o => o.Payment)
                                         .Include(o => o.OrderDetails)
                                         .ThenInclude(o => o.Product)
                                         .FirstOrDefaultAsync(i => i.Id == id);
        return order;
    }

    public async Task<int> CreatePendingOrder(string userId, CartViewModel cart, OrderAddress orderAddress)
    {
        var order = new Order
        {
            UserId = userId,
            OrderPlaced = DateTime.Now,
            Status = OrderStatus.Pending,
            Address = orderAddress
        };
        await AddAsync(order);
        await SaveAsync();

        foreach (var item in cart.Items)
        {
            var orderDetail = new OrderDetail
            {
                ProductId = item.ProductId,
                Price = item.Price,
                Quantity = item.Quantity,
                OrderId = order.Id
            };
            
            await AddOrderDetailsAsync(orderDetail);
            var prd = await _productService.GetByIdAsync(item.ProductId);
            if (prd != null)
                prd.UnitsInStock -= item.Quantity;
        }
        await SaveAsync();

        return order.Id;
    }
}