
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Api.Entities;

public enum MembershipType
{
    Standard = 0,
    Premium = 1
}

public class Member
{
    public int Id { get; set; }

    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(254)]
    public string Email { get; set; } = string.Empty;

    public MembershipType MembershipType { get; set; }
        = MembershipType.Standard;

    public bool IsActive { get; set; } = true;

    public ICollection<Loan> Loans { get; set; }
        = new List<Loan>();

    public ICollection<Reservation> Reservations { get; set; }
        = new List<Reservation>();
}