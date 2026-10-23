
using LibraryManagementSystem.Api.DTOs;
using LibraryManagementSystem.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.Api.Controllers;

[ApiController]
[Route("api/books")]
public class BooksController : ControllerBase
{
    private readonly ILibraryService _service;

    public BooksController(ILibraryService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResponse<BookListItem>>> GetBooks(
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _service.GetBooks(search, page, pageSize);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetBook(int id)
    {
        var book = await _service.GetBook(id);

        if (book == null)
            return NotFound(new { message = "Book not found." });

        return Ok(book);
    }

    [HttpPost]
    public async Task<IActionResult> CreateBook(
        [FromBody] CreateBookRequest request)
    {
        var book = await _service.CreateBook(request);

        return CreatedAtAction(
            nameof(GetBook),
            new { id = book.Id },
            book);
    }
}