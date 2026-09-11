using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;

namespace MechanicShop.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IApplicationDbContextInitialiser, ApplicationDbContextInitialiser>();
        return services;
    }
}
