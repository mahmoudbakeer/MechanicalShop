using MediatR;

namespace MechanicShop.Application.Features.Customers.Commands.UpdateCustomer;

public sealed record UpdateVehicleCommand(
    Guid Id,
    string Make,
    string Model,
    int Year,
    string LicensePlate
) : IRequest;
