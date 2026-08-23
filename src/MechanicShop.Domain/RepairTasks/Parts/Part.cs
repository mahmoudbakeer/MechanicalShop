using MechanicShop.Domain.Common;
using MechanicShop.Domain.Common.Results;

namespace MechanicShop.Domain.RepairTasks.Parts;


public class Part : AuditableEntity
{
    public string Name { get; private set; }
    public int Quantity { get; private set; }
    public decimal Cost { get; private set; }


#pragma warning disable CS8618
    private Part() { }
#pragma warning disable CS8618


    private Part(Guid id, string name, int quantity, decimal cost) : base(id)
    {
        Name = name;
        Quantity = quantity;
        Cost = cost;
    }

    public static Result<Part> Create(Guid id, string name, int quantity, decimal cost)
    {
        if (Guid.Empty == id) return PartError.PartIdRequired;
        if (string.IsNullOrEmpty(name)) return PartError.NameRequired;

        if (quantity < 1 || quantity > 10) return PartError.QuantityInValid;
        if (cost < 1 || cost > 10000) return PartError.CostInValid;

        return new Part(id, name, quantity, cost);
    }
    public Result<Updated> Update(string name, int quantity, decimal cost)
    {

        if (string.IsNullOrEmpty(name)) return PartError.NameRequired;

        if (quantity < 1 || quantity > 10) return PartError.QuantityInValid;
        if (cost < 1 || cost > 10000) return PartError.CostInValid;


        Name = name;
        Quantity = quantity;
        Cost = cost;
        return Result.Updated;
    }


}