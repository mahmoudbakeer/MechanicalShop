using System.Security.Cryptography.X509Certificates;
using MechanicShop.Domain.Common;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.RepairTasks.Enum;
using MechanicShop.Domain.RepairTasks.Parts;

namespace MechanicShop.Domain.RepairTasks;

public class RepairTask : AuditableEntity
{
    public string Name { get; private set; }
    public decimal LaborCost { get; private set; }
    public RepairTaskDuration EstimatedDuration { get; private set; }
    private readonly List<Part> _parts = [];
    public IEnumerable<Part> Parts => _parts.AsReadOnly(); // so no one can modify the list of parts or clear it.
    public decimal TotalCost => _parts.Sum(p => p.Cost * p.Quantity) + LaborCost;

#pragma warning disable CS8618
    private RepairTask() { }
#pragma warning disable CS8618


    private RepairTask(Guid id, string name, decimal laborCost, RepairTaskDuration estimatedDuration, List<Part> parts) : base(id)
    {
        Name = name;
        LaborCost = laborCost;
        EstimatedDuration = estimatedDuration;
        _parts = parts;
    }

    public static Result<RepairTask> Create(Guid id, string name, decimal laborCost, RepairTaskDuration estimatedDuration, List<Part> parts)
    {
        if (Guid.Empty == id) return RepairTaskError.RepairTaskIdRequired;

        if (string.IsNullOrEmpty(name))
            return RepairTaskError.NameRequired;
        if (laborCost < 1 || laborCost > 10000)
            return RepairTaskError.LaborCostInValid;
        if (!System.Enum.IsDefined(typeof(RepairTaskDuration), estimatedDuration))
            return RepairTaskError.DurationRequired;
        if (!parts.Any()) return RepairTaskError.PartsRequired;
        return new RepairTask(id, name.Trim(), laborCost, estimatedDuration, parts);
    }
    public Result<Updated> Update(string name, decimal laborCost, RepairTaskDuration estimatedDuration)
    {
        if (string.IsNullOrEmpty(name))
            return RepairTaskError.NameRequired;
        if (laborCost < 1 || laborCost > 10000)
            return RepairTaskError.LaborCostInValid;
        if (!System.Enum.IsDefined(typeof(RepairTaskDuration), estimatedDuration))
            return RepairTaskError.DurationRequired;

        Name = name.Trim();
        LaborCost = laborCost;

        return Result.Updated;
    }
    public Result<Updated> UpsertParts(List<Part> parts)
    {
        _parts.RemoveAll(existing => parts.All(v => v.Id != existing.Id));
        foreach (var incoming in parts)
        {
            var exist = _parts.FirstOrDefault(v => v.Id == incoming.Id);

            if (exist is null)
            {
                _parts.Add(incoming);
            }
            else
            {
                var updatedpartresult = exist.Update(incoming.Name, incoming.Quantity, incoming.Cost);

                if (updatedpartresult.IsError)
                {
                    return updatedpartresult.Errors!;
                }
            }
        }
        return Result.Updated;
    }

}