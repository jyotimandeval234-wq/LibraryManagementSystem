
namespace LibraryManagementSystem.Api.Entities;

public class Loan
{
    public int Id { get; set; }

    public int BookCopyId { get; set; }

    public int MemberId { get; set; }

    public DateTime LoanDate { get; set; }

    public DateTime DueDate { get; set; }

    public DateTime? ReturnDate { get; set; }

    public bool Renewed { get; set; } = false;

    public decimal LateFee { get; set; } = 0.00m;

    public bool FeePaid { get; set; } = false;

    // Navigation properties
    public BookCopy BookCopy { get; set; } = null!;

    public Member Member { get; set; } = null!;
}
