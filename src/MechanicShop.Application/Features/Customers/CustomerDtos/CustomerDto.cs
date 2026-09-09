using MechanicShop.Application.Features.Commands.Customers.CustomerDtos.Vehicle;

namespace MechanicShop.Application.Features.Commands.Customers.CustomerDtos;

public class CustomerDto
{
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public List<VehicleDto> Vehicles { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;
    public Guid Id { get; set; }
}
