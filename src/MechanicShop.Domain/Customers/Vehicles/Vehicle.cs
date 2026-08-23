using System.Security.Cryptography.X509Certificates;
using MechanicShop.Domain.Common;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.Customers;
namespace MechanicShop.Domain.Customers.Vehicles;


public class Vehicle : AuditableEntity
{
    public Guid CustomerId { get; }
    public Customer Customer { get; } = null!;

    public int Year { get; private set; }
    public string Make { get; private set; }
    public string Model { get; private set; }
    public string LicensePlate { get; private set; }

    public string VehicleInfo => $"{Make} | {Model} | {Year}";
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    private Vehicle()
    { }

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    private Vehicle(Guid id, string make, string licenseplate, string model, int year) : base(id)
    {
        Make = make;
        Model = model;
        Year = year;
        LicensePlate = licenseplate;
    }


    public static Result<Vehicle> Create(Guid id, string make, string licenseplate, string model, int year)
    {
        if (Guid.Empty == id) return VehicleError.VehicleIdRequired;

        if (string.IsNullOrEmpty(make))
        {
            return VehicleError.MakeRequired;
        }
        if (string.IsNullOrEmpty(model))
        {
            return VehicleError.ModelRequired;
        }
        if (string.IsNullOrEmpty(licenseplate))
        {
            return VehicleError.LicensePlateRequired;
        }
        if (year < 1886 || year > DateTime.UtcNow.AddYears(1).Year)
        {
            return VehicleError.YearInvalid;
        }
        return new Vehicle(id, make, licenseplate, model, year);
    }
    public Result<Updated> Update(string make, string licenseplate, string model, int year)
    {
        if (string.IsNullOrEmpty(make))
        {
            return VehicleError.MakeRequired;
        }
        if (string.IsNullOrEmpty(model))
        {
            return VehicleError.ModelRequired;
        }
        if (string.IsNullOrEmpty(licenseplate))
        {
            return VehicleError.LicensePlateRequired;
        }
        if (year < 1886 || year > DateTime.UtcNow.AddYears(1).Year)
        {
            return VehicleError.YearInvalid;
        }
        Make = make;
        LicensePlate = licenseplate;
        Model = model;
        Make = make;
        return Result.Updated;
    }
}