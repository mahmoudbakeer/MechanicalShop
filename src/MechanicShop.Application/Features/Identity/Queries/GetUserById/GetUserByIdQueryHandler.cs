using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.Identity.IdentityDtos;
using MechanicShop.Domain.Common.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.Identity.Queries.GetUserById;

public class GetUserByIdQueryHandler(
    IIdentityService identityService,
    ILogger<GetUserByIdQueryHandler> logger
) : IRequestHandler<GetUserByIdQuery, Result<AppUserDto>>
{
    private readonly IIdentityService _identityService = identityService;
    private readonly ILogger<GetUserByIdQueryHandler> _logger = logger;

    public async Task<Result<AppUserDto>> Handle(
        GetUserByIdQuery request,
        CancellationToken cancellationToken
    )
    {
        Result<AppUserDto>? userResult = await _identityService.GetUserByIdAsync(
            request.UserId!,
            cancellationToken
        );

        if (userResult.IsError)
        {
            _logger.LogWarning(
                "Get User by Id failed, an error occured : {Error}",
                userResult.TopError.Description
            );
            return userResult.Errors!;
        }

        return userResult.Value!;
    }
}
