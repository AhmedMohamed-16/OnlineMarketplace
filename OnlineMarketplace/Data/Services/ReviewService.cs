// File: Data/Services/ReviewService.cs
using OnlineMarketplace.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace OnlineMarketplace.Data.Services;

public class ReviewService : IReviewService
{
    private readonly ShopContext _context;

    public ReviewService(ShopContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ReviewViewModel>> GetReviewsByProductIdAsync(int productId)
    {
        var reviews = await _context.Reviews
            .Where(r => r.ProductId == productId)
            .Include(r => r.Product)
            .ToListAsync();

        return reviews.Select(r => new ReviewViewModel
        {
            Id = r.Id,
            ProductId = r.ProductId,
            ProductName = r.Product.Name,
            Rating = r.Rating,
            Comment = r.Comment,
            ApplicationUserId = r.ApplicationUserId
        });
    }

    public async Task<bool> AddReviewAsync(ClaimsPrincipal user, ReviewViewModel model)
    {
        var customerId = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(customerId))
            return false;

        var review = new Review
        {
            ProductId = model.ProductId,
            Rating = model.Rating,
            Comment = model.Comment,
            ApplicationUserId = model.ApplicationUserId,
            CreatedDate = DateTime.Now,
            UpdatedDate = DateTime.Now
        };

        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateReviewAsync(ClaimsPrincipal user, ReviewViewModel model)
    {
        var review = await _context.Reviews.FindAsync(model.Id);
        if (review == null)
            return false;

        if (review.ApplicationUserId != user.FindFirstValue(ClaimTypes.NameIdentifier))
            return false;

        review.Rating = model.Rating;
        review.Comment = model.Comment;

        _context.Reviews.Update(review);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteReviewAsync(int id)
    {
        var review = await _context.Reviews.FindAsync(id);
        if (review == null)
            return false;

        _context.Reviews.Remove(review);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Review?> GetReviewByIdAsync(int id) => await _context.Reviews.Include(r => r.ApplicationUser).Include(r => r.Product).FirstOrDefaultAsync(r => r.Id == id);
}
