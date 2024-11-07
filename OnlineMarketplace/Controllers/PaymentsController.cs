using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineMarketplace.Data;
using OnlineMarketplace.Data.Services;
using OnlineMarketplace.Models;
using OnlineMarketplace.Models.ViewModels;
using System.Security.Claims;

namespace OnlineMarketplace.Controllers;

[Authorize]
public class PaymentsController : Controller
{
    private readonly IOrderService _orderService;
    private readonly IPaymentService _paymentService;

    public PaymentsController(IOrderService orderService, IPaymentService paymentService)
    {
        _orderService = orderService;
        _paymentService = paymentService;
    }

    [HttpPost]
    public async Task<IActionResult> ProcessPayment(int orderId, PaymentViewModel payment)
    {
        var order = await _orderService.GetByIdAsync(orderId);
        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (order == null || userId == null) return View("NotFound");


        // Process the payment (this could be handled via a payment gateway service)
        var paymentResult = await _paymentService.ProcessPaymentAsync(userId, order, payment);
        if (paymentResult)
            return View("ProcessSuccess");

        return View("Error");
    }

    [HttpPost]
    [Authorize(Roles = UserRoles.Admin)] // Only admins can issue refunds
    public async Task<IActionResult> RefundPayment(int paymentId)
    {
        var payment = await _paymentService.GetByIdAsync(paymentId);
        if (payment == null) return View("NotFound");

        // Process the refund (this might use a payment gateway's refund API)
        var refundResult = await _paymentService.RefundAsync(payment);

        if (refundResult)
            return View("RefundSuccess");

        return View("Error");
    }

    [HttpGet]
    public async Task<IActionResult> PaymentHistory(string userId)
    {
        var payments = await _paymentService.GetAllByFilterAsync(p => p.ApplicationUserId == userId, p => p.ApplicationUser, p => p.Order);

        return View(payments);
    }

    [HttpGet]
    public async Task<IActionResult> GetPaymentStatus(int paymentId)
    {
        var payment = await _paymentService.GetByIdAsync(paymentId);
        if (payment == null) return View("NotFound");

        return Json(new { status = payment.Status });
    }
}
