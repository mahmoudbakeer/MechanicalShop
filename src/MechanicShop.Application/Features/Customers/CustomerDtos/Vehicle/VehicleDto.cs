namespace MechanicShop.Application.Features.Commands.Customers.CustomerDtos.Vehicle;

public class VehicleDto
{
    public Guid Id { get; set; }
    public int Year { get; set; }
    public string Make { get; set; } = default!;
    public string Model { get; set; } = default!;
    public string LicensePlate { get; set; } = default!;
}
