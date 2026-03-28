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

## DI Registration

DI registration is split across two files — each layer registers what it owns:

**`Application/DependencyInjection.cs`** — registers Services (both interface and implementation live in Application):
```csharp
public static IServiceCollection AddApplicationServices(this IServiceCollection services)
{
    services.AddScoped<ICategoryService, CategoryService>();
    services.AddScoped<IProductService, ProductService>();
    services.AddScoped<ICartService, CartService>();
    services.AddScoped<IOrderService, OrderService>();
    services.AddScoped<IAdminOrderService, AdminOrderService>();
    return services;
}
```

**`Infrastructure/DependencyInjection.cs`** — registers Repositories (Phase 4):
```csharp
public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
{
    // Repository registrations added in Phase 4
    return services;
}
```

**`Program.cs`** calls both:
```csharp
builder.Services.AddApplicationServices();    // Services
builder.Services.AddInfrastructureServices(); // Repositories (Phase 4)
```

**Why split?** Infrastructure cannot reference Application (circular dependency — Application already references Infrastructure for domain models). Each layer only registers what it owns.

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

- [x] All Service interfaces created in Application/Interfaces/
- [x] All Service implementations created in Application/Services/
- [ ] All Services registered in Program.cs with Scoped lifetime  ← đang làm (DependencyInjection.cs)
- [ ] Controllers inject interfaces, not DbContext                 ← Phase 4
- [x] No EF Core types referenced in Application project
- [x] DTOs created for all major entities
