using MechanicShop.Application.Features.Commands.Customers.CustomerDtos;
using MechanicShop.Domain.Common.Results;
using MediatR;

namespace MechanicShop.Application.Features.Customers.Commands.CreateCustomer;

public sealed record CreateCustomerCommand(
    string Name,
    string Email,
    string PhoneNumber,
    List<CreateVehicleCommand> Vehicles
) : IRequest<Result<CustomerDto>>;
