using MechanicShop.Application.Features.Labors.LaborDtos;
using MechanicShop.Domain.Employees;

namespace MechanicShop.Application.Features.Labors.Mappers;

public static class EmployeeMapper
{
    public static LaborDto ToDto(this Employee employee)
    {
        return new LaborDto
        {
            Id = employee.Id,
            Name = $"{employee.FirstName} {employee.LastName}",
        };
    }

    public static List<LaborDto> ToDto(this IEnumerable<Employee> employees)
    {
        return [.. employees.Select(e => e.ToDto())];
    }
}
