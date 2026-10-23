
using LibraryManagementSystem.Api.Entities;

namespace LibraryManagementSystem.Api.Services;

public static class LoanRules
{
    public static int GetMaximumActiveLoans(MembershipType type)
    {
        return type switch
        {
            MembershipType.Standard => 3,
            MembershipType.Premium => 5,
            _ => throw new ArgumentException(
                "Invalid membership type.")
        };
    }

    public static int GetLoanPeriodDays(MembershipType type)
    {
        return type switch
        {
            MembershipType.Standard => 14,
            MembershipType.Premium => 21,
            _ => throw new ArgumentException(
                "Invalid membership type.")
        };
    }

    public static decimal CalculateLateFee(
        DateTime dueDate,
        DateTime returnDate)
    {
        int lateDays = Math.Max(
            0,
            (returnDate.Date - dueDate.Date).Days);

        return Math.Min(lateDays * 0.50m, 20.00m);
    }

    public static void ValidateBorrowing(
        Member member,
        int activeLoanCount,
        bool hasOverdueLoan)
    {
        if (!member.IsActive)
        {
            throw new InvalidOperationException(
                "Inactive members cannot borrow books.");
        }

        if (hasOverdueLoan)
        {
            throw new InvalidOperationException(
                "Return overdue books before borrowing another book.");
        }

        int limit = GetMaximumActiveLoans(member.MembershipType);

        if (activeLoanCount >= limit)
        {
            throw new InvalidOperationException(
                $"This member has reached the active loan limit of {limit}.");
        }
    }

    public static void ValidateRenewal(
        bool renewed,
        DateTime dueDate,
        DateTime today)
    {
        if (renewed)
        {
            throw new InvalidOperationException(
                "This loan has already been renewed.");
        }

        if (today.Date > dueDate.Date)
        {
            throw new InvalidOperationException(
                "An overdue loan cannot be renewed.");
        }
    }
}
