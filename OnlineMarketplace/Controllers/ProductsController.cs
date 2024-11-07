using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineMarketplace.Data.Services;
using OnlineMarketplace.Models;

namespace OnlineMarketplace.Controllers;

[Authorize]
public class ProductsController : Controller
{
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;

    public ProductsController(IProductService service, ICategoryService categoryService)
    {
        _productService = service;
        _categoryService = categoryService;
    }

    [AllowAnonymous]
    public async Task<IActionResult> Index(int? categoryId)
    {
        var categories = await _categoryService.GetAllAsync();
        ViewBag.Categories = new SelectList(categories, "Id", "Name");

        var products = categoryId == null ? await _productService.GetAllAsync() 
            : await _productService.GetAllByFilterAsync(p => p.CategoryId == categoryId, p => p.Category);

        return View(products);
    }


    [AllowAnonymous]
    public async Task<IActionResult> Details(int id)
    {
        var product = await _productService.GetByIdAsync(id, p => p.Category, p => p.Reviews);

        if (product == null) return View("NotFound");
        return View(product);
    }

    public async Task<ActionResult> Create()
    {
        ViewData["CategoryList"] = new SelectList(await _categoryService.GetAllAsync(), "Id", "Name");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Product product)
    {
        if (ModelState.IsValid)
        {
            await _productService.AddAsync(product);
            await _productService.SaveAsync();
            return RedirectToAction(nameof(Index));
        }

        ViewData["CategoryList"] = new SelectList(await _categoryService.GetAllAsync(), "Id", "Name");
        return View(product);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var product = await _productService.GetByIdAsync(id, p => p.Category);
        if (product == null) return View("NotFound");

        ViewData["CategoryList"] = new SelectList(await _categoryService.GetAllAsync(), "Id", "Name", product.CategoryId);
        return View(product);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Product product)
    {
        if (ModelState.IsValid)
        {
            try
            {
                await _productService.UpdateAsync(id, product);
                await _productService.SaveAsync();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View("Error");
            }
        }

        ViewData["CategoryList"] = new SelectList(await _categoryService.GetAllAsync(), "Id", "Name", product.CategoryId);
        return View(product);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var product = await _productService.GetByIdAsync(id, p => p.Category, p => p.Reviews);
        if (product == null) return View("NotFound");

        return View(product);
    }

    [HttpPost, ActionName(nameof(Delete))]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var product = await _productService.GetByIdAsync(id, p => p.Category);
        if (product == null) return View("NotFound");
        try
        {
            await _productService.DeleteAsync(id);
            await _productService.SaveAsync();
        }
        catch
        {
            return View("Error");
        }

        return RedirectToAction(nameof(Index));
    }
}
