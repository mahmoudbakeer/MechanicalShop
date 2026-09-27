using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace MechanicShop.Api.OpenApi.Transformers;

public sealed class VersioningTransformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync(
        OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken
    )
    {
        var version = context.DocumentName;
        document.Info.Version = version;
        document.Info.Title = $"MechanicalShop API {version}";

        return Task.CompletedTask;
    }
}
