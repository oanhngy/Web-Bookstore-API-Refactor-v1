using BookstoreWeb.Application.Interfaces;
using BookstoreWeb.Infrastructure.Data;
using BookstoreWeb.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace BookstoreWeb.Infrastructure;

//đky Respo vào DI container, Infras layer đky: Repo implementations
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        //dky BokstoreDbContext vs MySQL
        services.AddDbContext<BookstoreContext>(options => options.UseMySql(
            configuration.GetConnectionString("DefaultConnection"),
            ServerVersion.AutoDetect(configuration.GetConnectionString("DefaultConnection"))
        ));

        //dky repository implementations
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();

        return services;
    }
}