using MechanicShop.Application.Common.Interfaces;
using Microsoft.IdentityModel.JsonWebTokens;

namespace MechanicShop.Api.Services;

public class CurrentUser(IHttpContextAccessor httpContext) : IUser
{
    public string? UserId =>
        httpContext.HttpContext?.User?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
}
