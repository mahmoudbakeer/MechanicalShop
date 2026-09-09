using MechanicShop.Domain.Common.Results;

namespace MechanicShop.Domain.RepairTasks;


public static class RepairTaskError
{
    public static Error RepairTaskIdRequired => Error.Validation("RepairTask_Id_Required.", "Id cannot be null or empty.");

    public static Error NameRequired => Error.Validation("RepairTask_Name_Required.", "Name cannot be null or empty.");
    public static Error LaborCostInValid => Error.Validation("RepairTask_LaborCost_InValid.", "LaborCost must be between 1$ and 10,000$.");
    public static Error DurationRequired => Error.Validation("RepairTask_Duration_Required.", "RepairTask Duration cannot be empty or null.");
    public static Error PartsRequired => Error.Validation("RepairTask_Parts_Required.", "RepairTask Parts cannot be empty or null.");
    public static Error InUse => Error.Validation("RepairTask_InUse.", "Cannot Delete RepairTask that is in Use.");
    public static Error DuplicateName => Error.Validation("RepairTask_Parts_DuplicateName.", "There is already part has the same name in the parts of the RepairTask.");
    public static Error RepairTaskAlreadyExist => Error.Validation("RepairTask_Already_Exist.", "RepairTask with the same name already exist.");
}