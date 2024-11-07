using Microsoft.AspNetCore.Identity;
using OnlineMarketplace.Data.Repository;
using OnlineMarketplace.Models;

namespace OnlineMarketplace.Data.Services;

public class CustomerService : EntityBaseRepository<Customer>, ICustomerService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public CustomerService(ShopContext context, UserManager<ApplicationUser> userManager) : base(context)
    {
        _userManager = userManager;
    }

    public async Task<IList<string>> GetCustomerRolesAsync(string userID)
    {
        // Find the customer by their ID
        var user = await _userManager.FindByIdAsync(userID);
        if (user == null)
            throw new Exception("Customer not found.");

        // Get the roles for this customer
        var roles = await _userManager.GetRolesAsync(user);

        return roles;
    }



}