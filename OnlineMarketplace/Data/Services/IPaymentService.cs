using OnlineMarketplace.Data.Repository;
using OnlineMarketplace.Models;
using OnlineMarketplace.Models.ViewModels;

namespace OnlineMarketplace.Data.Services;

public interface IPaymentService : IEntityBaseRepository<Payment>
{
    Task<bool> ProcessPaymentAsync(string? userId, Order order, PaymentViewModel paymentVM);
    Task<bool> RefundAsync(Payment payment);
}
