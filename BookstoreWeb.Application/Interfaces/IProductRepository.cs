using BookstoreWeb.Application.Domain;
namespace BookstoreWeb.Application.Interfaces;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllAsync();
    Task<int> GetCountAsync(string? searchString, int? categoryId); //tách riêng Search và GetCount vì GettAll trả all --> k hiệu quả khi có filter+pagination, service cần 2 query riêng: lấy data, đếm total
    Task<IEnumerable<Product>> SearchAsync(string? searchString, int? categoryId, string? sortOrder, int page, int pageSize);
    Task<Product?> GetByIdAsync(int id);
    Task<Product> AddAsync(Product product);
    Task<Product> UpdateAsync(Product product);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}