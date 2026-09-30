using MechanicShop.Api;
using MechanicShop.Api.Extensions;
using MechanicShop.Application;
using MechanicShop.Infrastructure;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
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
    app.MapOpenApi().WithDocumentPerVersion();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("openapi/json.v1", "MechanicalShop Api v1");
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
