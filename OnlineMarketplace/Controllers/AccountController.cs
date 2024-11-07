using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineMarketplace.Data;
using OnlineMarketplace.Data.Services;
using OnlineMarketplace.Models;

namespace OnlineMarketplace.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ICustomerService _customerService;

    public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ICustomerService customerService, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _customerService = customerService;
        _roleManager = roleManager;
    }

    [Authorize]
    public async Task<IActionResult> Users()
    {
        var users = await _userManager.Users.ToListAsync();
        return View(users);
    }

    public IActionResult Login() => View(new LoginVM());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginVM loginVM)
    {
        if (!ModelState.IsValid) return View(loginVM);

        var user = await _userManager.FindByEmailAsync(loginVM.EmailAddress);
        if (user != null)
        {
            var passwordCheck = await _userManager.CheckPasswordAsync(user, loginVM.Password);
            if (passwordCheck)
            {
                await _signInManager.SignInAsync(user, loginVM.RememberMe);
                return RedirectToAction("Index", "Home");
            }
            TempData["Error"] = "Wrong credentials. Please, try again!";
            return View(loginVM);
        }

        TempData["Error"] = "Wrong credentials. Please, try again!";
        return View(loginVM);
    }

    public IActionResult Register() => View(new RegisterVM());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterVM registerVM)
    {
        if (!ModelState.IsValid) return View(registerVM);

        var user = await _userManager.FindByEmailAsync(registerVM.EmailAddress);
        if (user != null)
        {
            TempData["Error"] = "This email address is already in use";
            return View(registerVM);
        }

        var newUser = new ApplicationUser()
        {
            FirstName = registerVM.FirstName,
            LastName = registerVM.LastName,
            UserName = registerVM.EmailAddress,
            City = registerVM.City,
            Email = registerVM.EmailAddress,
            PhoneNumber = registerVM.PhoneNumber,
        };

        var response = await _userManager.CreateAsync(newUser, registerVM.Password);

        if (response.Succeeded)
        {
            if (await _roleManager.RoleExistsAsync(UserRoles.User))
                await _userManager.AddToRoleAsync(newUser, UserRoles.User);

            var customer = new Customer
            {
                FullName = registerVM.FirstName + " " + registerVM.LastName,
                DateJoined = DateTime.Now,
                ApplicationUserId = newUser.Id
            };
            await _customerService.AddAsync(customer);
            await _customerService.SaveAsync();
            return View("RegisterCompleted");
        }
        
        return View(registerVM);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction(nameof(Login));
    }

    public IActionResult AccessDenied(string returnUrl) => View();
}
