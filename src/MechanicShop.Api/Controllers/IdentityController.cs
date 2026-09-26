using System.Security.Claims;
using Asp.Versioning;
using MechanicShop.Application.Features.Identity;
using MechanicShop.Application.Features.Identity.IdentityDtos;
using MechanicShop.Application.Features.Identity.Queries.GenerateToken;
using MechanicShop.Application.Features.Identity.Queries.GetUserById;
using MechanicShop.Application.Features.Identity.Queries.RefreshToken;
using MechanicShop.Contracts.Requests.Identity;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MechanicShop.Api.Controllers;

[Route("identity")]
[ApiVersionNeutral]
public class IdentityController(ISender sender) : ControllerApi
{
    [HttpPost("token/generate")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    [EndpointSummary("Generate Access Token.")]
    [EndpointDescription("Generate an access token for user using its email and password.")]
    [EndpointName("GenerateToken")]
    public async Task<ActionResult<TokenResponse>> GenerateToken(
        [FromBody] GenerateTokenRequest request,
        CancellationToken ct
    )
    {
        var result = await sender.Send(new GenerateTokenQuery(request.Email, request.Password), ct);

        return result.Match(Ok, Problem!);
    }

    [HttpPost("token/refresh-token")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    [EndpointSummary("Refresh Access Token.")]
    [EndpointDescription("Refresh an access token for user if its valid.")]
    [EndpointName("RefreshToken")]
    public async Task<ActionResult<TokenResponse>> RefreshToken(
        [FromBody] RefreshTokenRequest request,
        CancellationToken ct
    )
    {
        var result = await sender.Send(
            new RefreshTokenQuery(request.RefreshToken, request.ExpiredAccessToken),
            ct
        );

        return result.Match(Ok, Problem!);
    }

    [HttpGet("current-user/claims")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(AppUserDto), StatusCodes.Status200OK)]
    [EndpointSummary("Retreive UserClaims.")]
    [EndpointDescription("Return Claims of User by user ID.")]
    [EndpointName("GetUser")]
    public async Task<ActionResult<AppUserDto>> GetUser(CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await sender.Send(new GetUserByIdQuery(userId!), ct);
        return result.Match(Ok, Problem!);
    }
}
