

using LibraryManagementSystem.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.Api.Controllers;

[ApiController]
[Route("api/reports")]
public class ReportsController : ControllerBase
{
    private readonly ILibraryService _service;

    public ReportsController(ILibraryService service)
    {
        _service = service;
    }

    [HttpGet("member-activity")]
    public async Task<IActionResult> GetMemberActivity(
        [FromQuery] int year,
        [FromQuery] int month)
    {
        if (year < 1 || year > 9999)
        {
            return BadRequest(new
            {
                message = "Please provide a valid year."
            });
        }

        if (month < 1 || month > 12)
        {
            return BadRequest(new
            {
                message = "Month must be between 1 and 12."
            });
        }

        var report = await _service.GetMemberActivity(year, month);

        return Ok(report);
    }
}