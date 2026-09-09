using System.Security.Claims;
using MechanicShop.Application.Common.Errors;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.Identity.IdentityDtos;
using MechanicShop.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.Identity.Queries.RefreshToken;

public class RefreshTokenQueryHandler(
    ITokenProvider tokenProvider,
    IAppDbContext context,
    ILogger<RefreshTokenQueryHandler> logger,
    IIdentityService identityService,
    TimeProvider timeProvider
) : IRequestHandler<RefreshTokenQuery, Result<TokenResponse>>
{
    private readonly ITokenProvider _tokenProvider = tokenProvider;
    private readonly IAppDbContext _context = context;
    private readonly ILogger<RefreshTokenQueryHandler> _logger = logger;
    private readonly IIdentityService _identityService = identityService;
    private readonly TimeProvider _timeProvider = timeProvider;

    public async Task<Result<TokenResponse>> Handle(
        RefreshTokenQuery request,
        CancellationToken cancellationToken
    )
    {
        ClaimsPrincipal? principal = _tokenProvider.GetPrincipalsFromExpiredToken(
            request.ExpiredAccessToken
        );

        if (principal == null)
        {
            _logger.LogWarning("Invalid expired access token");
            return ApplicationErrors.ExpiredAccessTokenInvalid;
        }

        string? userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
        {
            _logger.LogWarning("User ID not found in expired access token");
            return ApplicationErrors.UserClaimsInvalid;
        }

        Result<AppUserDto> userResult = await _identityService.GetUserByIdAsync(
            userId,
            cancellationToken
        );

        if (userResult.IsError)
        {
            _logger.LogWarning(
                "Get user by Id failed, an error occurred: {Error}",
                userResult.TopError.Description
            );
            return userResult.Errors!;
        }

        var refreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(
            rt => rt.Token == request.RefreshToken && rt.UserId == userId,
            cancellationToken
        );

        if (refreshToken == null || refreshToken.ExpiresAtUtc < _timeProvider.GetUtcNow())
        {
            _logger.LogWarning("Refresh token is invalid or expired");
            return ApplicationErrors.RefreshTokenExpired;
        }
        var tokenResult = await _tokenProvider.GenerateTokenAsync(
            userResult.Value,
            cancellationToken
        );

        if (tokenResult.IsError)
        {
            _logger.LogWarning(
                "Generating token failed, an error occurred : {Error}",
                tokenResult.TopError
            );
            return tokenResult.Errors!;
        }

        return tokenResult.Value;
    }
}
