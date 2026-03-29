using BookstoreWeb.Application.DTOs.Categories;
using BookstoreWeb.Application.Exceptions;
using BookstoreWeb.Application.Interfaces;
using BookstoreWeb.Application.Domain;
using Microsoft.Extensions.Logging;
namespace BookstoreWeb.Application.Services;

//Service= trung gian giữa controller và repository
public class CategoryService : ICategoryService
{
    //inject interface, not implementation
    private readonly ICategoryRepository _categoryRepository;

    //ILogger inject qua DI, gắn category log với class CategoryService --> xem log biết từ class nào
    private readonly ILogger<CategoryService> _logger;

    //constructor-nơi DI Container truyền dependency vào
    public CategoryService(ICategoryRepository categoryRepository, ILogger<CategoryService> logger)
    {
        _categoryRepository=categoryRepository;
        _logger=logger;
    }

    //1
    public async Task<IEnumerable<CategoryResponse>> GetAllAsync()
    {
        _logger.LogInformation("Retrieving all categories");
        var categories=await _categoryRepository.GetAllAsync();
        return categories.Select(ToResponse);
    }

    //2
    public async Task<CategoryResponse> GetByIdAsync(int id)
    {
        _logger.LogInformation("Retrieving category with id {CategoryId}", id);
        var category=await _categoryRepository.GetByIdAsync(id);

        //check null
        if(category==null)
        {
            _logger.LogWarning("Category with id {CategoryId} was not found", id);
        throw new NotFoundException($"Category with id {id} was not found");
        }
        return ToResponse(category); //if found
    }

    //3-create
    public async Task<CategoryResponse> AddAsync(CreateCategoryRequest request)
    {
        _logger.LogInformation("Creating new category with name {name}", request.Name);

        //map DTO to entity
        var category=new Category
        {
            Name=request.Name
        };

        //gửi entity cuống Repo để insert vào db, Repo trả về entity
        var created=await _categoryRepository.AddAsync(category);

        _logger.LogInformation("Category created with id {CategoryId}", created.CategoryID);

        return ToResponse(created); //map entity vừa tạo to DTO trả về Controller, trả DTO vs HTTP 201 created
    }

    //4-update
    public async Task<CategoryResponse> UpdateAsync(int id, UpdateCategoryRequest request)
    {
        _logger.LogInformation("Updating category with id {CategoryId}", id);
        //1st fetch entity từ db, check có exist k
        var category=await _categoryRepository.GetByIdAsync(id);

        if(category==null)
        {
            _logger.LogWarning("Category with id {CategoryId} was not found", id);
            throw new NotFoundException($"Category with id {id} was not found");
        }

        //ONLY update field đc phép thay đổi
        category.Name=request.Name;

        //gọi Repo lưu update vào db
        var updated=await _categoryRepository.UpdateAsync(category);

        //map entity to DTO + return
        return ToResponse(updated);
    }

    //5- delete
    public async Task DeleteAsync(int id)
    {
        _logger.LogInformation("Deleting category with id {CategoryId}", id);
        var exists=await _categoryRepository.ExistsAsync(id);
        if(!exists)
        {
            _logger.LogWarning("Category with id {CategoryId} was not found", id);
            throw new NotFoundException($"Category with id {id} was not found");
        }
        await _categoryRepository.DeleteAsync(id);
    }

    //helper
    private static CategoryResponse ToResponse(Category category)
    {
        return new CategoryResponse
        {
            CategoryID=category.CategoryID,
            Name=category.Name
        };
    }
}