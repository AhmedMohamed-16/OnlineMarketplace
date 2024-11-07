// File: Controllers/ReviewController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineMarketplace.Data.Services;
using OnlineMarketplace.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace OnlineMarketplace.Controllers;

[Authorize]
public class ReviewController : Controller
{
    private readonly IReviewService _reviewService;
    private readonly IProductService _productService;
    private readonly UserManager<ApplicationUser> _userManager;

    public ReviewController(IReviewService reviewService, IProductService productService, UserManager<ApplicationUser> userManager)
    {
        _reviewService = reviewService;
        _productService = productService;
        _userManager = userManager;
    }

    // GET: /Review/Create/{productId}
    [HttpGet]
    public async Task<IActionResult> Create(int productId)
    {
        var product = await _productService.GetByIdAsync(productId);
        var userId = _userManager.GetUserId(User);
        if (product == null || userId == null)
            return View("NotFound");

        var review = new ReviewViewModel
        {
            ProductId = productId,
            ProductName = product.Name,
            ApplicationUserId = userId
        };

        return View(review);
    }

    // POST: /Review/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ReviewViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _reviewService.AddReviewAsync(User, model);
        if (result)
        {
            return RedirectToAction("Details", "Products", new { id = model.ProductId });
        }

        ModelState.AddModelError("", "Unable to add review. Please try again.");
        return View(model);
    }

    // GET: /Review/Edit/{id}
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var review = await _reviewService.GetReviewByIdAsync(id);
        if (review == null)
        {
            return NotFound();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (review.ApplicationUserId != userId)
        {
            return Forbid();
        }

        var model = new ReviewViewModel
        {
            Id = review.Id,
            ProductId = review.ProductId,
            Rating = review.Rating,
            Comment = review.Comment
        };

        return View(model);
    }

    // POST: /Review/Edit/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ReviewViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _reviewService.UpdateReviewAsync(User, model);
        if (result)
        {
            return RedirectToAction("Details", "Products", new { id = model.ProductId });
        }

        ModelState.AddModelError("", "Unable to update review. Please try again.");
        return View(model);
    }

    // GET: /Review/Delete/{id}
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var review = await _reviewService.GetReviewByIdAsync(id);
        if (review == null)
        {
            return NotFound();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!User.IsInRole("Admin") && review.ApplicationUserId != userId)
        {
            return Forbid();
        }

        var model = new ReviewViewModel
        {
            Id = review.Id,
            ProductId = review.ProductId,
            ProductName = review.Product.Name,
            Rating = review.Rating,
            Comment = review.Comment
        };

        return View(model);
    }

    // POST: /Review/Delete/{id}
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var review = await _reviewService.GetReviewByIdAsync(id);
        if (review == null)
        {
            return NotFound();
        }

        // Only admin or review owner can delete
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!User.IsInRole("Admin") && review.ApplicationUserId != userId)
        {
            return Forbid();
        }

        var result = await _reviewService.DeleteReviewAsync(id);
        if (result)
        {
            return RedirectToAction("Details", "Products", new { id = review.ProductId });
        }

        // Error handling: pass the existing review details back to the view
        var model = new ReviewViewModel
        {
            Id = review.Id,
            ProductId = review.ProductId,
            ProductName = review.Product.Name,
            Rating = review.Rating,
            Comment = review.Comment
        };

        ModelState.AddModelError("", "Unable to delete review. Please try again.");
        return View(model);
    }
}
