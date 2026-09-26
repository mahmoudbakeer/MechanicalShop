namespace MechanicShop.Contracts.Requests.Customers;

public class CreateCustomerRequest
{
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;
    public List<CreateVehicleRequest> Vehicle { get; set; } = default!;
}

public class CreateVehicleRequest
{
    public string Make { get; set; } = default!;
    public string Model { get; set; } = default!;
    public int Year { get; set; }
    public string LicensePlate { get; set; } = default!;
}
