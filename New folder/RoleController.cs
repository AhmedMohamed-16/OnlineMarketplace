using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineMarketplace.Data;
using OnlineMarketplace.Models.ViewModel;

namespace OnlineMarketplace.Controllers;

//[Authorize(Roles = UserRoles.Admin)]
public class RoleController : Controller
{
    private readonly RoleManager<IdentityRole> roleManager;
    public RoleController(RoleManager<IdentityRole> roleManager)
    {
        this.roleManager = roleManager;
    }

    public IActionResult Index()
    {
        var res = roleManager.Roles.Select(r => r.Name).ToList();
        return View(res);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(RoleVM roleViewModel) {
        if(ModelState.IsValid)
        {
            var role = new IdentityRole(roleViewModel.RoleName);
            var result = await roleManager.CreateAsync(role);
            if (result.Succeeded == true)
                return RedirectToAction(nameof(Index));
        }

        return View(roleViewModel);
    }

}
