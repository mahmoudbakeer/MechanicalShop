using System.Security.Claims;
using MechanicShop.Application.Features.Identity;
using MechanicShop.Application.Features.Identity.IdentityDtos;
using MechanicShop.Domain.Common.Results;

namespace MechanicShop.Application.Common.Interfaces;

public interface ITokenProvider
{
    Task<Result<TokenResponse>> GenerateTokenAsync(
        AppUserDto user,
        CancellationToken token = default
    );
    ClaimsPrincipal? GetPrincipalsFromExpiredToken(string expiredToken);
}
