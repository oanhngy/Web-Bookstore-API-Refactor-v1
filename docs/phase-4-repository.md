# Phase 4 — Repository Pattern

## Goal
Create an abstraction layer between Services and the database.
After this phase, Services no longer know about EF Core or DbContext — they only call Repository interfaces.

## What to create
```
BookstoreWeb.Infrastructure/
├── Repositories/
│   ├── Interfaces/           ← or put these in Application/Interfaces/
│   │   ├── IProductRepository.cs
│   │   ├── IOrderRepository.cs
│   │   └── ICategoryRepository.cs
│   ├── ProductRepository.cs
│   ├── OrderRepository.cs
│   └── CategoryRepository.cs
└── DependencyInjection.cs    ← register repositories here
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

## DI Registration (DependencyInjection.cs)

```csharp
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        // DbContext
        services.AddDbContext<BookstoreContext>(options =>
            options.UseMySql(
                configuration.GetConnectionString("DefaultConnection"),
                ServerVersion.AutoDetect(configuration.GetConnectionString("DefaultConnection"))
            ));

        // Repositories
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();

        return services;
    }
}
```

Then in Program.cs, replace individual registrations with:
```csharp
builder.Services.AddInfrastructure(builder.Configuration);
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

- [ ] Repository interfaces created
- [ ] Repository implementations created in Infrastructure project
- [ ] All repositories registered in DependencyInjection.cs
- [ ] Services updated — no more direct DbContext calls
- [ ] DbContext and Migrations moved to Infrastructure/Data/
- [ ] No EF Core references remain in Application project
