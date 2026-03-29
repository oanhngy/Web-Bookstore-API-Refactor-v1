using BookstoreWeb.Application.DTOs.Common;
using BookstoreWeb.Application.DTOs.Products;
using BookstoreWeb.Application.Exceptions;
using BookstoreWeb.Application.Interfaces;
using BookstoreWeb.Application.Domain;
using Microsoft.Extensions.Logging;
namespace BookstoreWeb.Application.Services;

public class ProductService : IProductService
{
    //inject interface
    private readonly IProductRepository _productRepository;

    //gắn log vs class
    private readonly ILogger<ProductService> _logger;

    //constructor DI
    public ProductService(IProductRepository productRepository, ILogger<ProductService> logger)
    {
        _productRepository=productRepository;
        _logger=logger;
    }

    //1-get all-dsach có filter, sort, pagination
    public async Task<PagedResponse<ProductResponse>> GetAllAsync(string? searchString, int? categoryId, string? sortOrder, int page, int pageSize)
    {
        _logger.LogInformation("Retrieving products - search: {Search}, categoryId: {CategoryId}, page: {Page}", searchString, categoryId, page);
        //query 1: lấy đúng số item trong trang hiện tại
        var products=await _productRepository.SearchAsync(searchString, categoryId, sortOrder, page, pageSize);

        //query 2: đếm tổng số item khớp filter
        var totalCount=await _productRepository.GetCountAsync(searchString, categoryId);

        //trả PagedResponse - bọc data + metadata pagination vào 1 project
        return new PagedResponse<ProductResponse>
        {
            Items=products.Select(ToResponse),
            //metadata pagination
            Page=page,
            PageSize=pageSize,
            TotalCount=totalCount
        };
    }

    //2- get by id
    public async Task<ProductResponse> GetByIdAsync(int id)
    {
        _logger.LogInformation("Retrieving product with id {ProductId}", id);
        //gọi repo lấy entity
        var product=await _productRepository.GetByIdAsync(id);
        if(product==null)
        {
            //data issue --> warning
            _logger.LogWarning("Product with id {ProductId} was not found", id);
            throw new NotFoundException($"Product with id {id} was not found");
        }
        //map entity -> DTO + return
        return ToResponse(product);
    }

    //3-create
    //map dsach ảnh từ DTO -> entity, ProductImage =navigation property, EF Core sẽ insert kèm Product
    public async Task<ProductResponse> AddAsync(CreateProductRequest request)
    {
        _logger.LogInformation("Creating a new product with name {ProductName}", request.Name);

        //map DTO -> entity, k xài thẳng request vì entity có field nhạy cảm/field user k tự set
        var product=new Product
        {
            Name=request.Name,
            Description=request.Description,
            Author=request.Author,
            Price=request.Price,
            CategoryID=request.CategoryID,
            //dsach ảnh
            ProductImages=request.Images.Select(img => new ProductImage
            {
                ImagePath=img.FileName,
                IsPrimary=img.IsPrimary,
                ImageType=Path.GetExtension(img.FileName).TrimStart('.')
            }).ToList()
        };

        //insert vào db
        var created=await _productRepository.AddAsync(product);
        _logger.LogInformation("Product created with id {ProductId}", created.ProductID);
        return ToResponse(created);
    }

    //4-update
    //có 3 thao tác: xóa hình theo ID, đổi hình chính, thêm hình mới. Thứ tự: xóa -> đổi primary -> set primary mới
    public async Task<ProductResponse> UpdateAsync(int id, UpdateProductRequest request)
    {
        _logger.LogInformation("Updating product with id {ProductId}", id);
        //fetch entity về memory, EFcore track entity này để update
        //GetByIdAsync nên incllude ProductImages 
        var product=await _productRepository.GetByIdAsync(id);
        if(product==null)
        {
            _logger.LogWarning("Product with id {ProductId} was not found", id);
            throw new NotFoundException($"Product with id {id} was not found");
        }

        //upate field
        product.Name=request.Name;
        product.Description=request.Description;
        product.Author=request.Author;
        product.Price=request.Price;
        product.CategoryID=request.CategoryID;

        //if ProductImages null -> gán new List, if !null giữ nguyên
        product.ProductImages ??=new List<ProductImage>();

        //a-xóa
        product.ProductImages.RemoveAll(img =>request.DeleteImageIds.Contains(img.ImageID));

        //b-đổi hình chính: PrimaryImageId=0 -> k đổi, >0 đổi sang id đó
        if(request.PrimaryImageId >0)
        {
            foreach(var img in product.ProductImages)
            {
                //******************
                img.IsPrimary=img.ImageID==request.PrimaryImageId;
            }
        }

        //c-thêm hình mới: append vào cái đang có
        foreach (var img in request.Images)
        {
            product.ProductImages.Add(new ProductImage
            {
                ImagePath = img.FileName,
                IsPrimary = img.IsPrimary,
                ImageType = Path.GetExtension(img.FileName).TrimStart('.')
            });
        }
    var updated=await _productRepository.UpdateAsync(product);
    return ToResponse(updated);
    }

    //5-delete theo id
    public async Task DeleteAsync(int id)
    {
        _logger.LogInformation("Deleting product with id {ProductId}", id);
        var exists=await _productRepository.ExistsAsync(id);
        if(!exists)
        {
            _logger.LogWarning("Product with id {ProductId} was not found", id);
            throw new NotFoundException($"Product with id {id} was not found");
        }
        await _productRepository.DeleteAsync(id);
    }

    //helper mapping
    private static ProductResponse ToResponse(Product product)
    {
        return new ProductResponse
        {
            ProductID=product.ProductID,
            Name=product.Name,
            Description=product.Description,
            Author=product.Author,
            Price=product.Price,
            CategoryID=product.CategoryID,

            //!null -> .NAme, null -> trả null
            CategoryName=product.Category?.Name,

            //null -> trả list rỗng. Images luôn là 1 list( maybe rỗng) nhưng kbh null
            Images = product.ProductImages?.Select(img => new ProductImageResponse
            {
                ImageID=img.ImageID,
                ImagePath=img.ImagePath,
                IsPrimary=img.IsPrimary
            }).ToList() ?? new List<ProductImageResponse>()
        };
    }
}