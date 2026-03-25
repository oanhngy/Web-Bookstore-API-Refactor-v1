# Web-Bookstore-API-Refactor-v1

## 1. Introduction
This project is a **web application simulating an online bookstore management and sales system**, developed using **ASP.NET Core MVC (C#)** and **MySQL**.

The application supports both **customer-facing features** (browsing books, shopping cart, ordering) and **administration features** (managing books, categories, orders, and revenue reports), aiming to practice web application development following the MVC pattern.

> **Note:** This is a rebuild of an original project that was lost (Models, Views, and SQL Server database were missing).
> The rebuild focused on **backend development** (controllers, models, database, business logic) as the primary learning objective.
> Frontend styling (Bootstrap UI) was handled with the assistance of **Claude AI (Claude Code)**.

---

## 2. Main Features

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

## 3. Technologies Used
- **Programming Languages:** C#, HTML, CSS
- **Framework:** ASP.NET Core MVC 6
- **Database:** MySQL (database script included: `dulieuBookstoreDb.sql`)
- **Tools & Libraries:** VS Code, Entity Framework Core, Bootstrap 5, ASP.NET Identity, X.PagedList

---

## 4. Project Structure
- **Controllers/** — Handle user requests and application logic
- **Models/** — Define data entities: Product, Category, Order, OrderDetail, ProductImage
- **Views/** — User interface built with Razor Views (MVC)
- **Data/** — BookstoreContext (EF Core DbContext) and SeedData
- **Migrations/** — Entity Framework Core migrations
- **wwwroot/** — Static files (CSS, JavaScript, uploaded images)

---

## 5. Installation & Run Guide

1. Clone the project from GitHub
2. Open the project using **VS Code** or any editor
3. Import `dulieuBookstoreDb.sql` into **MySQL** to initialize the database schema and seed data
4. Update the connection string in `appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=localhost;Database=BookstoreDb;User=root;Password=yourpassword;"
   }
   ```
5. Run the project:
   ```bash
   dotnet watch
   ```
6. Access the application at `https://localhost:7001`
7. A default Admin account is automatically created on first run:
   - **Email:** `admin@gmail.com`
   - **Password:** `Abc@123456`

---

## 6. Technical Highlights
- Developed using ASP.NET Core MVC following the Model–View–Controller pattern
- Implemented role-based authentication and authorization for **Admin** and **Customer**
- Used Entity Framework Core for database interaction and migrations
- Migrated database from SQL Server to MySQL using Pomelo EF Core provider
- Applied CRUD operations, searching, filtering, and pagination (X.PagedList)
- Primary image selection and image deletion for product management
- Revenue report powered by Chart.js with dynamic API endpoint
- Responsive UI built with Bootstrap 5 and Bootstrap Icons

---

## 7. What I Learned
- Building web applications using ASP.NET Core MVC
- Working with Entity Framework Core and MySQL
- Implementing authentication, authorization, and role management with ASP.NET Identity
- Designing and managing relational databases
- Structuring scalable and maintainable web projects
- Understanding real-world workflows of an e-commerce system
- Migrating databases across different providers (SQL Server → MySQL)

---

## 8. Screenshots

### Customer Section

Home page

Book details

Cart

Checkout

My Orders

### Common Features

Login

Register

### Admin Section

Dashboard

Product Management

Order Management

Revenue Report

Category Management
