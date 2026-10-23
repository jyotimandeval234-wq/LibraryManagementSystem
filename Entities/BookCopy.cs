
namespace LibraryManagementSystem.Api.Entities;

public enum CopyStatus
{
    Available,
    OnLoan
}

public class BookCopy
{
    public int Id { get; set; }

    public int BookId { get; set; }

    public string CopyCode { get; set; } = "";

    public CopyStatus Status { get; set; }

    public Book Book { get; set; } = null!;
}
