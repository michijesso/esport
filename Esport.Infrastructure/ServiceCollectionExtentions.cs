namespace Esport.Infrastructure;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtentions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<EsportDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("PostgresConnection")));

        return services;
    }
}