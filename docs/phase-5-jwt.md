# Phase 5 — JWT Auth + Global Exception Middleware + Logging

## Goal
Replace cookie-based auth with JWT, add centralized error handling, and add structured logging across all Services.
These three are grouped together because they are all cross-cutting concerns — they affect the whole system, not one specific feature.

---

## JWT Authentication

### Configuration (appsettings.json)
```json
"Jwt": {
  "SecretKey": "your-secret-key-minimum-32-characters-long",
  "Issuer": "BookstoreAPI",
  "Audience": "BookstoreClient",
  "ExpiresInMinutes": 60
}
```

### Program.cs setup
```csharp
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]!)),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateLifetime = true
        };
    });

// Middleware order matters — always UseAuthentication before UseAuthorization
app.UseAuthentication();
app.UseAuthorization();
```

### Claims to include in token
```csharp
var claims = new[]
{
    new Claim(JwtRegisteredClaimNames.Sub,   user.Id),
    new Claim(JwtRegisteredClaimNames.Email, user.Email!),
    new Claim(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString()),
    new Claim(ClaimTypes.Role,               role)
};
```

---

## Global Exception Handling Middleware

### Custom exception classes (Application project)
```csharp
public class NotFoundException   : Exception { public NotFoundException(string msg)   : base(msg) {} }
public class ValidationException : Exception { public ValidationException(string msg) : base(msg) {} }
public class ConflictException   : Exception { public ConflictException(string msg)   : base(msg) {} }
```

### Middleware (API project)
```csharp
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            // Log all unhandled exceptions with full context
            _logger.LogError(ex, "Unhandled exception on {Method} {Path}",
                context.Request.Method, context.Request.Path);

            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // Map exception type to HTTP status code
        var statusCode = exception switch
        {
            NotFoundException   => StatusCodes.Status404NotFound,
            ValidationException => StatusCodes.Status400BadRequest,
            ConflictException   => StatusCodes.Status409Conflict,
            _                   => StatusCodes.Status500InternalServerError
        };

        var problem = new ProblemDetails
        {
            Status   = statusCode,
            Title    = GetTitle(statusCode),
            Detail   = exception.Message,
            Instance = context.Request.Path
        };

        context.Response.StatusCode  = statusCode;
        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsJsonAsync(problem);
    }

    private static string GetTitle(int statusCode) => statusCode switch
    {
        404 => "Not Found",
        400 => "Bad Request",
        409 => "Conflict",
        _   => "Internal Server Error"
    };
}
```

Register in Program.cs — must be first in the pipeline:
```csharp
app.UseMiddleware<ExceptionHandlingMiddleware>();
```

---
## Logging with ILogger

### Inject in every Service
```csharp
public class ProductService : IProductService
{
    private readonly IProductRepository _repository;
    private readonly ILogger<ProductService> _logger;

    public ProductService(IProductRepository repository, ILogger<ProductService> logger)
    {
        _repository = repository;
        _logger = logger;
    }
}
```

### Log level guidelines
```csharp
// Normal business flow — what happened
_logger.LogInformation("Product {ProductId} created successfully", product.Id);

// Unexpected but not a crash — worth monitoring
_logger.LogWarning("Product {ProductId} not found, returning null", id);

// Something broke — always include the exception object
_logger.LogError(ex, "Failed to create product with name {Name}", request.Name);
```

## Checklist before moving to Phase 6

- [ ] JWT configured in Program.cs with correct middleware order
- [ ] Token generation implemented in AccountService
- [ ] All 3 custom exception classes created in Application project
- [ ] ExceptionHandlingMiddleware created and registered first in pipeline
- [ ] ILogger<T> injected in all Service classes
- [ ] Cookie-based auth removed
- [ ] [Authorize] attributes updated on Controllers
