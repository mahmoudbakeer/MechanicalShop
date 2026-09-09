using MechanicShop.Application.Common.Errors;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.Customers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.Customers.Commands.DeleteCustomer;

public class DeleteCustomerCommandHandler(
    IAppDbContext context,
    ILogger<DeleteCustomerCommandHandler> logger,
    HybridCache cache
) : IRequestHandler<DeleteCustomerCommand, Result<Deleted>>
{
    private readonly IAppDbContext _context = context;
    private readonly ILogger<DeleteCustomerCommandHandler> _logger = logger;
    private readonly HybridCache _cache = cache;

    public async Task<Result<Deleted>> Handle(
        DeleteCustomerCommand request,
        CancellationToken cancellationToken
    )
    {
        var customer = await _context.Customers.FindAsync(request.Id, cancellationToken);

        if (customer is null)
        {
            _logger.LogWarning("Customer deltetion failed, customer does not exist.");

            return ApplicationErrors.CustomerNotFound;
        }
        var hasworkorders = await _context.WorkOrders.AnyAsync(
            wo => wo.Vehicle != null && wo.Vehicle.CustomerId == request.Id,
            cancellationToken
        );
        if (hasworkorders)
        {
            _logger.LogWarning("cannot delete customer that has associated workOrder.");
            return CustomerError.CannotDeleteCustomer;
        }

        _context.Customers.Remove(customer);
        await _context.SaveChangesAsync(cancellationToken);

        await _cache.RemoveByTagAsync("customer", cancellationToken);

        return Result.Deleted;
    }
}
