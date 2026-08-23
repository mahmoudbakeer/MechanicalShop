using MechanicShop.Domain.Common.Results;

namespace MechanicShop.Domain.RepairTasks.Parts;

public static class PartError
{
    public static Error PartIdRequired => Error.Validation("Part_Id_Required.", "Id cannot be null or empty.");

    public static Error NameRequired => Error.Validation("Part_Name_Required.", "Name cannot be null or empty.");
    public static Error CostInValid => Error.Validation("Part_Cost_InValid.", "Cost must be between 1$ and 10,000$.");
    public static Error QuantityInValid => Error.Validation("Part_Quantity_InValid.", "Quantity must be between 1 and 10.");
}