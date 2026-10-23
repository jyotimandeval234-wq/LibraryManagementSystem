
using LibraryManagementSystem.Api.DTOs;
using LibraryManagementSystem.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.Api.Controllers;

[ApiController]
[Route("api/reservations")]
public class ReservationsController : ControllerBase
{
    private readonly ILibraryService _service;

    public ReservationsController(ILibraryService service)
    {
        _service = service;
    }


    // GET: api/reservations
    // Retrieve all reservation records
    [HttpGet]
    public async Task<IActionResult> GetReservations()
    {
        var reservations = await _service.GetReservations();

        return Ok(new
        {
            success = true,
            totalReservations = reservations.Count(),
            data = reservations
        });
    }
    // POST: api/reservations
    [HttpPost]
    public async Task<IActionResult> CreateReservation(
        [FromBody] ReservationRequest request)
    {
        try
        {
            var reservation = await _service.Reserve(request);

            return Ok(reservation);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                message = ex.Message
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    // POST: api/reservations/{id}/cancel
    [HttpPost("{id:int}/cancel")]
    public async Task<IActionResult> CancelReservation(int id)
    {
        try
        {
            var cancelled = await _service.CancelReservation(id);

            if (!cancelled)
            {
                return NotFound(new
                {
                    message = "Reservation not found."
                });
            }

            return Ok(new
            {
                message = "Reservation cancelled successfully."
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }
}
