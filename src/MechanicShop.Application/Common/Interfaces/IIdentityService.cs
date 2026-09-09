using MechanicShop.Application.Features.Identity.IdentityDtos;
using MechanicShop.Domain.Common.Results;

namespace MechanicShop.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<bool> IsInRoleAsync(string userId, string role, CancellationToken token);
    Task<bool> AuthorizeAsync(string userId, string? policyName, CancellationToken token);
    Task<Result<AppUserDto>> AuthenticateAsync(
        string email,
        string password,
        CancellationToken token
    );

    Task<Result<AppUserDto>> GetUserByIdAsync(string userId, CancellationToken token);
    Task<string?> GetUserNameAsync(string userId, CancellationToken token);
}
