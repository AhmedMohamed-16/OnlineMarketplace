// File: Data/Services/IReviewService.cs
using OnlineMarketplace.Models;

namespace OnlineMarketplace.Data.Services;

public interface IReviewService
{
    Task<bool> AddReviewAsync(System.Security.Claims.ClaimsPrincipal user, ReviewViewModel model);
    Task<IEnumerable<ReviewViewModel>> GetReviewsByProductIdAsync(int id);
    Task<Review?> GetReviewByIdAsync(int id);
    Task<bool> UpdateReviewAsync(System.Security.Claims.ClaimsPrincipal user, ReviewViewModel model);
    Task<bool> DeleteReviewAsync(int id);
}
