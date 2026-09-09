using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Domain.Common.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.Identity.Queries.GenerateToken;

public class GenerateTokenQueryHandler(
    ILogger<GenerateTokenQueryHandler> logger,
    ITokenProvider tokenProvider,
    IIdentityService identityService
) : IRequestHandler<GenerateTokenQuery, Result<TokenResponse>>
{
    private readonly ILogger<GenerateTokenQueryHandler> _logger = logger;
    private readonly ITokenProvider _tokenProvider = tokenProvider;
    private readonly IIdentityService _identityService = identityService;

    public async Task<Result<TokenResponse>> Handle(
        GenerateTokenQuery request,
        CancellationToken cancellationToken
    )
    {
        var authResult = await _identityService.AuthenticateAsync(
            request.Email,
            request.Password,
            cancellationToken
        );

        if (authResult.IsError)
            return authResult.Errors!;

        var generationResult = await _tokenProvider.GenerateTokenAsync(
            authResult.Value!,
            cancellationToken
        );

        if (generationResult.IsError)
        {
            _logger.LogError(
                "Generate token error occurred: {ErrorDescription}",
                generationResult.TopError.Description
            );
            return generationResult.Errors!;
        }

        return generationResult.Value!;
    }
}
