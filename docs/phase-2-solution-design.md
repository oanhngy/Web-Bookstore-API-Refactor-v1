# Phase 2 — Design Solution Structure

## Goal
Design the project structure and establish all conventions before writing any code.
This phase produces the blueprint that all subsequent phases follow.

---
## Architecture: 3-tier Layered Architecture

```
BookstoreWeb.API/
│   Purpose: Entry point. Handle HTTP only.
│   Contains: Controllers, Middleware, Program.cs, appsettings.json, Swagger config
│   Must NOT: contain business logic or reference DbContext directly
│
BookstoreWeb.Application/
│   Purpose: Business logic layer. The heart of the system.
│   Contains: Service interfaces, Service implementations, DTOs
│   Must NOT: reference EF Core, DbContext, or any Infrastructure types
│
BookstoreWeb.Infrastructure/
│   Purpose: Data access layer. Talk to the database.
│   Contains: Repository interfaces, Repository implementations,
│             BookstoreContext, Migrations, DependencyInjection.cs
│   Must NOT: contain business logic
│
BookstoreWeb.Tests/
    Purpose: Verify business logic works correctly.
    Contains: xUnit test classes, Moq mocks
    References: BookstoreWeb.Application only
```

**Dependency rule (never violate):**
```
API → Application → Infrastructure → Database
```
- Each project only references the project directly below it
- No circular dependencies
- Application has zero knowledge of HTTP or EF Core
- This rule is enforced by project references in `.csproj` files

---
## Interfaces to create

**Service interfaces** — in `Application/Interfaces/`:
```
IProductService
IOrderService
ICartService
ICategoryService
IAccountService
```

**Repository interfaces** — in `Infrastructure/Repositories/`:
```
IProductRepository
IOrderRepository
ICategoryRepository
```

---
## Developer Tools

**Swagger UI** replaces Razor Views as the visual interface.
- Package: `Swashbuckle.AspNetCore`
- Enabled: Development environment only
- URL: `https://localhost:7001/swagger`
- Purpose: Explore endpoints, test requests/responses directly in browser

**Postman** is used for structured API testing:
- Test auth flows (register → login → get token → call protected endpoint)
- Test edge cases (invalid input, missing token, 404, 409)
- Swagger handles quick visual checks; Postman handles scenario-based testing

---
## How to create the solution structure
```bash
# Create solution
dotnet new sln -n BookstoreWeb

# Create projects
dotnet new webapi  -n BookstoreWeb.API
dotnet new classlib -n BookstoreWeb.Application
dotnet new classlib -n BookstoreWeb.Infrastructure
dotnet new xunit   -n BookstoreWeb.Tests

# Add all projects to solution
dotnet sln add BookstoreWeb.API
dotnet sln add BookstoreWeb.Application
dotnet sln add BookstoreWeb.Infrastructure
dotnet sln add BookstoreWeb.Tests

# Set project references — enforce the dependency rule
dotnet add BookstoreWeb.API           reference BookstoreWeb.Application
dotnet add BookstoreWeb.Application   reference BookstoreWeb.Infrastructure
dotnet add BookstoreWeb.Tests         reference BookstoreWeb.Application
```

> **Important:** `BookstoreWeb.API` does NOT directly reference `BookstoreWeb.Infrastructure`.
> Infrastructure is registered via DI at runtime — API never needs to know about it at compile time.

---
## Expected folder structure after this phase
```
Web-Bookstore-API-Refactor-v1/
├── CLAUDE.md
├── README.md
├── BookstoreWeb.sln
├── docs/
│   ├── phase-2-solution-design.md
│   ├── phase-3-service.md
│   ├── phase-4-repository.md
│   ├── phase-5-jwt.md
│   └── phase-6-testing.md
├── BookstoreWeb.API/
│   ├── Controllers/
│   ├── Middleware/
│   ├── Program.cs
│   └── appsettings.json
├── BookstoreWeb.Application/
│   ├── Interfaces/
│   ├── Services/
│   └── DTOs/
├── BookstoreWeb.Infrastructure/
│   ├── Repositories/
│   ├── Data/
│   └── DependencyInjection.cs
└── BookstoreWeb.Tests/
    └── Services/
```

---
## Checklist
- [x] Architecture confirmed: 3-tier
- [x] All 4 projects created and added to solution
- [x] Project references set correctly in .csproj
- [x] Folder structure created inside each project
- [x] All naming conventions decided — documented in CLAUDE.md
- [x] All technical decisions made (DTO, exceptions, JWT, testing, Swagger) — documented in CLAUDE.md
- [x] Swagger UI configured and accessible at https://localhost:7001/swagger
- [x] Ready to start Phase 3
