using MechanicShop.Domain.Common.Results;

namespace MechanicShop.Domain.Employees;


public static class EmployeeError
{
    public static Error EmployeeIdRequired => Error.Validation("Employee_Id_Required.", "Id cannot be null or empty.");
    public static Error FirstNameRequired => Error.Validation("Employee_FirstName_Required.", "FirstName cannot be null or empty.");
    public static Error LastNameRequired => Error.Validation("Employee_LastName_Required.", "LastName cannot be null or empty.");
    public static Error RoleRequired => Error.Validation("Employee_Role_Required.", "Role cannot be null or empty.");
    public static Error RoleInValid => Error.Validation("Employee_Role_InValid.", "Employee Role InValid.");
}