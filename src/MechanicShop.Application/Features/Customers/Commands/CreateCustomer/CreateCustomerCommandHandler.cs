using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.Commands.Customers.CustomerDtos;
using MechanicShop.Application.Features.Customers.Mappers;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.Customers;
using MechanicShop.Domain.Customers.Vehicles;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.Customers.Commands.CreateCustomer;

public class CreateCustomerCommandHandler(
    IAppDbContext context,
    ILogger<CreateCustomerCommandHandler> logger,
    HybridCache cache
) : IRequestHandler<CreateCustomerCommand, Result<CustomerDto>>
{
    private readonly IAppDbContext _context = context;
    private readonly ILogger<CreateCustomerCommandHandler> _logger = logger;
    private readonly HybridCache _cache = cache;

    public async Task<Result<CustomerDto>> Handle(
        CreateCustomerCommand request,
        CancellationToken cancellationToken
    )
    {
        var email = request.Email.Trim().ToLower();
        var exist = await _context.Customers.FirstOrDefaultAsync(
            c => c.Email.ToLower() == email,
            cancellationToken: cancellationToken
        );

        if (exist is not null)
        {
            _logger.LogWarning("Customer creation aborted. Email is already exist.");

            return CustomerError.CustomerExist;
        }

        var vehicles = new List<Vehicle>() { };
        foreach (var v in request.Vehicles)
        {
            var result = Vehicle.Create(Guid.NewGuid(), v.Make, v.LicensePlate, v.Model, v.Year);

            if (result.IsError)
            {
                return result.Errors!;
            }

            vehicles.Add(result.Value);
        }
        var customerResult = Customer.Create(
            Guid.NewGuid(),
            request.Name.Trim(),
            request.Email.Trim().ToLower(),
            request.PhoneNumber.Trim(),
            vehicles
        );
        if (customerResult.IsError)
        {
            return customerResult.Errors!;
        }
        var customer = customerResult.Value;
        _context.Customers.Add(customer);

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation(
            "Customer created with id '{CustomerId}' successfully.",
            customer.Id
        );
        await _cache.RemoveByTagAsync("customer");
        return customer.ToDto();
    }
}
