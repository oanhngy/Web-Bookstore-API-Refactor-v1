using BookstoreWeb.Application.DTOs.Common;
using BookstoreWeb.Application.DTOs.Products;
namespace BookstoreWeb.Application.Interfaces;

public interface IProductService
{
    //return paginated, filtered, sorted list of products
    Task<PagedResponse<ProductResponse>> GetAllAsync(string? searchString, int? categoryId, string? sortOrder, int page, int pageSize);

    //return a single product. throw NotFoundException if not found
    Task<ProductResponse> GetByIdAsync(int id);

    //create a new product + return
    Task<ProductResponse> AddAsync(CreateProductRequest request);

    //update a product, throw NotFoundException
    Task<ProductResponse> UpdateAsync(int id, UpdateProductRequest request);

    //delete, throw NotFoundException
    Task DeleteAsync(int id);
}