using BookstoreWeb.Application.DTOs.Categories;

namespace BookstoreWeb.Application.Interfaces;

public interface ICategoryService
{
    //Returns all categories
    Task<IEnumerable<CategoryResponse>> GetAllAsync();

    //Returns a single category by ID. Throws NotFoundException if not found
    Task<CategoryResponse> GetByIdAsync(int id);

    //Creates a new category. Returns the created category
    Task<CategoryResponse> AddAsync(CreateCategoryRequest request);

    //Updates an existing category by ID. Throws NotFoundException if not found
    Task<CategoryResponse> UpdateAsync(int id, UpdateCategoryRequest request);

    //Deletes a category by ID. Throws NotFoundException if not found
    Task DeleteAsync(int id);
}
