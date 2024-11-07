// File: Data/Services/ICategoryService.cs
using OnlineMarketplace.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineMarketplace.Data.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAllAsync();
        Task<Category> GetByIdAsync(int id);
        Task<bool> AddAsync(Category category);
        Task<bool> UpdateAsync(int id, Category category);
        Task<bool> DeleteAsync(int id);

    }
}
