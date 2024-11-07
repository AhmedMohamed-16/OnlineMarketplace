using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineMarketplace.Data;
using OnlineMarketplace.Data.Services;
using OnlineMarketplace.Models;

namespace OnlineMarketplace.Controllers;

[Authorize]
public class CustomersController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICustomerService _customerService;

    public CustomersController(ICustomerService customerService, UserManager<ApplicationUser> userManager)
    {
        _customerService = customerService;
        _userManager = userManager;
    }

    [Authorize(Roles = UserRoles.Admin)]
    public async Task<IActionResult> Index()
    {
        var customers = await _customerService.GetAllAsync();
        return View(customers);
    }

    [Authorize(Roles = UserRoles.Admin)]
    public async Task<IActionResult> Edit(int id)
    {
        var customer = await _customerService.GetByIdAsync(id);
        if (customer == null)
            return View("NotFound");

        var customerVM = new CustomerVM
        {
            Id = id,
            Address = customer.Address,
            Age = customer.Age,
            FullName = customer.FullName,
        };

        return View(customerVM);
    }

    [HttpPost]
    [Authorize(Roles = UserRoles.Admin)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,FullName,Address,Age")] CustomerVM customerVM)
    {
        if (ModelState.IsValid)
        {
            var customer = await _customerService.GetByIdAsync(id);
            if (customer == null) return View("NotFound");

            customer.FullName = customerVM.FullName;
            customer.Address = customerVM.Address;
            customer.Age = customerVM.Age;

            try
            {
                await _customerService.UpdateAsync(id, customer);
                await _customerService.SaveAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return View("Error");
            }

            TempData["SuccessMessage"] = "Your profile has been updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        TempData["Error"] = "Unexpected error.";
        return View(customerVM);
    }

    [Authorize(Roles = UserRoles.User)]
    public async Task<IActionResult> Profile()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return View("NotFound");
        var userId = user.Id;
        var customer = await _customerService.GetByFilterAsync(filter: c => c.ApplicationUserId == userId);
        if (customer == null)
            return View("NotFound");
        
        var customerVM = new CustomerUserVM
        {
            FullName = customer.FullName,
            Address = customer.Address,
            Age = customer.Age,
            DateJoined = customer.DateJoined,
            PhoneNumber = user.PhoneNumber,
            City = user.City,
        };

        return View(customerVM);
    }
    
    [HttpPost]
    [Authorize(Roles = UserRoles.User)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Profile([Bind("FullName,Address,Age,City,PhoneNumber")] CustomerUserVM customerVM)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return View("NotFound");

        var userId = user.Id;
        var customer = await _customerService.GetByFilterAsync(filter: c => c.ApplicationUserId == userId);
        if (customer == null)
            return View("NotFound");

        if (ModelState.IsValid)
        {
            customer.FullName = customerVM.FullName;
            customer.Address = customerVM.Address;
            customer.Age = customerVM.Age;
            user.PhoneNumber = customerVM.PhoneNumber;
            user.City = customerVM.City;

            try
            {
                await _userManager.UpdateAsync(user);
                await _customerService.UpdateAsync(customer.Id, customer);
                await _customerService.SaveAsync();
            }
            catch (Exception)
            {
                return View("Error");
            }
            TempData["SuccessMessage"] = "Your profile has been updated successfully!";
            return RedirectToAction(nameof(Profile));
        }

        TempData["Error"] = "Unexpected error.";
        return RedirectToAction(nameof(Profile));
    }
    
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<IActionResult> Delete(int id)
    {
        var customer = await _customerService.GetByIdAsync(id, c => c.ApplicationUser);
        if (customer == null)
            return View("NotFound");

        return View(customer);
    }

    [HttpPost, ActionName("Delete")]
    [Authorize(Roles = UserRoles.Admin)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var customer = await _customerService.GetByIdAsync(id, c => c.ApplicationUser);
        if (customer != null)
        {
            await _customerService.DeleteAsync(id);
            await _customerService.SaveAsync();
            return RedirectToAction(nameof(Index));
        }

        return View("NotFound");
    }
}
