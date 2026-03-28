using BookstoreWeb.Application.Interfaces;
using BookstoreWeb.Application.Services;
using Microsoft.Extensions.DependencyInjection;
namespace BookstoreWeb.Application;

//đky all Service vô DI container
//application layer đky thứ nó sỡ hữu: interface +implementation
public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ICartService, CartService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IAdminOrderService, AdminOrderService>();

        //IAccountService — implement ở Phase 5 cùng JWT

        return services;
    }
}