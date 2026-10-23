
using LibraryManagementSystem.Api.Entities;

namespace LibraryManagementSystem.Api.DTOs;

public record BookListItem(
    int Id,
    string Title,
    string Author,
    string ISBN,
    string Genre,
    int PublishedYear,
    string Description,
    int AvailableCopies,
    int TotalCopies
);

public record CopyResponse(
    int Id,
    string CopyCode,
    CopyStatus Status
);

public record ReservationResponse(
    int Id,
    int MemberId,
    string MemberName,
    DateTime CreatedAt,
    ReservationStatus Status
);

public record BookDetailsResponse(
    int Id,
    string Title,
    string Author,
    string ISBN,
    string Genre,
    int PublishedYear,
    List<CopyResponse> Copies,
    List<ReservationResponse> Reservations
);

public record LoanResponse(
    int Id,
    int BookCopyId,
    string CopyCode,
    int BookId,
    string BookTitle,
    int MemberId,
    string MemberName,
    DateTime LoanDate,
    DateTime DueDate,
    DateTime? ReturnDate,
    bool Renewed,
    decimal LateFee,
    bool FeePaid
);

public record MemberDetailsResponse(
    int Id,
    string Name,
    string Email,
    MembershipType MembershipType,
    bool IsActive,
    List<LoanResponse> ActiveLoans,
    List<LoanResponse> LoanHistory,
    List<ReservationResponse> Reservations
);

public record PagedResponse<T>(
    int Page,
    int PageSize,
    int TotalCount,
    List<T> Items
);