
using System.ComponentModel.DataAnnotations;
using LibraryManagementSystem.Api.Entities;

namespace LibraryManagementSystem.Api.DTOs;

public record CreateBookRequest(
    [Required, MaxLength(200)] string Title,
    [Required, MaxLength(150)] string Author,
    [Required, MaxLength(20)] string ISBN,
    [Required, MaxLength(80)] string Genre,
    [Range(1, 9999)] int PublishedYear,
    [Range(1, 100)] int NumberOfCopies
);

public record CreateMemberRequest(
    [Required, MaxLength(150)] string Name,
    [Required, EmailAddress, MaxLength(254)] string Email,
    MembershipType MembershipType,
    bool IsActive = true
);

public record UpdateMemberRequest(
    [Required, MaxLength(150)] string Name,
    [Required, EmailAddress, MaxLength(254)] string Email,
    MembershipType MembershipType,
    bool IsActive
);

public record BorrowRequest(
    [Range(1, int.MaxValue)] int MemberId,
    [Range(1, int.MaxValue)] int BookId
);

public record ReturnRequest(
    DateTime? ReturnDate
);

public record ReservationRequest(
    [Range(1, int.MaxValue)] int MemberId,
    [Range(1, int.MaxValue)] int BookId
);

public record PayFeeRequest(
    bool Paid = true
);