using BookstoreWeb.Application.DTOs.Categories;

namespace BookstoreWeb.Application.Interfaces;

public interface ICategoryService
{
    //1-Returns all categories
    Task<IEnumerable<CategoryResponse>> GetAllAsync();

    //2-Returns a single category by ID. Throws NotFoundException if not found
    Task<CategoryResponse> GetByIdAsync(int id);

    //3-Creates a new category. Returns the created category
    Task<CategoryResponse> AddAsync(CreateCategoryRequest request);

    //4-Updates an existing category by ID. Throws NotFoundException if not found
    Task<CategoryResponse> UpdateAsync(int id, UpdateCategoryRequest request);

    //5-Deletes a category by ID. Throws NotFoundException if not found
    Task DeleteAsync(int id);
}
