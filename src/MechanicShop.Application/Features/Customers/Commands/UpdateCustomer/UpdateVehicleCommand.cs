using MechanicShop.Application.Features.Commands.Customers.CustomerDtos.Vehicle;
using MechanicShop.Domain.Common.Results;
using MediatR;

namespace MechanicShop.Application.Features.Customers.Commands.UpdateCustomer;

public sealed record UpdateVehicleCommand(
    Guid Id,
    string Make,
    string Model,
    int Year,
    string LicensePlate
) : IRequest;
