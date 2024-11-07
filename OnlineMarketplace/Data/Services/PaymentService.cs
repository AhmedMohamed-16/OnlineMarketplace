using Microsoft.EntityFrameworkCore;
using OnlineMarketplace.Data.Repository;
using OnlineMarketplace.Models;
using OnlineMarketplace.Models.ViewModels;
using System.Linq.Expressions;
using System.Linq;

namespace OnlineMarketplace.Data.Services;

public class PaymentService : EntityBaseRepository<Payment>, IPaymentService
{
    public PaymentService(ShopContext context) : base(context)
    {
    }

    public async Task<bool> ProcessPaymentAsync(string? userId, Order order, PaymentViewModel paymentVM)
    {
        if (order == null || paymentVM.OrderId != order.Id || userId == null)
            return false;

        // Implement the logic to process payment through a (e.g., Stripe, PayPal, credit card, etc.) service provider
        // Example: Calling an API to charge the customer
        bool isPaymentSuccessful = true; // Simulating a successful payment

        if (isPaymentSuccessful)
        {
            PaymentStatus p = PaymentStatus.Completed;
            DateTime? dateTime = DateTime.Now;
            if (paymentVM.Method == PaymentMethod.Cash) {
                dateTime = null;
                p = PaymentStatus.Pending;
            }
        
            var payment = new Payment
            {
                Amount = paymentVM.Amount,
                Method = paymentVM.Method,
                OrderId = order.Id,
                PaymentDate = dateTime,
                Status = p,
                ApplicationUserId = userId
            };

            await AddAsync(payment);
            await SaveAsync();
            return true;
        }

        return false;
    }

    public async Task<bool> RefundAsync(Payment payment)
    {
        if (!CanRefund(payment))
            return false;

        // Implement the logic to refund payment through a (e.g., Stripe, PayPal, credit card, etc.) service provider
        bool isPaymentRefundedSuccessful = true;

        if (isPaymentRefundedSuccessful)
        {
            payment.Status = PaymentStatus.Refunded;

            // Here implementation for returning the money to the user

            await UpdateAsync(payment.Id, payment);
            await SaveAsync();

            return true;
        }

        return false;
    }

    // Check if the payment status is "Completed" and if it's within a refund period (30 days refund policy)
    private bool CanRefund(Payment payment)
    {
        if (payment.Status == PaymentStatus.Completed ||
            (DateTime.Now - payment.PaymentDate)?.TotalDays > 30)
            return false;

        return true;
    }
}
