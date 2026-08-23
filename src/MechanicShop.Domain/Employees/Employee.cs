using MechanicShop.Domain.Common;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.Employees.Enum;

namespace MechanicShop.Domain.Employees;


public class Employee : AuditableEntity
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string FullName => $"{FirstName} {LastName}";

    public Role Role { get; private set; }
#pragma warning disable CS8618
    private Employee() { }
#pragma warning disable CS8618

    private Employee(Guid id, string firstName, string lastName, Role role) : base(id)
    {
        FirstName = firstName;
        LastName = lastName;
        Role = role;
    }


    public static Result<Employee> Create(Guid id, string firstName, string lastName, Role role)
    {
        if (Guid.Empty == id) return EmployeeError.EmployeeIdRequired;

        if (string.IsNullOrEmpty(firstName)) return EmployeeError.FirstNameRequired;
        if (string.IsNullOrEmpty(lastName)) return EmployeeError.LastNameRequired;
        if (!System.Enum.IsDefined(typeof(Role), role)) return EmployeeError.RoleInValid;

        return new Employee(id, firstName, lastName, role);
    }

    public Result<Updated> Update(string firstName, string lastName, Role role)
    {
        if (string.IsNullOrEmpty(firstName)) return EmployeeError.FirstNameRequired;
        if (string.IsNullOrEmpty(lastName)) return EmployeeError.LastNameRequired;
        if (!System.Enum.IsDefined(typeof(Role), role)) return EmployeeError.RoleInValid;

        FirstName = firstName;
        LastName = lastName;
        Role = role;
        return Result.Updated;
    }
}