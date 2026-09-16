using System.Security.Claims;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Common.Utiltiy;
using MechanicShop.Application.Features.Identity.IdentityDtos;
using MechanicShop.Domain.Common.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace MechanicShop.Infrastructure.Identity;

public class IdenetityService(
    UserManager<AppUser> userManager,
    IUserClaimsPrincipalFactory<AppUser> userClaimsPrincipalFactory,
    IAuthorizationService authorizationService
) : IIdentityService
{
    private readonly UserManager<AppUser> _userManager = userManager;
    private readonly IUserClaimsPrincipalFactory<AppUser> _userClaimsPrincipalFactory =
        userClaimsPrincipalFactory;
    private readonly IAuthorizationService _authorizationService = authorizationService;

    public async Task<Result<AppUserDto>> AuthenticateAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
        {
            return Error.NotFound(
                "User_Not_Found",
                $"User with email {UtilityService.MaskEmail(email)} not found"
            );
        }

        if (!user.EmailConfirmed)
        {
            return Error.UnAuthorized(
                "Email_Not_Confirmed",
                $"Email {UtilityService.MaskEmail(email)} is not confirmed"
            );
        }

        if (!await _userManager.CheckPasswordAsync(user, password))
        {
            return Error.UnAuthorized(
                "Invalid_Credentials",
                $"Invalid credentials for user wit email {UtilityService.MaskEmail(email)}"
            );
        }

        return new AppUserDto(
            user.Id,
            user.Email!,
            await _userManager.GetRolesAsync(user),
            await _userManager.GetClaimsAsync(user)
        );
    }

    public async Task<bool> AuthorizeAsync(string userId, string policyName)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return false;
        }
        ClaimsPrincipal pricipal = await _userClaimsPrincipalFactory.CreateAsync(user);

        AuthorizationResult result = await _authorizationService.AuthorizeAsync(
            pricipal,
            policyName
        );

        return result.Succeeded;
    }

    public async Task<Result<AppUserDto>> GetUserByIdAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
        {
            return Error.NotFound("User_Not_Found", $"User with id {userId} not found");
        }
        else
            return new AppUserDto(
                user.Id,
                user.Email!,
                await _userManager.GetRolesAsync(user),
                await _userManager.GetClaimsAsync(user)
            );
    }

    public async Task<string?> GetUserNameAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        return user?.UserName;
    }

    public async Task<bool> IsInRoleAsync(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);
        return user is not null && await _userManager.IsInRoleAsync(user, role);
    }
}
