using MechanicShop.Application.Common.Errors;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.Customers.Vehicles;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.Customers.Commands.UpdateCustomer;

public class UpdateCustomerCommandHandler(
    IAppDbContext context,
    ILogger<UpdateCustomerCommandHandler> logger,
    HybridCache cache
) : IRequestHandler<UpdateCustomerCommand, Result<Updated>>
{
    private readonly IAppDbContext _context = context;
    private readonly ILogger<UpdateCustomerCommandHandler> _logger = logger;
    private readonly HybridCache _cache = cache;

    public async Task<Result<Updated>> Handle(
        UpdateCustomerCommand request,
        CancellationToken cancellationToken
    )
    {
        var customer = await _context.Customers.FindAsync(request.Id, cancellationToken);

        if (customer is null)
        {
            _logger.LogWarning("Customer with id '{CustomerId}' does not exist.", request.Id);
            return ApplicationErrors.CustomerNotFound;
        }

        var customerResult = customer.Update(request.Name, request.Email, request.PhoneNumber);

        if (customerResult.IsError)
        {
            _logger.LogWarning("Updation of customer failed, an error has occured.");
            return customerResult.Errors!;
        }

        var vehicles = new List<Vehicle>() { };

        foreach (var v in request.Vehicles)
        {
            var vehicleId = v.Id == Guid.Empty ? Guid.NewGuid() : v.Id;
            var vehicleResult = Vehicle.Create(vehicleId, v.Make, v.LicensePlate, v.Model, v.Year);

            if (vehicleResult.IsError)
            {
                _logger.LogWarning("Updation of customer failed, an error has occured.");
                return vehicleResult.Errors!;
            }
        }

        var VehiclesResult = customer.UpsertVehicles(vehicles);

        if (VehiclesResult.IsError)
        {
            _logger.LogWarning("Updation of customer failed, an error has occured.");
            return VehiclesResult.Errors!;
        }

        await _context.SaveChangesAsync(cancellationToken);
        await _cache.RemoveByTagAsync("customer", cancellationToken);
        return Result.Updated;
    }
}
