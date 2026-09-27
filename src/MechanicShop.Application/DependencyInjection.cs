using System.Reflection;
using FluentValidation;
using MechanicShop.Application.Common.Behaviours;
using Microsoft.Extensions.DependencyInjection;

namespace MechanicShop.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddBehavior(typeof(ValidationBehavior<,>));
            cfg.AddBehavior(typeof(PerformanceBehavior<,>));
            cfg.AddBehavior(typeof(LoggingBehavior<>));
            cfg.AddBehavior(typeof(CachingBehavior<,>));
        });
        return services;
    }
}
