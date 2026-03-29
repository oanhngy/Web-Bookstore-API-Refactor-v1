using BookstoreWeb.Application.Domain;
using BookstoreWeb.Application.Interfaces;
using BookstoreWeb.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
namespace BookstoreWeb.Infrastructure.Repositories;

//CRUD bảng Category
public class CategoryRepository:ICategoryRepository
{
    private readonly BookstoreContext _context;
    public CategoryRepository(BookstoreContext context)
    {
        _context=context;
    }

    //1-lấy all
    public async Task<IEnumerable<Category>> GetAllAsync()
    {
        return await _context.Categories.ToListAsync();
    }

    //2-lấy 1 theo id, null nếu k t hấy
    public async Task<Category?> GetByIdAsync(int id)
    {
        return await _context.Categories.FirstOrDefaultAsync(c=>c.CategoryID==id);
    }

    //3-add new
    public async Task<Category> AddAsync(Category category)
    {
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        return category;
    }

    //4-update
    public async Task<Category> UpdateAsync(Category category) 
    {
        _context.Categories.Update(category);
        await _context.SaveChangesAsync();
        return category;
    }

    //5-delete
    public async Task DeleteAsync(int id)
    {
        var category=await _context.Categories.FindAsync(id);
        if(category is not null)
        {
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }
    }

    //6-check exist
    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Categories.AnyAsync(c=>c.CategoryID==id);
    }
}