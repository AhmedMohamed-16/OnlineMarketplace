using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineMarketplace.Data;
using OnlineMarketplace.Data.Services;
using OnlineMarketplace.Models;
using OnlineMarketplace.Models.ViewModels;

namespace OnlineMarketplace.Controllers;

[Authorize]
public class OrdersController : Controller
{
    private readonly IOrderService _orderService;
    private readonly IProductService _productService;
    private readonly IPaymentService _paymentService;

    public OrdersController(IOrderService orderService, IProductService productService, IPaymentService paymentService)
    {
        _orderService = orderService;
        _productService = productService;
        _paymentService = paymentService;
    }

    public async Task<IActionResult> Index()
    {
        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        string? userRole = User.FindFirstValue(ClaimTypes.Role);

        var orders = await _orderService.GetOrdersByUserIdAndRoleAsync(userId, userRole);
        return View(orders);
    }

    public async Task<IActionResult> Details(int id)
    {
        var order = await _orderService.GetOrderIncluded(id);
        if (order == null)
            return View("NotFound");

        return View(order);
    }

    [Authorize(Roles = UserRoles.Admin)]
    public async Task<IActionResult> Create(int productId)
    {
        var product = await _productService.GetByIdAsync(productId);
        if (product == null) return View("Error");

        var paymentMethods = Enum.GetValues(typeof(PaymentMethod))
                            .Cast<PaymentMethod>()
                            .ToList();
        ViewData["PaymentMethodList"] = new SelectList(paymentMethods);
        ViewData["QuantityList"] = new SelectList(Enumerable.Range(1, product.UnitsInStock), 1);
        return View();
    }

    [HttpPost]
    [Authorize(Roles = UserRoles.Admin)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(int productId, OrderViewModel orderVM)
    {
        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var product = await _productService.GetByIdAsync(productId);

        if (userId == null || product == null) return View("Error");

        if (ModelState.IsValid)
        {
            var orderAddress = new OrderAddress()
            {
                City = orderVM.City,
                District = orderVM.District,
                Street = orderVM.Street,
                ZibCode = orderVM.ZipCode
            };
            var order = new Order()
            {
                UserId = userId,
                OrderPlaced = DateTime.Now,
                Status = OrderStatus.Pending,
                Address = orderAddress
            };

            await _orderService.AddAsync(order);
            product.UnitsInStock -= orderVM.Quantity;
            await _orderService.SaveAsync();

            var orderItem = new OrderDetail()
            {
                ProductId = productId,
                OrderId = order.Id,
                Quantity = orderVM.Quantity,
                Price = product.Price * orderVM.Quantity
            };

            var PaymentVM = new PaymentViewModel
            {
                OrderId = order.Id,
                Amount = orderItem.Price,
                Method = orderVM.PaymentMethod
            };

            await _orderService.AddOrderDetailsAsync(orderItem);
            await _orderService.SaveAsync();
            bool isSuccess = await _paymentService.ProcessPaymentAsync(userId, order, PaymentVM);
            if (!isSuccess)
                TempData["PaymentError"] = "The payment proccess failed!";

            return View("OrderCompleted");
        }

        var paymentMethods = Enum.GetValues(typeof(PaymentMethod))
                            .Cast<PaymentMethod>()
                            .ToList();
        ViewData["PaymentMethodList"] = new SelectList(paymentMethods);
        ViewData["QuantityList"] = new SelectList(Enumerable.Range(1, product.UnitsInStock), 1);
        return View(orderVM);
    }

    [Authorize(Roles = UserRoles.Admin)]
    public async Task<IActionResult> Edit(int id)
    {
        var order = await _orderService.GetByIdAsync(id);
        if (order == null)
            return View("NotFound");

        var orderStatuses = Enum.GetValues(typeof(OrderStatus))
                            .Cast<OrderStatus>()
                            .ToList();
        ViewData["StatusesList"] = new SelectList(orderStatuses, order.Status.ToString());
        return View(new OrderEditVM() { OrderId = id });
    }

    [HttpPost]
    [Authorize(Roles = UserRoles.Admin)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, OrderEditVM orderVM)
    {
        var order = await _orderService.GetByIdAsync(id);
        if (order  == null || order.Id != orderVM.OrderId)
            return View("Error");

        if (ModelState.IsValid)
        {
            order.OrderFulfilled = orderVM.OrderFulfilled;
            order.Status = orderVM.Status;
            await _orderService.SaveAsync();
            return RedirectToAction(nameof(Details), routeValues: new { id });
        }

        var orderStatuses = Enum.GetValues(typeof(OrderStatus))
                            .Cast<OrderStatus>()
                            .ToList();
        ViewData["StatusesList"] = new SelectList(orderStatuses, order.Status.ToString());
        return View(order);
    }
	
    public async Task<IActionResult> Cancel(int id)
    {
        // var order = await _orderService.GetByIdAsync(id, o => o.OrderDetails);
		var order = await _orderService.GetOrderIncluded(id);
		if (order == null)
            return View("NotFound");

        if (order.Status != OrderStatus.Pending)
            TempData["Error"] = "The order cannot be canceled because it has already been processed or canceled, or it's on its way.";

        return View(order);
    }
	
    [HttpPost, ActionName("Cancel")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CancelConfirmed(int id)
    {
        var order = await _orderService.GetOrderIncluded(id);
        if (order == null)
			return View("NotFound");
		if (order.Status != OrderStatus.Pending)
			return RedirectToAction(nameof(Details), new { id });
		
		order.Status = OrderStatus.Canceled;
        try
		{
            order.Payment.Status = PaymentStatus.Refunded;
            foreach (var item in order.OrderDetails)
                item.Product.UnitsInStock += item.Quantity;
			await _orderService.SaveAsync();
		}
		catch (Exception)
		{
			return View("Error");
		}
        return RedirectToAction(nameof(Index));
    }
}
