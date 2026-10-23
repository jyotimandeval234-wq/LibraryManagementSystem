

using LibraryManagementSystem.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.Api.Controllers;

[ApiController]
[Route("api/dashboard")]
public class DashboardController : ControllerBase
{
    private readonly ILibraryService _service;

    public DashboardController(ILibraryService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetDashboard()
    {
        var dashboard = await _service.GetDashboard();

        return Ok(dashboard);
    }
}