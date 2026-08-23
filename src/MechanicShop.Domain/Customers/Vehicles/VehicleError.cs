using MechanicShop.Domain.Common.Results;

namespace MechanicShop.Domain.Customers.Vehicles;


public static class VehicleError
{
    public static Error VehicleIdRequired => Error.Validation("Vehicle_Id_Required.", "Id cannot be null or empty.");

    public static Error MakeRequired => Error.Validation("Vehicle_Make_Required", "The Make cannot be null or empty.");
    public static Error ModelRequired => Error.Validation("Vehicle_Model_Required", "The Model cannot be null or empty.");
    public static Error LicensePlateRequired => Error.Validation("Vehicle_LicensePlate_Required", "The LicensePlate cannot be null or empty.");
    public static Error YearInvalid => Error.Validation("Vehicle_Year_Invalid", "The Year is invalid must be between 1886 and next year.");
}