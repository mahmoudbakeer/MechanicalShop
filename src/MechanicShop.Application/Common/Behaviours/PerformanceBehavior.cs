using System.Diagnostics;
using MechanicShop.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Common.Behaviours;

public class PerformanceBehavior<TRequest, TResponse>(
    Stopwatch stopwatch,
    ILogger<PerformanceBehavior<TRequest, TResponse>> logger,
    IIdentityService identityService,
    IUser user
) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly Stopwatch _timer = stopwatch;
    private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger = logger;
    private readonly IIdentityService _identityService = identityService;
    private readonly IUser _user = user;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        _timer.Start();

        var response = await next(cancellationToken);

        _timer.Stop();

        var elapsedMilliseconds = _timer.ElapsedMilliseconds;

        if (elapsedMilliseconds > 500)
        {
            string? requestName = typeof(TRequest).Name;
            string? userId = _user.UserId ?? string.Empty;
            string? userName = string.Empty;

            if (!string.IsNullOrEmpty(userId))
            {
                userName = await _identityService.GetUserNameAsync(userId, cancellationToken);
            }

            _logger.LogInformation(
                "Long Running Request: {Name} {@UserId} {@UserName} {@Request}",
                requestName,
                userId,
                userName,
                request
            );
        }

        return response;
    }
}
