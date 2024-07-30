using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AuthBack.Service.Attributes;

namespace Test.Test.WebApi.Controllers.WorkSchedules;

[ApiController]
[ApiVersion("1.0")]
[Route("api/cd/v{version:apiVersion}/employees/{id}/schedule")]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
[Produces("application/json")]
public class WorkScheduleByEmployeeController : ResourceBasedAuthSupportController
{
    private readonly ICalcWorkScheduleService workScheduleService;
    private readonly IEmployeeService employeeService;
    private readonly IMapper mapper;

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ApiProblemDetails))]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ApiProblemDetails))]
    public async Task<ActionResult<WorkScheduleDto>> Get([FromRoute] uint employeeId,
        [FromQuery] DateRangeFilterDto filterDto, CancellationToken ct)
    {
        var employee = await employeeService.GetAsync(employeeId, ct);
        if (employee is null)
            return NotFound();

        if (!await AllowedAsync(employee))
            return Forbid();
        
        var workSchedule = await workScheduleService.GetByEmployeeAsync(employeeId, filterDto.FromDate, filterDto.TillDate, ct);

        return mapper.Map<WorkScheduleDto>(workSchedule);
    }
}