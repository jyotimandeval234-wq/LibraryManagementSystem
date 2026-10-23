
using LibraryManagementSystem.Api.DTOs;
using LibraryManagementSystem.Api.Entities;
using LibraryManagementSystem.Data;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace LibraryManagementSystem.Api.Services;

public class LibraryService : ILibraryService
{
    private readonly LibraryDbContext _db;

    public LibraryService(LibraryDbContext db)
    {
        _db = db;
    }

    // -------------------- BOOKS --------------------

    public async Task<PagedResponse<BookListItem>> GetBooks(
        string? search, int page, int pageSize)
    {
        if (page < 1)
            throw new ArgumentException("Page must be at least 1.");

        if (pageSize < 1 || pageSize > 100)
            throw new ArgumentException(
                "Page size must be between 1 and 100.");

        var query = _db.Books.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            string term = search.Trim();

            query = query.Where(b =>
                b.Title.Contains(term) ||
                b.Author.Contains(term) ||
                b.ISBN.Contains(term) ||
                 b.Description.Contains(term) ||
                b.Genre.Contains(term));
        }

        int totalCount = await query.CountAsync();

        var books = await query
            .OrderBy(b => b.Title)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(b => new BookListItem(
                b.Id,
                b.Title,
                b.Author,
                b.ISBN,
                b.Genre,
                b.PublishedYear,
                b.Description,
                b.Copies.Count(c =>
                    c.Status == CopyStatus.Available),
                b.Copies.Count()))
            .ToListAsync();

