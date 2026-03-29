using BookstoreWeb.Application.DTOs.Common;
using BookstoreWeb.Application.DTOs.Products;
using BookstoreWeb.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
namespace BookstoreWeb.API.Controllers;
using Microsoft.AspNetCore.Authorization;

[Authorize]
[ApiController]
[Route("api/products")]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;
    public ProductController(IProductService productService)
    {
        _productService=productService;
    }

    //1- lấy list products có filter, short, pagin
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<ProductResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? searchString,
        [FromQuery] int? categoryId,
        [FromQuery] string? sortOrder,
        [FromQuery] int page=1,
        [FromQuery] int pageSize=10)
    {
        //truyền hết vào service
        var result=await _productService.GetAllAsync(searchString, categoryId, sortOrder, page, pageSize);
        return Ok(result);
    }

    //2-get 1 product by id
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        //Service tự throw nếu k thấy
        var product=await _productService.GetByIdAsync(id);
        return Ok(product);
    }

    //3-create new
    [HttpPost]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateProductRequest request) {
        //Images bỏ qua ở phase 4, request.Images=list rỗng
        //phase 5 (refactor v2) xử lý file upload với IFormFile
        var created=await _productService.AddAsync(request);
        return CreatedAtAction(nameof(GetById), new {id=created.ProductID}, created);
    }

    //4-update
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProductRequest request)
    {
        //Service throw
        var updated=await _productService.UpdateAsync(id, request);
        return Ok(updated);
    }

    //5-delete
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await _productService.DeleteAsync(id);
        return NoContent();
    }
}