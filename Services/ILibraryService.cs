
using LibraryManagementSystem.Api.DTOs;
using LibraryManagementSystem.Api.Entities;

namespace LibraryManagementSystem.Api.Services;

public interface ILibraryService
{
    Task<PagedResponse<BookListItem>> GetBooks(
        string? search, int page, int pageSize);

    Task<BookDetailsResponse?> GetBook(int id);
    Task<Book> CreateBook(CreateBookRequest request);

    Task<List<Member>> GetMembers();

    Task<MemberDetailsResponse?> GetMember(int id);
    Task<Member> CreateMember(CreateMemberRequest request);
    Task<Member?> UpdateMember(int id, UpdateMemberRequest request);

    Task<LoanResponse> Borrow(BorrowRequest request);
    Task<LoanResponse?> Return(int id, DateTime? returnDate);
    Task<LoanResponse?> Renew(int id);
    Task<LoanResponse?> PayFee(int id);
    Task<List<LoanResponse>> GetLoans();
    Task<ReservationResponse> Reserve(ReservationRequest request);
    Task<bool> CancelReservation(int id);
    Task<List<ReservationResponse>> GetReservations();

    Task<object> GetDashboard();
    Task<List<object>> GetMemberActivity(int year, int month);
}
