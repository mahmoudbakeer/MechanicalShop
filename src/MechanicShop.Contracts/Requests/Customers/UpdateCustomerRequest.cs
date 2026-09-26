namespace MechanicShop.Contracts.Requests.Customers;

public class UpdateCustomerRequest
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;
    public List<UpdateVehicleRequest> Vehicle { get; set; } = default!;
}

public class UpdateVehicleRequest
{
    public Guid Id { get; set; }
    public string Make { get; set; } = default!;
    public string Model { get; set; } = default!;
    public int Year { get; set; }
    public string LicensePlat { get; set; } = default!;
}
