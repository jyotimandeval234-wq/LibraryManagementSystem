
using LibraryManagementSystem.Api.DTOs;
using LibraryManagementSystem.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.Api.Controllers;

[ApiController]
[Route("api/loans")]
[Produces("application/json")]
public class LoansController : ControllerBase
{
    private readonly ILibraryService _service;

    public LoansController(ILibraryService service)
    {
        _service = service;
    }

    // GET: api/loans
    // Retrieve all loan records
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLoans()
    {
        var loans = await _service.GetLoans();

        return Ok(new
        {
            success = true,
            totalLoans = loans.Count(),
            activeLoans = loans.Count(x => x.ReturnDate == null),
            returnedLoans = loans.Count(x => x.ReturnDate != null),
            overdueLoans = loans.Count(x =>
                x.ReturnDate == null &&
                x.DueDate.Date < DateTime.Today),
            data = loans
        });
    }

    // POST: api/loans
    // Create a new loan
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Borrow(
        [FromBody] BorrowRequest request)
    {
        try
        {
            var loan = await _service.Borrow(request);

            return Ok(new
            {
                success = true,
                message = "Book borrowed successfully.",
                data = loan
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                success = false,
                message = ex.Message
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                success = false,
                message = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    // POST: api/loans/{id}/return
    // Return a borrowed book
    [HttpPost("{id:int}/return")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Return(
        int id,
        [FromBody] ReturnRequest? request)
    {
        try
        {
            var loan = await _service.Return(
                id,
                request?.ReturnDate);

            if (loan == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = $"Loan with ID {id} was not found."
                });
            }

            return Ok(new
            {
                success = true,
                message = "Book returned successfully.",
                data = loan
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                success = false,
                message = ex.Message
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                success = false,
                message = ex.Message
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    // POST: api/loans/{id}/renew
    // Renew an active loan
    [HttpPost("{id:int}/renew")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Renew(int id)
    {
        try
        {
            var loan = await _service.Renew(id);

            if (loan == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = $"Loan with ID {id} was not found."
                });
            }

            return Ok(new
            {
                success = true,
                message = "Loan renewed successfully.",
                data = loan
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                success = false,
                message = ex.Message
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                success = false,
                message = ex.Message
            });
        }
    }


    [HttpPost("{id:int}/pay-fee")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PayFee(int id)
    {
        try
        {
            var loan = await _service.PayFee(id);

            if (loan == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = $"Loan with ID {id} was not found."
                });
            }

            return Ok(new
            {
                success = true,
                message = "Late fee paid successfully.",
                data = loan
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                success = false,
                message = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

}
