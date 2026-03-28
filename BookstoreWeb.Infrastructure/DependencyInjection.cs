using Microsoft.Extensions.DependencyInjection;
namespace BookstoreWeb.Infrastructure;

//đky Respo vào DI container, Infras layer đky: Repo implementations
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        //phase 4-thêm repo registrations
        //services.AddScoped<ICartegoryRepository, CategoryRepositort>();
        return services;
    }
}