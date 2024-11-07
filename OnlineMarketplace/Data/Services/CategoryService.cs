// File: Data/Services/CategoryService.cs
using Microsoft.EntityFrameworkCore;
using OnlineMarketplace.Data;
using OnlineMarketplace.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineMarketplace.Data.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ShopContext _context;

        public CategoryService(ShopContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<Category> GetByIdAsync(int id)
        {
            return await _context.Categories.FindAsync(id);
        }

        public async Task<bool> AddAsync(Category category)
        {
            _context.Categories.Add(category);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> UpdateAsync(int id, Category category)
        {
            var existingCategory = await _context.Categories.FindAsync(id);
            if (existingCategory == null)
                return false;

            existingCategory.Name = category.Name;
            // Update other properties as needed

            _context.Categories.Update(existingCategory);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return false;

            _context.Categories.Remove(category);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }
    }
}
