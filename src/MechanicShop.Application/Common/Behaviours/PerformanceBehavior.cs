using System.Diagnostics;
using MechanicShop.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Common.Behaviours;

public class PerformanceBehavior<TRequest, TResponse>(
    ILogger<PerformanceBehavior<TRequest, TResponse>> logger,
    IUser user
) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger = logger;
    private readonly IUser _user = user;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        var timer = Stopwatch.StartNew();

        var response = await next(cancellationToken);

        timer.Stop();

        if (timer.ElapsedMilliseconds > 500)
        {
            string? requestName = typeof(TRequest).Name;
            string? userId = _user.UserId ?? string.Empty;

            _logger.LogInformation(
                "Long Running Request: {Name} {@UserId} {@Request}",
                requestName,
                userId,
                request
            );
        }
        return response;
    }
}
