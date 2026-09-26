using MechanicShop.Domain.WorkOrders.Events;

namespace MechanicShop.Application.Features.Scheduling.SchedulingDtos;



public sealed class SpotDto
{
    public Spot Spot { get; set; }
    public List<AvailabilitySlotDto> Slots { get; set; } = [];
}