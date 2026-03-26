# Phase 3 — Service Layer + Interfaces + DI

## Goal
Move all business logic out of Controllers into Service classes.
After this phase, Controllers only receive requests and call Services — no DbContext, no business logic.

## What to create
```
BookstoreWeb.Application/
├── Interfaces/
│   ├── IProductService.cs
│   ├── IOrderService.cs
│   ├── ICartService.cs
│   ├── ICategoryService.cs
│   └── IAccountService.cs
├── Services/
│   ├── ProductService.cs
│   ├── OrderService.cs
│   ├── CartService.cs
│   ├── CategoryService.cs
│   └── AccountService.cs
└── DTOs/
    ├── Products/
    │   ├── CreateProductRequest.cs
    │   ├── UpdateProductRequest.cs
    │   └── ProductResponse.cs
    ├── Orders/
    │   ├── UpdateOrderStatusRequest.cs
    │   └── OrderResponse.cs
    └── ...
```

## DI Registration (Program.cs)
Register all services with `Scoped` lifetime:
```csharp
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IAccountService, AccountService>();
```

## What a thin Controller looks like after this phase
```csharp
[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;

    // Inject interface, not concrete class
    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductResponse>>> GetAll()
    {
        var products = await _productService.GetAllAsync();
        return Ok(products);
    }
}
```

## Manual DTO mapping (no AutoMapper)
Put mapping method inside the Service class, keep it private:
```csharp
private static ProductResponse ToResponse(Product product)
{
    return new ProductResponse
    {
        Id = product.Id,
        Name = product.Name,
        Price = product.Price,
        CategoryName = product.Category?.Name
    };
}
```

## Checklist before moving to Phase 4

- [ ] All Service interfaces created in Application/Interfaces/
- [ ] All Service implementations created in Application/Services/
- [ ] All Services registered in Program.cs with Scoped lifetime
- [ ] Controllers inject interfaces, not DbContext
- [ ] No EF Core types referenced in Application project
- [ ] DTOs created for all major entities
