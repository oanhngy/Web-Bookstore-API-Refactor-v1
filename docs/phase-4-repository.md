# Phase 4 — Repository Pattern

## Goal
Create an abstraction layer between Services and the database.
After this phase, Services no longer know about EF Core or DbContext — they only call Repository interfaces.

## What to create
```
BookstoreWeb.Application/
└── Interfaces/               ← Repository interfaces already created here (Phase 3)
    ├── ICategoryRepository.cs
    ├── IProductRepository.cs
    └── IOrderRepository.cs

BookstoreWeb.Infrastructure/
├── Repositories/             ← implementations go here
│   ├── CategoryRepository.cs
│   ├── ProductRepository.cs
│   └── OrderRepository.cs
└── DependencyInjection.cs    ← AddInfrastructureServices() — register repositories here
```

## Method naming rule: [Verb][Noun][Condition]Async

```csharp
public interface IProductRepository
{
    Task<Product?> GetByIdAsync(int id);
    Task<IEnumerable<Product>> GetAllAsync();
    Task<IEnumerable<Product>> GetByCategoryIdAsync(int categoryId);
    Task<IEnumerable<Product>> SearchByNameAsync(string keyword);
    Task AddAsync(Product product);
    Task UpdateAsync(Product product);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
```

## DI Registration (Infrastructure/DependencyInjection.cs)

`AddInfrastructureServices()` already exists (created in Phase 3 as placeholder).
Fill in repository + DbContext registrations:

```csharp
public static IServiceCollection AddInfrastructureServices(
    this IServiceCollection services, IConfiguration configuration)
{
    // DbContext — connects to MySQL via connection string in appsettings.json
    services.AddDbContext<BookstoreContext>(options =>
        options.UseMySql(
            configuration.GetConnectionString("DefaultConnection"),
            ServerVersion.AutoDetect(configuration.GetConnectionString("DefaultConnection"))
        ));

    // Repositories
    services.AddScoped<ICategoryRepository, CategoryRepository>();
    services.AddScoped<IProductRepository, ProductRepository>();
    services.AddScoped<IOrderRepository, OrderRepository>();

    return services;
}
```

Program.cs — pass configuration to AddInfrastructureServices:
```csharp
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
```

## What a Repository implementation looks like

```csharp
public class ProductRepository : IProductRepository
{
    private readonly BookstoreContext _context;

    public ProductRepository(BookstoreContext context)
    {
        _context = context;
    }

    // Get a single product by its ID, including its category
    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _context.Products
            .Include(p => p.Category)
            .Include(p => p.ProductImages)
            .FirstOrDefaultAsync(p => p.ProductID == id);
    }

    // Get all products with their categories
    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _context.Products
            .Include(p => p.Category)
            .Include(p => p.ProductImages.Where(img => img.IsPrimary))
            .ToListAsync();
    }
}
```

## Checklist before moving to Phase 5

- [x] Repository interfaces created in Application/Interfaces/
- [x] BookstoreContext created in Infrastructure/Data/
- [x] Repository implementations created in Infrastructure/Repositories/
- [x] All repositories registered in Infrastructure/DependencyInjection.cs
- [x] Controllers created in API — inject Service interfaces, not DbContext
- [x] dotnet build passes with 0 errors
