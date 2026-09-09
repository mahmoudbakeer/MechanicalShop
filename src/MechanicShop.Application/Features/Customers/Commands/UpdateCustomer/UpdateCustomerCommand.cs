using MechanicShop.Application.Features.Commands.Customers.CustomerDtos;
using MechanicShop.Domain.Common.Results;
using MediatR;

namespace MechanicShop.Application.Features.Customers.Commands.UpdateCustomer;

public sealed record UpdateCustomerCommand(
    Guid Id,
    string Name,
    string Email,
    string PhoneNumber,
    List<UpdateVehicleCommand> Vehicles
) : IRequest<Result<Updated>>;
