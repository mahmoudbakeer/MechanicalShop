using MechanicShop.Application.Features.Commands.Customers.CustomerDtos;
using MechanicShop.Application.Features.Commands.Customers.CustomerDtos.Vehicle;
using MechanicShop.Domain.Customers;
using MechanicShop.Domain.Customers.Vehicles;

namespace MechanicShop.Application.Features.Customers.Mappers;

public static class CustomerMapper
{
    public static CustomerDto ToDto(this Customer customer)
    {
        ArgumentNullException.ThrowIfNull(customer);

        return new CustomerDto
        {
            Id = customer.Id,
            Name = customer.Name,
            Email = customer.Email,
            PhoneNumber = customer.PhoneNumber,
            Vehicles = [.. customer.Vehicles.ToDto()],
        };
    }

    public static VehicleDto ToDto(this Vehicle vehicle)
    {
        ArgumentNullException.ThrowIfNull(vehicle);

        return new VehicleDto
        {
            Make = vehicle.Make,
            Model = vehicle.Model,
            LicensePlate = vehicle.LicensePlate,
            Year = vehicle.Year,
            Id = vehicle.Id,
        };
    }

    public static IEnumerable<VehicleDto> ToDto(this IEnumerable<Vehicle> vehicles)
    {
        return [.. vehicles.Select(v => v.ToDto())];
    }

    public static IEnumerable<CustomerDto> ToDto(this IEnumerable<Customer> customers)
    {
        return [.. customers.Select(c => c.ToDto())];
    }
}
