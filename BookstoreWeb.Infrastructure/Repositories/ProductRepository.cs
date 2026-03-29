using BookstoreWeb.Application.Domain;
using BookstoreWeb.Application.Interfaces;
using BookstoreWeb.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
namespace BookstoreWeb.Infrastructure.Repositories;

//CRUD +search bảng Products
public class ProductRepository:IProductRepository
{
    private readonly BookstoreContext _context;
    public ProductRepository(BookstoreContext context)
    {
        _context=context;
    }

    //1-lấy all product kèm Category + hình
    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _context.Products
            .Include(p=>p.Category)
            .Include(p=>p.ProductImages)
            .ToListAsync();
    }

    //2-get count, đếm tổng sp khớp filter, service để tính TotalPages
    public async Task<int> GetCountAsync(string? searchString, int? categoryId)
    {
        var query=_context.Products.AsQueryable();
        if(!string.IsNullOrEmpty(searchString)) query=query.Where(p=>p.Name.Contains(searchString));
        if(categoryId.HasValue) query=query.Where(p=>p.CategoryID==categoryId.Value);
        return await query.CountAsync();
    }

    //3-get spham có filter, sort, pagination + only load hình primary (tiết kiệm)
    public async Task<IEnumerable<Product>> SearchAsync(string? searchString, int? categoryId, string? sortOrder, int page, int pageSize)
    {
        var query=_context.Products
            .Include(p=>p.Category)
            .Include(p=>p.ProductImages.Where(img=>img.IsPrimary))
            .AsQueryable();
        
        //filter theo category
        if(categoryId.HasValue) query=query.Where(p=>p.CategoryID==categoryId.Value);

        //filter theo keyword trong sp
        if(!string.IsNullOrEmpty(searchString)) query=query.Where(p=>p.Name.Contains(searchString));

        //sort, default= theo tên nếu k truyền sortOrder
        query=sortOrder switch
        {
            "price_asc"=>query.OrderBy(p=>p.Price),
            "price_desc"=>query.OrderByDescending(p=>p.Price),
            _=>query.OrderBy(p=>p.Name)
        };

        //pagination
        return await query.Skip((page-1)*pageSize).Take(pageSize).ToListAsync();
    }

    //4-get 1 product kèm Category +all hình
    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _context.Products
            .Include(p=>p.Category)
            .Include(p=>p.ProductImages)
            .FirstOrDefaultAsync(p=>p.ProductID==id);
    }

    //5-add sp vô db
    public async Task<Product> AddAsync(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return product;
    }

    //6-update
    public async Task<Product> UpdateAsync(Product product)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
        return product;
    }

    //delete
    public async Task DeleteAsync(int id)
    {
        var product=await _context.Products.FindAsync(id);
        if(product is not null)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
    }

    //check exist
    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Products.AnyAsync(p=>p.ProductID==id);
    }
}