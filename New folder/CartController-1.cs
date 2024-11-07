using Microsoft.AspNetCore.Mvc;
using OnlineMarketplace.Models.ViewModels;
using OnlineMarketplace.Models;
using System.Security.Claims;
using OnlineMarketplace.Data.Services;

public class CartController : Controller
{
    private readonly ICartService _cartService;
    private readonly IOrderService _orderService;
    private readonly IPaymentService _paymentService;

    public CartController(ICartService cartService, IOrderService orderService, IPaymentService paymentService)
    {
        _cartService = cartService;
        _orderService = orderService;
        _paymentService = paymentService;
    }
    public async Task<IActionResult> Index()
    {
        var cart = await _cartService.GetCartAsync(HttpContext);
        return View(cart);
    }

    [HttpPost]
    public async Task<IActionResult> AddToCart(int productId)
    {
        await _cartService.AddToCartAsync(productId, HttpContext);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> RemoveFromCart(int productId)
    {
        await _cartService.RemoveFromCartAsync(productId, HttpContext);
        return RedirectToAction(nameof(Index));
    }
    [HttpPost]
    public async Task<IActionResult> UpdateQuantity(int productId, int quantity)
    {
        await _cartService.UpdateQuantityAsync(productId, quantity, HttpContext);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> ClearCart()
    {
        await _cartService.ClearCartAsync(HttpContext);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet, HttpPost]
    public IActionResult Checkout()
    {
        // Assuming you handle the redirection to the checkout process here
        return RedirectToAction("PlaceOrder", "Order");
    }
    [HttpPost]
    public async Task<IActionResult> Checkout(OrderVM orderVM)
    {
        var cart = await _cartService.GetCartAsync(HttpContext);
        if (cart == null || !cart.Items.Any())
        {
            TempData["Error"] = "Your cart is empty!";
            return RedirectToAction(nameof(Index));
        }

        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            TempData["Error"] = "Please log in to complete the purchase.";
            return RedirectToAction("Login", "Account");
        }

        // Create the order
        var orderAddress = new OrderAddress
        {
            City = orderVM.City,
            District = orderVM.District,
            Street = orderVM.Street,
            ZibCode = orderVM.ZipCode
        };

        var order = new Order
        {
            UserId = userId,
            OrderPlaced = DateTime.Now,
            Status = OrderStatus.Pending,
            Address = orderAddress
        };

        // Save the order and its details
        await _orderService.AddAsync(order);
        var orderDetails = cart.Items.Select(item => new OrderDetail
        {
            ProductId = item.ProductId,
            OrderId = order.Id,
            Quantity = item.Quantity,
            Price = item.Price * item.Quantity
        }).ToList();

        await _orderService.AddOrderDetailsAsync(orderDetails);
        await _orderService.SaveAsync();

        // Prepare for payment
        var paymentVM = new PaymentViewModel
        {
            OrderId = order.Id,
            Amount = orderDetails.Sum(od => od.Price),
            Method = orderVM.PaymentMethod
        };

        bool isSuccess = await _paymentService.ProcessPaymentAsync(userId, order, paymentVM);
        if (!isSuccess)
        {
            TempData["PaymentError"] = "Payment failed!";
            return RedirectToAction(nameof(Index));
        }

        // Clear the cart after successful checkout
        await _cartService.ClearCartAsync(HttpContext);

        // Redirect to order confirmation
        return RedirectToAction("Details", "Orders", new { id = order.Id });
    }
    // Additional methods for UpdateQuantity and ClearCart
}
