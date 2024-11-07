using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OnlineMarketplace.Data;
using OnlineMarketplace.Data.Repository;
using OnlineMarketplace.Data.Services;
using OnlineMarketplace.Models;

namespace OnlineMarketplace;

public class Program
{
    public async static Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();
        
        // Cookies, Session, Cache
        builder.Services.AddDistributedMemoryCache(); // Adds a default in-memory implementation of IDistributedCache
        builder.Services.AddSession(options => {
            options.IdleTimeout = TimeSpan.FromMinutes(30);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true; // For GDPR compliance
        });

        builder.Services.AddDbContext<ShopContext>(options =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
        });

        builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireDigit = false;
        }).AddEntityFrameworkStores<ShopContext>();
        
        builder.Services.AddScoped<IProductService, ProductService>();
        builder.Services.AddScoped<ICustomerService, CustomerService>();
        builder.Services.AddScoped<ICategoryService, CategoryService>();
        builder.Services.AddScoped<IOrderService, OrderService>();
        builder.Services.AddScoped<ICartService, CartService>();
        builder.Services.AddScoped<IReviewService, ReviewService>();
        builder.Services.AddScoped<IPaymentService, PaymentService>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
        }
        app.UseStaticFiles();

        app.UseSession();

        app.UseRouting();

        app.UseAuthentication();

        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        #region Seed Database
        /// Seed the database
        //using (var scope = app.Services.CreateScope())
        //{
        //    var context = scope.ServiceProvider.GetRequiredService<ShopContext>();
        //    Seed(context); // Call seed method
        //}
        #endregion
        #region Ensure Roles Exist
        //var ser = app.Services.CreateScope().ServiceProvider;
        //var roleManager = ser.GetRequiredService<RoleManager<IdentityRole>>();
        //await EnsureRolesExist(roleManager);
        #endregion

        app.Run();
    }

    private static async Task EnsureRolesExist(RoleManager<IdentityRole> roleManager)
    {
        // Ensure the User role exists
        if (!await roleManager.RoleExistsAsync(UserRoles.User))
        {
            await roleManager.CreateAsync(new IdentityRole(UserRoles.User));
        }

        // Ensure the Admin role exists (if you need this role)
        if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
        {
            await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
        }

        if (!await roleManager.RoleExistsAsync(UserRoles.Guest))
        {
            await roleManager.CreateAsync(new IdentityRole(UserRoles.Guest));
        }
    }

    //private static void Seed(ShopContext context)
    //{
    //    if (!context.Categories.Any()) // Ensure the data is only added once
    //    {
    //        var categories = new List<Category>
    //        {
    //            new Category
    //            {
    //                Name = "Electronics",
    //                Description = "Devices and gadgets that operate on electrical energy."
    //            },
    //            new Category
    //            {
    //                Name = "Books",
    //                Description = "Printed or electronic works consisting of written words."
    //            },
    //            new Category
    //            {
    //                Name = "Clothing",
    //                Description = "Wearable items, including shirts, pants, dresses, and accessories."
    //            },
    //            new Category
    //            {
    //                Name = "Home & Kitchen",
    //                Description = "Products for household use, including kitchen appliances and furniture."
    //            }
    //        };
    //        context.AddRange(categories);
    //        context.SaveChanges();

    //        var products = new List<Product>
    //        {
    //            new Product
    //            {
    //                Name = "Smartphone",
    //                Description = "A high-end smartphone with a powerful processor and great camera.",
    //                UnitsInStock = 50,
    //                Price = 699.99m,
    //                ImageURL = "http://example.com/images/smartphone.jpg",
    //                CategoryId = 1,
    //                Category = categories.First(c => c.Id == 1)
    //            },
    //            new Product
    //            {
    //                Name = "Laptop",
    //                Description = "A lightweight laptop suitable for both work and play.",
    //                UnitsInStock = 30,
    //                Price = 999.99m,
    //                ImageURL = "http://example.com/images/laptop.jpg",
    //                CategoryId = 1,
    //                Category = categories.First(c => c.Id == 1)
    //            },
    //            new Product
    //            {
    //                Name = "Fiction Novel",
    //                Description = "An exciting fiction novel that keeps you on the edge of your seat.",
    //                UnitsInStock = 100,
    //                Price = 19.99m,
    //                ImageURL = "http://example.com/images/fiction_novel.jpg",
    //                CategoryId = 2,
    //                Category = categories.First(c => c.Id == 2)
    //            },
    //            new Product
    //            {
    //                Name = "Winter Jacket",
    //                Description = "A warm and stylish jacket perfect for cold weather.",
    //                UnitsInStock = 20,
    //                Price = 89.99m,
    //                ImageURL = "http://example.com/images/winter_jacket.jpg",
    //                CategoryId = 3,
    //                Category = categories.First(c => c.Id == 3)
    //            },
    //            new Product
    //            {
    //                Name = "Cookware Set",
    //                Description = "A complete set of cookware for all your cooking needs.",
    //                UnitsInStock = 15,
    //                Price = 149.99m,
    //                ImageURL = "http://example.com/images/cookware_set.jpg",
    //                CategoryId = 4,
    //                Category = categories.First(c => c.Id == 4)
    //            }
    //        };


    //        context.AddRange(products);
    //        context.SaveChanges();
    //    }
    //}
}
