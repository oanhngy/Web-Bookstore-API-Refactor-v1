using BookstoreWeb.Application.DTOs.Categories;
using BookstoreWeb.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
namespace BookstoreWeb.API.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;
    public CategoryController(ICategoryService categoryService)
    {
        _categoryService=categoryService;
    }

    //1- get list of all categories
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CategoryResponse>), StatusCodes.Status200OK)]
    public async Task <IActionResult> GetAll()
    {
        var categories=await _categoryService.GetAllAsync();
        return Ok(categories);
    }

    //2-get 1 product by id
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CategoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var category=await _categoryService.GetByIdAsync(id); //tự throw nếu k tìm thấy, phase 5 sẽ bắt + trả 404
        return Ok(category);
    }

    //3-create new
    [HttpPost]
    [ProducesResponseType(typeof(CategoryResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateCategoryRequest request)
    {
        //ApiController tự validate required/length, if sai -> trả 400 trước khi vào đây
        var created=await _categoryService.AddAsync(request);

        return CreatedAtAction(nameof(GetById), new{id=created.CategoryID}, created);
    }

    //4-update
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(CategoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task <IActionResult> Update(int id, [FromBody] UpdateCategoryRequest request)
    {
        //Service throw nếu id k tồn tại
        var updated=await _categoryService.UpdateAsync(id, request);
        return Ok(updated);
    }

    //5-delete
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        //Service throw
        await _categoryService.DeleteAsync(id);
        return NoContent();
    }
}