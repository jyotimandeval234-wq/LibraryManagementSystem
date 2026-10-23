
using LibraryManagementSystem.Api.DTOs;
using LibraryManagementSystem.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.Api.Controllers;

[ApiController]
[Route("api/members")]
public class MembersController : ControllerBase
{
    private readonly ILibraryService _service;

    public MembersController(ILibraryService service)
    {
        _service = service;
    }

    // GET: api/members
    [HttpGet]
    public async Task<IActionResult> GetMembers()
    {
        try
        {
            var members = await _service.GetMembers();

            var result = members.Select(m => new
            {
                m.Id,
                m.Name,
                m.Email,
                m.MembershipType,
                m.IsActive
            });

            return Ok(result);
        }
        catch (Exception)
        {
            return StatusCode(500, new
            {
                message = "An unexpected error occurred while retrieving members."
            });
        }
    }

    // GET: api/members/1
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetMember(int id)
    {
        try
        {
            var member = await _service.GetMember(id);

            if (member == null)
            {
                return NotFound(new
                {
                    message = $"Member with ID {id} was not found."
                });
            }

            return Ok(member);
        }
        catch (Exception)
        {
            return StatusCode(500, new
            {
                message = "An unexpected error occurred while retrieving the member."
            });
        }
    }

    // POST: api/members
    [HttpPost]
    public async Task<IActionResult> CreateMember(
        [FromBody] CreateMemberRequest request)
    {
        try
        {
            var member = await _service.CreateMember(request);

            return CreatedAtAction(
                nameof(GetMember),
                new { id = member.Id },
                new
                {
                    message = "Member created successfully.",
                    data = member
                });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new
            {
                message = ex.Message
            });
        }
        catch (Exception)
        {
            return StatusCode(500, new
            {
                message = "An unexpected error occurred while creating the member."
            });
        }
    }

    // PUT: api/members/1
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateMember(
        int id,
        [FromBody] UpdateMemberRequest request)
    {
        try
        {
            var member = await _service.UpdateMember(id, request);

            if (member == null)
            {
                return NotFound(new
                {
                    message = $"Member with ID {id} was not found."
                });
            }

            return Ok(new
            {
                message = "Member updated successfully.",
                data = member
            });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new
            {
                message = ex.Message
            });
        }
        catch (Exception)
        {
            return StatusCode(500, new
            {
                message = "An unexpected error occurred while updating the member."
            });
        }
    }
}
