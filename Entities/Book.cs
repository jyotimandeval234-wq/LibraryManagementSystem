
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Api.Entities;

public class Book
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Title { get; set; } = "";

    public string Description { get; set; } = "";

    [Required, MaxLength(150)]
    public string Author { get; set; } = "";

    [Required, MaxLength(20)]
    public string ISBN { get; set; } = "";

    [Required, MaxLength(80)]
    public string Genre { get; set; } = "";

    public int PublishedYear { get; set; }

    public List<BookCopy> Copies { get; set; } = [];

    public List<Reservation> Reservations { get; set; } = [];
}