        return new PagedResponse<BookListItem>(
            page, pageSize, totalCount, books);
    }

    public async Task<BookDetailsResponse?> GetBook(int id)
    {
        var book = await _db.Books
            .AsNoTracking()
            .Include(b => b.Copies)
            .Include(b => b.Reservations)
                .ThenInclude(r => r.Member)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (book == null)
            return null;

        return new BookDetailsResponse(
            book.Id,
            book.Title,
            book.Author,
            book.ISBN,
            book.Genre,
            book.PublishedYear,
            book.Copies
                .OrderBy(c => c.CopyCode)
                .Select(c => new CopyResponse(
                    c.Id, c.CopyCode, c.Status))
                .ToList(),
            book.Reservations
                .OrderBy(r => r.CreatedAt)
                .Select(r => new ReservationResponse(
                    r.Id,
                    r.MemberId,
                    r.Member.Name,
                    r.CreatedAt,
                    r.Status))
                .ToList());
    }

    public async Task<Book> CreateBook(CreateBookRequest request)
    {
        string isbn = request.ISBN.Trim();

        if (await _db.Books.AnyAsync(b => b.ISBN == isbn))
        {
            throw new InvalidOperationException(
                "A book with this ISBN already exists.");
        }

        if (request.NumberOfCopies < 1 ||
            request.NumberOfCopies > 100)
        {
            throw new ArgumentException(
                "Number of copies must be between 1 and 100.");
        }

        var book = new Book
        {
            Title = request.Title.Trim(),
            Author = request.Author.Trim(),
            ISBN = isbn,
            Genre = request.Genre.Trim(),
            PublishedYear = request.PublishedYear
        };

        await using var transaction =
            await _db.Database.BeginTransactionAsync();

        try
        {
            _db.Books.Add(book);
            await _db.SaveChangesAsync();

            var copies = Enumerable.Range(
                    1, request.NumberOfCopies)
                .Select(i => new BookCopy
                {
                    BookId = book.Id,
                    CopyCode = $"{book.ISBN}-{i:D3}",
                    Status = CopyStatus.Available
                })
                .ToList();

            _db.BookCopies.AddRange(copies);
            await _db.SaveChangesAsync();

            await transaction.CommitAsync();

            return book;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    // -------------------- MEMBERS --------------------

    public async Task<List<Member>> GetMembers()
    {
        return await _db.Members
            .AsNoTracking()
            .OrderBy(m => m.Name)
            .ToListAsync();
    }

    public async Task<MemberDetailsResponse?> GetMember(int id)
    {
        var member = await _db.Members
            .AsNoTracking()
            .Include(m => m.Loans)
                .ThenInclude(l => l.BookCopy)
                    .ThenInclude(c => c.Book)
            .Include(m => m.Loans)
                .ThenInclude(l => l.Member)
            .Include(m => m.Reservations)
                .ThenInclude(r => r.Book)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (member == null)
            return null;

        var activeLoans = member.Loans
            .Where(l => l.ReturnDate == null)
            .OrderBy(l => l.DueDate)
            .Select(ToLoanResponse)
            .ToList();

        var loanHistory = member.Loans
            .Where(l => l.ReturnDate != null)
            .OrderByDescending(l => l.LoanDate)
            .Select(ToLoanResponse)
            .ToList();

        var reservations = member.Reservations
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new ReservationResponse(
                r.Id,
                r.MemberId,
                member.Name,
                r.CreatedAt,
                r.Status))
            .ToList();

        return new MemberDetailsResponse(
            member.Id,
            member.Name,
            member.Email,
            member.MembershipType,
            member.IsActive,
            activeLoans,
            loanHistory,
            reservations);
    }

    public async Task<Member> CreateMember(CreateMemberRequest request)
    {
        string email = request.Email.Trim();

        if (await _db.Members.AnyAsync(m => m.Email == email))
            throw new InvalidOperationException(
                "A member with this email already exists.");

        var member = new Member
        {
            Name = request.Name.Trim(),
            Email = email,
            MembershipType = request.MembershipType,
            IsActive = request.IsActive
        };

        _db.Members.Add(member);
        await _db.SaveChangesAsync();

        return member;
    }

    public async Task<Member?> UpdateMember(
        int id, UpdateMemberRequest request)
    {
        var member = await _db.Members.FindAsync(id);

        if (member == null)
            return null;

        string email = request.Email.Trim();

        if (await _db.Members.AnyAsync(
            m => m.Id != id && m.Email == email))
        {
            throw new InvalidOperationException(
                "Another member already uses this email.");
        }

        member.Name = request.Name.Trim();
        member.Email = email;
        member.MembershipType = request.MembershipType;
        member.IsActive = request.IsActive;

        await _db.SaveChangesAsync();

        return member;
    }


    // -------------------- LOANS --------------------

    // GET: Retrieve all loan records
    public async Task<List<LoanResponse>> GetLoans()
    {
        var loans = await _db.Loans
            .AsNoTracking()
            .Include(l => l.Member)
            .Include(l => l.BookCopy)
                .ThenInclude(c => c.Book)
            .OrderByDescending(l => l.LoanDate)
            .ToListAsync();

        return loans
            .Select(ToLoanResponse)
            .ToList();
    }

    // POST: Borrow a book
    public async Task<LoanResponse> Borrow(BorrowRequest request)
    {
        var member = await _db.Members
            .FirstOrDefaultAsync(m => m.Id == request.MemberId);

        if (member == null)
            throw new KeyNotFoundException("Member not found.");

        if (!member.IsActive)
            throw new InvalidOperationException(
                "Inactive members cannot borrow books.");

        var book = await _db.Books
            .Include(b => b.Copies)
            .FirstOrDefaultAsync(b => b.Id == request.BookId);

        if (book == null)
            throw new KeyNotFoundException("Book not found.");

        int activeLoanCount = await _db.Loans.CountAsync(l =>
            l.MemberId == member.Id && l.ReturnDate == null);

        DateTime today = DateTime.UtcNow.Date;

        bool hasOverdueLoan = await _db.Loans.AnyAsync(l =>
            l.MemberId == member.Id &&
            l.ReturnDate == null &&
            l.DueDate.Date < today);

        LoanRules.ValidateBorrowing(
            member, activeLoanCount, hasOverdueLoan);

        var copy = book.Copies
            .Where(c => c.Status == CopyStatus.Available)
            .OrderBy(c => c.Id)
            .FirstOrDefault();

        if (copy == null)
            throw new InvalidOperationException(
                "No copies are currently available. Reserve this book instead.");

        DateTime loanDate = DateTime.UtcNow;

        var loan = new Loan
        {
            BookCopyId = copy.Id,
            MemberId = member.Id,
            LoanDate = loanDate,
            DueDate = loanDate.AddDays(
                LoanRules.GetLoanPeriodDays(member.MembershipType)),
            ReturnDate = null,
            Renewed = false,
            LateFee = 0m,
            FeePaid = true
        };

        copy.Status = CopyStatus.OnLoan;
        _db.Loans.Add(loan);

        // Fulfil this member's oldest waiting reservation for this book.
        var reservation = await _db.Reservations
            .Where(r =>
                r.BookId == book.Id &&
                r.MemberId == member.Id &&
                r.Status == ReservationStatus.Waiting)
            .OrderBy(r => r.CreatedAt)
            .FirstOrDefaultAsync();

        if (reservation != null)
            reservation.Status = ReservationStatus.Fulfilled;

        await _db.SaveChangesAsync();

        return await GetLoanResponse(loan.Id);
    }

    // POST: Return a borrowed book
    public async Task<LoanResponse?> Return(
        int id, DateTime? returnDate)
    {
        var loan = await _db.Loans
            .Include(l => l.Member)
            .Include(l => l.BookCopy)
                .ThenInclude(c => c.Book)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (loan == null)
            return null;

        if (loan.ReturnDate != null)
            throw new InvalidOperationException(
                "This loan has already been returned.");

        DateTime today = DateTime.UtcNow.Date;
        DateTime actualReturnDate =
            (returnDate ?? DateTime.UtcNow).Date;

        if (actualReturnDate < loan.LoanDate.Date)
            throw new ArgumentException(
                "Return date cannot be before the loan date.");

        if (actualReturnDate > today)
            throw new ArgumentException(
                "Return date cannot be in the future.");

        loan.ReturnDate = actualReturnDate;
        loan.LateFee = LoanRules.CalculateLateFee(
            loan.DueDate, actualReturnDate);

        loan.FeePaid = loan.LateFee == 0m;
        loan.BookCopy.Status = CopyStatus.Available;

        await _db.SaveChangesAsync();

        return ToLoanResponse(loan);
    }

    // POST: Renew an active loan
    public async Task<LoanResponse?> Renew(int id)
    {
        var loan = await _db.Loans
            .Include(l => l.Member)
            .Include(l => l.BookCopy)
                .ThenInclude(c => c.Book)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (loan == null)
            return null;

        if (loan.ReturnDate != null)
            throw new InvalidOperationException(
                "A returned loan cannot be renewed.");

        if (loan.Renewed)
            throw new InvalidOperationException(
                "This loan has already been renewed.");

        if (loan.DueDate.Date < DateTime.UtcNow.Date)
            throw new InvalidOperationException(
                "An overdue loan cannot be renewed.");

        int extensionDays =
            LoanRules.GetLoanPeriodDays(loan.Member.MembershipType);

        loan.DueDate = loan.DueDate.AddDays(extensionDays);
        loan.Renewed = true;

        await _db.SaveChangesAsync();

        return ToLoanResponse(loan);
    }

    // POST: Pay an outstanding late fee
    public async Task<LoanResponse?> PayFee(int id)
    {
        var loan = await _db.Loans
            .Include(l => l.Member)
            .Include(l => l.BookCopy)
                .ThenInclude(c => c.Book)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (loan == null)
            return null;

        if (loan.ReturnDate == null)
            throw new InvalidOperationException(
                "Return the book before paying its late fee.");

        if (loan.LateFee <= 0m)
            throw new InvalidOperationException(
                "There is no outstanding late fee for this loan.");

        if (loan.FeePaid)
            throw new InvalidOperationException(
                "The late fee has already been paid.");

        loan.FeePaid = true;

        await _db.SaveChangesAsync();

        return ToLoanResponse(loan);
    }


    // -------------------- RESERVATIONS --------------------

    // GET: Retrieve all reservation records
    public async Task<List<ReservationResponse>> GetReservations()
    {
        var reservations = await _db.Reservations
            .AsNoTracking()
            .Include(r => r.Member)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new ReservationResponse(
                r.Id,
                r.MemberId,
                r.Member.Name,
                r.CreatedAt,
                r.Status))
            .ToListAsync();

        return reservations;
    }

    // POST: Create a reservation
    public async Task<ReservationResponse> Reserve(
        ReservationRequest request)
    {
        var member = await _db.Members
            .FirstOrDefaultAsync(m => m.Id == request.MemberId);

        if (member == null)
            throw new KeyNotFoundException("Member not found.");

        if (!member.IsActive)
            throw new InvalidOperationException(
                "Inactive members cannot reserve books.");

        var book = await _db.Books
            .Include(b => b.Copies)
            .FirstOrDefaultAsync(b => b.Id == request.BookId);

        if (book == null)
            throw new KeyNotFoundException("Book not found.");

        bool available = book.Copies.Any(
            c => c.Status == CopyStatus.Available);

        if (available)
            throw new InvalidOperationException(
                "A copy is available. Borrow the book instead.");

        bool alreadyWaiting = await _db.Reservations.AnyAsync(r =>
            r.BookId == book.Id &&
            r.MemberId == member.Id &&
            r.Status == ReservationStatus.Waiting);

        if (alreadyWaiting)
            throw new InvalidOperationException(
                "You already have a waiting reservation for this book.");

        var reservation = new Reservation
        {
            BookId = book.Id,
            MemberId = member.Id,
            CreatedAt = DateTime.UtcNow,
            Status = ReservationStatus.Waiting
        };

        _db.Reservations.Add(reservation);
        await _db.SaveChangesAsync();

        return new ReservationResponse(
            reservation.Id,
            member.Id,
            member.Name,
            reservation.CreatedAt,
            reservation.Status);
    }

    // POST: Cancel a waiting reservation
    public async Task<bool> CancelReservation(int id)
    {
        var reservation = await _db.Reservations
            .FirstOrDefaultAsync(r => r.Id == id);

        if (reservation == null)
            return false;

        if (reservation.Status != ReservationStatus.Waiting)
            throw new InvalidOperationException(
                "Only waiting reservations can be cancelled.");

        reservation.Status = ReservationStatus.Cancelled;

        await _db.SaveChangesAsync();

        return true;
    }


    // -------------------- DASHBOARD --------------------

    public async Task<object> GetDashboard()
    {
        DateTime today = DateTime.UtcNow.Date;

        int totalBooks = await _db.Books.CountAsync();
        int totalCopies = await _db.BookCopies.CountAsync();

        int availableCopies = await _db.BookCopies.CountAsync(
            c => c.Status == CopyStatus.Available);

        int copiesOnLoan = await _db.BookCopies.CountAsync(
            c => c.Status == CopyStatus.OnLoan);
        int totalMembers = await _db.Members.CountAsync();

        int totalLoans = await _db.Loans.CountAsync();

        int activeLoans = await _db.Loans.CountAsync(
            l => l.ReturnDate == null);

        int activeMembers = await _db.Members.CountAsync(
            m => m.IsActive);

        var overdueLoans = await _db.Loans
            .AsNoTracking()
            .Where(l =>
                l.ReturnDate == null &&
                l.DueDate < today)
            .OrderBy(l => l.DueDate)
            .Select(l => new
            {
                l.Id,
                MemberName = l.Member.Name,
                BookTitle = l.BookCopy.Book.Title,
                l.DueDate
            })
            .ToListAsync();

        var overdueLoansWithDays = overdueLoans
            .Select(l => new
            {
                l.Id,
                l.MemberName,
                l.BookTitle,
                l.DueDate,
                DaysOverdue = (today - l.DueDate.Date).Days
            })
            .ToList();

        var topBorrowedBooks = await _db.Loans
            .AsNoTracking()
            .GroupBy(l => new
            {
                l.BookCopy.BookId,
                BookTitle = l.BookCopy.Book.Title
            })
            .Select(g => new
            {
                BookId = g.Key.BookId,
                BookTitle = g.Key.BookTitle,
                LoanCount = g.Count()
            })
            .OrderByDescending(b => b.LoanCount)
            .ThenBy(b => b.BookTitle)
            .Take(5)
            .ToListAsync();

        decimal unpaidFees = await _db.Loans
            .Where(l => !l.FeePaid && l.LateFee > 0m)
            .SumAsync(l => (decimal?)l.LateFee) ?? 0m;

        int waitingReservations = await _db.Reservations.CountAsync(
            r => r.Status == ReservationStatus.Waiting);

        return new
        {
            totalBooks,
            totalCopies,
            availableCopies,
            copiesOnLoan,
            totalMembers,
            activeMembers,
            totalLoans,
            activeLoans,
            overdueLoans = overdueLoansWithDays,
            topBorrowedBooks,
            waitingReservations,
            unpaidFees
        };
    }

    // -------------------- STORED PROCEDURE REPORT --------------------

    public async Task<List<object>> GetMemberActivity(
        int year, int month)
    {
        if (year < 1 || year > 9999)
            throw new ArgumentException("Year is invalid.");

        if (month < 1 || month > 12)
            throw new ArgumentException(
                "Month must be between 1 and 12.");

        var results = new List<object>();

        var connection = _db.Database.GetDbConnection();
        bool openedHere = connection.State != ConnectionState.Open;

        if (openedHere)
            await connection.OpenAsync();

        try
        {
            await using var command = connection.CreateCommand();

            command.CommandText =
                "EXEC dbo.usp_GetMemberActivityReport @Year, @Month";

            var yearParameter = command.CreateParameter();
            yearParameter.ParameterName = "@Year";
            yearParameter.Value = year;
            command.Parameters.Add(yearParameter);

            var monthParameter = command.CreateParameter();
            monthParameter.ParameterName = "@Month";
            monthParameter.Value = month;
            command.Parameters.Add(monthParameter);

            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                results.Add(new
                {
                    MemberId = reader["MemberId"],
                    MemberName = reader["MemberName"],
                    MembershipType = reader["MembershipType"],
                    LoansInMonth = reader["LoansInMonth"],
                    LateFeesInMonth = reader["LateFeesInMonth"]
                });
            }
        }
        finally
        {
            if (openedHere)
                await connection.CloseAsync();
        }

        return results;
    }

    // -------------------- MAPPING HELPERS --------------------

    private async Task<LoanResponse> GetLoanResponse(int id)
    {
        var loan = await _db.Loans
            .AsNoTracking()
            .Include(l => l.Member)
            .Include(l => l.BookCopy)
                .ThenInclude(c => c.Book)
            .FirstAsync(l => l.Id == id);

        return ToLoanResponse(loan);
    }

    private static LoanResponse ToLoanResponse(Loan loan)
    {
        return new LoanResponse(
            loan.Id,
            loan.BookCopyId,
            loan.BookCopy.CopyCode,
            loan.BookCopy.BookId,
            loan.BookCopy.Book.Title,
            loan.MemberId,
            loan.Member.Name,
            loan.LoanDate,
            loan.DueDate,
            loan.ReturnDate,
            loan.Renewed,
            loan.LateFee,
            loan.FeePaid);
    }
}
