# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Purpose
This is a **learning project**, understanding and skill improvement matter more than finishing fast.
- Guide me like a **senior developer teaching a fresher developer**
- For each task, explain: what are we doing, why, what options exits, trade-offs, which file+line to check
- Show how to commit to GitHub and what the commit message say
- After each step, ask for my confirmation before moving on

**Code changes:**
- Do NOT write, edit or refactor any code until I explicitly approve
- If code changes are needed, propose the plan as simple bullet points first
- Wait for my approval before applying any change
- When multiple valid approaches exist, recommend one and explain why

**Coding rules:**
- Every function and logic block must have a comment explaining what it does (not how)
- Prefer clear naming over short or clever code
- Keep code simple, readable and easy to understand
- Avoid unnecessary complexity

---
## Refactor Goal
Migrating from **ASP.NET Core MVC** to **Web API with 3-tier layered architecture**.

**Problem with original codebase:**
- Fat controllers — business logic, DB access, and response all in one place
- No service layer, no repository pattern, no DI abstraction
- Cookie-based auth — not compatible with API clients
- No logging, no global exception handling, no unit tests
- Razor Views — not usable by API clients; replaced by Swagger UI

**Target solution structure:**
```
BookstoreWeb.API/             Controllers, Middleware, Program.cs
BookstoreWeb.Application/     Services, Interfaces, DTOs
BookstoreWeb.Infrastructure/  Repositories, Data, DependencyInjection.cs
BookstoreWeb.Tests/           xUnit + Moq
```

**Dependency rule (never violate):**
```
API → Application → Infrastructure → Database
```
- Controllers must NOT inject `DbContext` directly
- Services must NOT reference any EF Core types
- Each layer depends only on the layer below it, always through interfaces

**Refactor roadmap:**
- [x] Phase 1 — Analyze codebase, identify pain points
- [ ] Phase 2 — Design solution structure
- [ ] Phase 3 — Service layer + Interfaces + DI
- [ ] Phase 4 — Repository pattern
- [ ] Phase 5 — JWT auth + Global Exception Middleware + Logging
- [ ] Phase 6 — Unit tests
- [ ] Phase 7 — EF Migration + end-to-end testing

Always check which phase is active before suggesting code changes. Do not skip ahead.

---

## Naming Conventions
Priority: **clarity over brevity**. A longer name that is immediately clear is better than a short name that requires guessing.

| Type | Pattern | Example |
|---|---|---|
| Interface | `I` prefix | `IProductService`, `IOrderRepository` |
| Service | `[Entity]Service` | `ProductService`, `OrderService` |
| Repository | `[Entity]Repository` | `ProductRepository`, `OrderRepository` |
| DTO Request | `[Action][Entity]Request` | `CreateProductRequest`, `UpdateOrderStatusRequest` |
| DTO Response | `[Entity]Response` | `ProductResponse`, `OrderResponse` |
| Middleware | `[Name]Middleware` | `ExceptionHandlingMiddleware` |
| Test class | `[Entity]ServiceTests` | `ProductServiceTests` |

**Repository method naming rule:** `[Verb][Noun][Condition]Async`
```csharp
GetByIdAsync(int id)
GetAllAsync()
GetByCategoryIdAsync(int categoryId)
SearchByNameAsync(string keyword)
AddAsync(Product product)
UpdateAsync(Product product)
DeleteAsync(int id)
ExistsAsync(int id)
```
Never use vague names like `FindAsync`, `GetAsync`, `SaveAsync`.

**Test method naming:** `MethodName_Scenario_ExpectedResult`
```
CancelOrderAsync_WhenStatusIsCompleted_ShouldThrowException
GetByIdAsync_WhenProductNotFound_ShouldThrowNotFoundException
```

---
## Key Technical Decisions

**DTOs:** Separate Request / Response DTOs. Manual mapping — no AutoMapper. Never return EF entity from Controller.

**Custom exceptions:**
```
NotFoundException   → HTTP 404
ValidationException → HTTP 400
ConflictException   → HTTP 409
Any other Exception → HTTP 500
```
Mapping lives in `ExceptionHandlingMiddleware` only. Services only `throw` — never set HTTP status codes.

**Error response format:** ProblemDetails (RFC 7807)
```json
{ "status": 404, "title": "Not Found", "detail": "Product with id 99 was not found" }
```

**JWT:** HS256 algorithm. Claims: `sub` (userId), `email`, `jti` (Guid.NewGuid()), `role`. Expiry: 60 minutes. No refresh token. SecretKey min 32 chars, always read from `appsettings.json`.

**Logging:** `ILogger<T>` injected in all Services. Use `Information` for normal flow, `Warning` for unexpected situations, `Error` with exception for failures.

**Swagger:** Swashbuckle.AspNetCore. Enabled in Development only.
Replaces Razor Views as the visual interface for exploring and testing the API.
All controllers must have XML doc comments on actions (`/// <summary>`).
Use `[ProducesResponseType]` to document all response codes per endpoint.

**Unit tests:** xUnit + Moq. Test files mirror Service structure under `Tests/Services/`. Pattern: Arrange → Act → Assert. Mock Repository interfaces — never use real DbContext. Focus on Services with business logic (`OrderService`, `CartService`, `ProductService`). Skip Repository and Controller tests.

---
## Original Data Model (reference — unchanged in refactor)
- `Product` → belongs to `Category` (n-1), has many `ProductImage` (1-n)
- `Order` → belongs to `IdentityUser` via `UserID`, has many `OrderDetail` (1-n)
- `OrderDetail` → belongs to `Order` and `Product`
- Cart = `Order` with `Status = "New"` — not a separate entity
- Status flow: `New` → `Checked Out` → `Confirmed` → `Completed` / `Cancelled`
- `BookstoreContext` extends `IdentityDbContext<IdentityUser>`

---
## Commands
```bash
dotnet run --project BookstoreWeb.API
dotnet build
dotnet test
dotnet test --collect:"XPlat Code Coverage"
dotnet ef migrations add <n> --project BookstoreWeb.Infrastructure --startup-project BookstoreWeb.API
dotnet ef database update --project BookstoreWeb.Infrastructure --startup-project BookstoreWeb.API
```

After `dotnet run`:
```
API available at: https://localhost:7001
Swagger UI at:    https://localhost:7001/swagger
```

---
## Phase Reference Files

When starting a new phase, tell Claude Code to read the corresponding file:
> "Read docs/phase-3-service.md before we start"

```
docs/phase-2-solution-design.md     F3-tier architecture, project boundaries, interface list, folder structure
docs/phase-3-service.md             Service layer, Interface, DI
docs/phase-4-repository.md          Repository pattern
docs/phase-5-jwt.md                 JWT, Middleware, Logging
docs/phase-6-testing.md             Unit tests, coverage
```