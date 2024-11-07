using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OnlineMarketplace.Models;

namespace OnlineMarketplace.Data;

//public class ShopContext : DbContext
public class ShopContext : IdentityDbContext<ApplicationUser>
{
    public virtual DbSet<Customer> Customers { get; set; }
    public virtual DbSet<Cart> Carts { get; set; }
    public virtual DbSet<CartItem> CartItems { get; set; }
    public virtual DbSet<Category> Categories { get; set; }
    public virtual DbSet<Order> Orders { get; set; }
    public virtual DbSet<OrderDetail> OrderDetails { get; set; }
    public virtual DbSet<Payment> Payments { get; set; }
    public virtual DbSet<Product> Products { get; set; }
    public virtual DbSet<Review> Reviews { get; set; }

    public ShopContext(DbContextOptions<ShopContext> options) : base(options) {}
    
    //public ShopContext() : base() {}

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //=> optionsBuilder.UseSqlServer("Data Source=DESKTOP-TQDL9EA\\SQLEXPRESS;Initial Catalog=OnlineMarketplace;Integrated Security=true;Encrypt=False;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>().OwnsOne(o => o.Address);

        modelBuilder.Entity<Payment>()
        .HasOne(p => p.ApplicationUser)
        .WithMany()
        .HasForeignKey(p => p.ApplicationUserId)
        .OnDelete(DeleteBehavior.NoAction);

        base.OnModelCreating(modelBuilder);
    }

}