using System.Runtime.Intrinsics;
using MechanicShop.Api;
using MechanicShop.Api.Extensions;
using MechanicShop.Application;
using MechanicShop.Infrastructure;
using QuestPDF.Infrastructure;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
QuestPDF.Settings.License = LicenseType.Community;
builder
    .Services.AddPresentation(builder.Configuration)
    .AddApplication()
    .AddInfrastructure(builder.Configuration);
builder.Host.UseSerilog(
    (context, loggerConfig) =>
    {
        loggerConfig.ReadFrom.Configuration(context.Configuration);
    }
);
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "MechanicalShop Api v1");
        options.EnableDeepLinking();
        options.DisplayRequestDuration();
        options.EnableFilter();
    });
    await app.InitialiseDatabaseAsync();
}
else
{
    app.UseHsts();
}
app.UseCoreMiddlewares(builder.Configuration);
app.MapControllers();
app.Run();
