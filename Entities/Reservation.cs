using LibraryManagementSystem.Api.Entities;

namespace LibraryManagementSystem.Api.Entities;

public enum ReservationStatus
{
    Waiting = 0,
    Fulfilled = 1,
    Cancelled = 2
}

public class Reservation
{
    public int Id { get; set; }

    public int BookId { get; set; }

    public int MemberId { get; set; }

    public DateTime CreatedAt { get; set; }

    public ReservationStatus Status { get; set; }
        = ReservationStatus.Waiting;

    public Book Book { get; set; } = null!;

    public Member Member { get; set; } = null!;
}