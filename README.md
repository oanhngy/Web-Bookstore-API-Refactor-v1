# Web-Bookstore-API-Refactor-v1

## 1. Introduction
 A refactored version of [Web-bookstore] (httpsL//github.com/oanhngy/Web-Bookstore) - migrated from **ASP.NET Core MVC** to a **RESTful Web API** with layered atchitecture (3-Tier), JWT authentication, repository pattern, global exception handling, logging and unit tests.

 **Original project**: ASP.NET Core MVC + MySQL + Razor Views
 **This project**: ASP.NET Core Web API + MySQL + Layered Architecture

---
## 2. My Role
This project focused entirely on **backend development**. The frontend from the original MVC project is kept as-is and not part of this refactor scope.

Responsibilities:
- Analyzed the original MVC codebase and identified architectural pan points (fat controller, no service layer, no DI abstraction, no tests)
- Designed the 3-tier layered architecture and defined project boudaries
- Migrated all business logic from Controller into Service classes
- Replaced cookie0based ASP.NET Identity auth with JWT Bearer authentication
- Added global exception handling middleware and structured logging via ILogger<T>
- Wrote unit test for Service layer using xUnit and Moq

**AI-Assisted development**: Claude Code and ChatGPT were used as assistants for generating boilerplate, debugging, and pattern suggestions. All architectural decisions and business logic were written and verified by me.

---
## 3. Target Architecture
```
┌──────────────────────────────┐
│      Presentation Layer      │  BookstoreWeb.API
│      (Controllers only)      │  HTTP in/out, no business logic
├──────────────────────────────┤
│      Application Layer       │  BookstoreWeb.Application
│   (Services + Interfaces)    │  Business logic, DTOs
├──────────────────────────────┤
│    Infrastructure Layer      │  BookstoreWeb.Infrastructure
│  (Repositories + EF Core)    │  Data access, DbContext, Migrations
└──────────────────────────────┘
              ↓
       MySQL Database
```

**Dependency rule:** Each layer only depends on the layer directly below it, always through interfaces — never concrete classes.

---
## 4. Key improvements
- Architecture: Fat controller -> 3-tier layered
- Business logic:L Scattered inside Controller -> Centralized in Service layer
- Data access: DbContext call directly in Controllers -> Repository pattern
- Authentication: Cookie-based -> JWT Bearer (stateless)
- DI: Only DbContext injected -> Full interface-based DI for all Services and Repositories
- Exception handling: None -> Global middleware, consistent ProblemDetails response
- Logging: Config exited but unused -> ILogger<T> injected in all Services
- Unit test: None -> xUnit + Moq covering Service layer business logic

---

## 5. Main Features

### User Management
- Role-based authorization (**Admin** / **Customer**)
- Register and login with ASP.NET Identity
- Default Admin account seeded on startup

### Book Management
- CRUD operations for books (Admin)
- Upload multiple product images with primary image selection
- Delete existing images in edit view
- Search books by title

### Category Management
- CRUD operations for categories (Admin)

### Shopping Cart
- Add books to cart
- Update quantities
- Remove items from cart

### Order Management
- Order status flow: `New (cart)` → `Checked Out` → `Confirmed` → `Completed` / `Cancelled`
- Customer can cancel orders that are not yet completed
- Admin can update order status from Order Management panel

### Admin Panel
- Dashboard with stats: total products, new orders today, today's revenue
- Manage products, categories, and orders
- Revenue report with bar chart (filter by day / week / month / year)

---
## 6. Project Structure
### Before (MVC - single project)
```
Web-Bookstore/
├── Controllers/          # Fat controllers — business logic + DB access + View return
├── Models/               # EF Core entities
├── Views/                # Razor Views
├── Data/                 # BookstoreContext, SeedData
├── Migrations/
└── wwwroot/
```

### After (this project - 4 projects)

```
Web-Bookstore-API-Refactor-v1/
├── BookstoreWeb.API/
│   ├── Controllers/      # HTTP only — inject Service, return ActionResult
│   ├── Middleware/        # GlobalExceptionHandlingMiddleware
│   └── Program.cs        # DI registration, JWT config, middleware pipeline
│
├── BookstoreWeb.Application/
│   ├── Services/         # ProductService, OrderService, CartService, ...
│   ├── Interfaces/       # IProductService, IOrderService, ...
│   └── DTOs/             # Request/Response DTOs
│
├── BookstoreWeb.Infrastructure/
│   ├── Repositories/     # ProductRepository, OrderRepository, ...
│   ├── Data/             # BookstoreContext, SeedData, Migrations
│   └── DependencyInjection.cs
│
└── BookstoreWeb.Tests/
    └── Services/         # Unit tests — xUnit + Moq
```

---
## 7. Technologies Used
- Language: C#
- Framework: ASP.NEt Core Web API (.NET 8)
- Database: MySQL - Pomelo EF Core provider
- ORM: Entity Framework Core (code first)
- Authentication: JWT Bearer
- Testing: xUnit, Moq
- Logging: ILogger<T>
- Tools: VS Code, Claude Code, Postman

## 8. Installation & Run Guide

### Prerequisites
- .NET 6 SDK
- MySQL Server
- VS Code (or any editor)

### Steps

1. Clone the repository
   ```bash
   git clone https://github.com/oanhngy/Web-Bookstore-API-Refactor-v1.git
   cd Web-Bookstore-API-Refactor-v1
   ```

2. Import the database
   ```bash
   # Import dulieuBookstoreDb.sql into MySQL
   mysql -u root -p < dulieuBookstoreDb.sql
   ```

3. Update connection string and JWT config in `BookstoreWeb.API/appsettings.json`
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=localhost;Database=BookstoreDb;User=root;Password=yourpassword;"
   },
   "Jwt": {
     "SecretKey": "your-secret-key-here",
     "Issuer": "BookstoreAPI",
     "Audience": "BookstoreClient",
     "ExpiresInMinutes": 60
   }
   ```

4. Run EF Core Migrations
   ```bash
   cd BookstoreWeb.Infrastructure
   dotnet ef database update --startup-project ../BookstoreWeb.API
   ```

5. Run the API
   ```bash
   cd BookstoreWeb.API
   dotnet watch
   ```

6. API available at: https://localhost:7001
   Swagger UI at:    https://localhost:7001/swagger

7. Default Admin account (seeded on first run):
   - **Email:** `admin@gmail.com`
   - **Password:** `Abc@123456`

### Run Unit Tests
```bash
cd BookstoreWeb.Tests
dotnet test
```

---

## What I Learned
(later)
