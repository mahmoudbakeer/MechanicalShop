namespace MechanicShop.Application.Features.Scheduling.SchedulingDtos;



public sealed class ScheduleDto
{
    public List<SpotDto> Spots { get; set; } = [];
    public DateOnly Date { get; set; }
    public bool EndOfDay { get; set; }
}