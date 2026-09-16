using System.Text;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Infrastructure.BackgroundServices;
using MechanicShop.Infrastructure.Data;
using MechanicShop.Infrastructure.Data.Interceptors;
using MechanicShop.Infrastructure.Identity;
using MechanicShop.Infrastructure.Identity.Policies;
using MechanicShop.Infrastructure.RealTime;
using MechanicShop.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace MechanicShop.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddScoped<IApplicationDbContextInitialiser, ApplicationDbContextInitialiser>();

        services.AddSingleton(TimeProvider.System);

        var connectionString = configuration.GetConnectionString("DefaultConnection");
        ArgumentNullException.ThrowIfNull(connectionString);

        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IWorkOrderPolicy, WorkOrderPolicy>();
        services.AddTransient<IIdentityService, IdenetityService>();
        services.AddScoped<ITokenProvider, TokenProvider>();
        services.AddDbContext<AppDbContext>(
            (IServiceProvider sp, DbContextOptionsBuilder options) =>
            {
                options.AddInterceptors(sp.GetService<ISaveChangesInterceptor>()!);
                options.UseSqlServer(connectionString);
            }
        );

        services.AddScoped<IAppDbContext>(op => op.GetService<AppDbContext>()!);

        services
            .AddAuthentication(op =>
            {
                op.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                op.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(op =>
            {
                var jwtSettings = configuration.GetSection("JwtSettings");

                op.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidateAudience = true,
                    ValidAudience = jwtSettings["Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings["Secret"]!)
                    ),
                };
            });
        services
            .AddIdentityCore<AppUser>(op =>
            {
                op.Password.RequiredLength = 6;
                op.Password.RequireDigit = false;
                op.Password.RequireNonAlphanumeric = false;
                op.Password.RequireUppercase = false;
                op.Password.RequireLowercase = false;
                op.Password.RequiredUniqueChars = 1;
                op.SignIn.RequireConfirmedAccount = false;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>();

        services.AddHybridCache(op =>
        {
            op.DefaultEntryOptions = new Microsoft.Extensions.Caching.Hybrid.HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromMinutes(10),
                LocalCacheExpiration = TimeSpan.FromMinutes(10),
            };
        });
        services.AddScoped<IAuthorizationHandler, AssignLaborHandler>();
        services
            .AddAuthorizationBuilder()
            .AddPolicy("ManagerOnly", policy => policy.RequireRole("Manager"))
            .AddPolicy(
                "SelfScopedWorkOrderAccess",
                policy => policy.Requirements.Add(new AssignLaborRequirement())
            );
        services.AddScoped<IWorkOrderNotifier, SignalRWorkOrderNotifier>();
        services.AddScoped<IInvoicePdfGenerator, InvoicePdfGenerator>();
        services.AddHostedService<OverDueOrdersCleanUpService>();

        return services;
    }
}
