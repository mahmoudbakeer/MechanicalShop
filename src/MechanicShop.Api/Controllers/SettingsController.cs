using Asp.Versioning;
using MechanicShop.Contracts.Responses;
using MechanicShop.Infrastructure.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace MechanicShop.Api.Controllers;

[Route("api/settings")]
[ApiVersionNeutral]
public class SettingsController(IOptions<AppSettings> options) : ControllerApi
{
    private readonly AppSettings settings = options.Value;

    [HttpGet("operating-hours")]
    public ActionResult<OperatingHoursResponse> GetOperatingHours()
    {
        return Ok(new OperatingHoursResponse(settings.OpeningTime, settings.ClosingTime));
    }
}
