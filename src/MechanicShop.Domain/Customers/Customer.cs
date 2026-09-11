using System.Text.RegularExpressions;
using MechanicShop.Domain.Common;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.Customers.Vehicles;

namespace MechanicShop.Domain.Customers;

public class Customer : AuditableEntity
{
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string PhoneNumber { get; private set; }
    private readonly List<Vehicle> _vehicles = [];
    public IEnumerable<Vehicle> Vehicles => _vehicles.AsReadOnly(); // so no one can recover the List<> and clear it or modify it.
#pragma warning disable CS8618
    private Customer() { }

    private Customer(Guid id, string name, string email, string phonenumber, List<Vehicle> vehicles)
        : base(id)
    {
        Name = name;
        Email = email;
        PhoneNumber = phonenumber;
        _vehicles = vehicles;
    }

    public static Result<Customer> Create(
        Guid id,
        string name,
        string email,
        string phoneNumber,
        List<Vehicle> vehicles
    )
    {
        if (Guid.Empty == id)
            return CustomerError.CustomerIdRequired;

        if (string.IsNullOrEmpty(name))
            return CustomerError.NameRequired;
        if (string.IsNullOrEmpty(phoneNumber))
            return CustomerError.PhoneNumberRequired;
        if (!phoneNumber.StartsWith("+") || phoneNumber.Count() > 15 || phoneNumber.Count() < 7)
            return CustomerError.PhoneNumberInvalid;
        if (string.IsNullOrEmpty(email))
            return CustomerError.EmailRequired;
        if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            return CustomerError.EmailInValid;
        return new Customer(id, name, email, phoneNumber, [.. vehicles]);
    }

    public Result<Updated> Update(string name, string email, string phonenumber)
    {
        if (string.IsNullOrEmpty(name))
            return CustomerError.NameRequired;
        if (string.IsNullOrEmpty(phonenumber))
            return CustomerError.PhoneNumberRequired;
        if (!phonenumber.StartsWith("+") || phonenumber.Count() > 15 || phonenumber.Count() < 7)
            return CustomerError.PhoneNumberInvalid;
        if (string.IsNullOrEmpty(email))
            return CustomerError.EmailRequired;
        if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            return CustomerError.EmailInValid;

        Name = name;
        Email = email;
        PhoneNumber = phonenumber;
        return Result.Updated;
    }

    public Result<Updated> UpsertVehicles(List<Vehicle> vehicles)
    {
        _vehicles.RemoveAll(existing => vehicles.All(v => v.Id != existing.Id)); // remove all the vehicles not present in the new incoming vehicles list.

        foreach (var incoming in vehicles)
        {
            var exist = _vehicles.FirstOrDefault(v => v.Id == incoming.Id);

            if (exist is null)
            {
                _vehicles.Add(incoming);
            }
            else
            {
                var updatedvehicleresult = exist.Update(
                    incoming.Make,
                    incoming.LicensePlate,
                    incoming.Model,
                    incoming.Year
                );

                if (updatedvehicleresult.IsError)
                {
                    return updatedvehicleresult.Errors!;
                }
            }
        }
        return Result.Updated;
    }
}
