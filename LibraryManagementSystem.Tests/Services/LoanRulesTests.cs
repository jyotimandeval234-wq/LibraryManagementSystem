using System;
using LibraryManagementSystem.Api.Entities;
using LibraryManagementSystem.Api.Services;
using Xunit;

namespace LibraryManagementSystem.Tests.Services;

public class LoanRulesTests
{
    // 1. LOAN LIMIT TESTS

    [Theory]
    [InlineData(MembershipType.Standard, 3)]
    [InlineData(MembershipType.Premium, 5)]
    public void GetMaximumActiveLoans_ReturnsCorrectLimit(
        MembershipType membershipType,
        int expectedLimit)
    {
        var limit =
            LoanRules.GetMaximumActiveLoans(membershipType);

        Assert.Equal(expectedLimit, limit);
    }

    [Fact]
    public void StandardMember_CannotBorrowWhenLimitReached()
    {
        var member = new Member
        {
            Name = "Standard Member",
            Email = "standard@test.com",
            MembershipType = MembershipType.Standard,
            IsActive = true
        };

        var exception = Assert.Throws<InvalidOperationException>(
            () => LoanRules.ValidateBorrowing(
                member,
                activeLoanCount: 3,
                hasOverdueLoan: false));

        Assert.Contains("loan limit", exception.Message);
    }

    [Fact]
    public void PremiumMember_CannotBorrowWhenLimitReached()
    {
        var member = new Member
        {
            Name = "Premium Member",
            Email = "premium@test.com",
            MembershipType = MembershipType.Premium,
            IsActive = true
        };

        var exception = Assert.Throws<InvalidOperationException>(
            () => LoanRules.ValidateBorrowing(
                member,
                activeLoanCount: 5,
                hasOverdueLoan: false));

        Assert.Contains("loan limit", exception.Message);
    }

    [Fact]
    public void MemberBelowLoanLimit_CanBorrow()
    {
        var member = new Member
        {
            Name = "Standard Member",
            Email = "standard2@test.com",
            MembershipType = MembershipType.Standard,
            IsActive = true
        };

        LoanRules.ValidateBorrowing(
            member,
            activeLoanCount: 2,
            hasOverdueLoan: false);
    }

    // 2. LATE FEE TESTS

    [Fact]
    public void CalculateLateFee_WhenReturnedOnTime_ReturnsZero()
    {
        var dueDate = new DateTime(2026, 10, 1);
        var returnDate = new DateTime(2026, 10, 1);

        var fee = LoanRules.CalculateLateFee(
            dueDate, returnDate);

        Assert.Equal(0.00m, fee);
    }

    [Fact]
    public void CalculateLateFee_WhenReturnedTwoDaysLate_ReturnsOne()
    {
        var dueDate = new DateTime(2026, 10, 1);
        var returnDate = new DateTime(2026, 10, 3);

        var fee = LoanRules.CalculateLateFee(
            dueDate, returnDate);

        Assert.Equal(1.00m, fee);
    }

    [Fact]
    public void CalculateLateFee_WhenNotLate_ReturnsZero()
    {
        var dueDate = new DateTime(2026, 10, 10);
        var returnDate = new DateTime(2026, 10, 5);

        var fee = LoanRules.CalculateLateFee(
            dueDate, returnDate);

        Assert.Equal(0.00m, fee);
    }

    [Fact]
    public void CalculateLateFee_WhenMaximumExceeded_ReturnsTwenty()
    {
        var dueDate = new DateTime(2026, 10, 1);
        var returnDate = new DateTime(2026, 12, 1);

        var fee = LoanRules.CalculateLateFee(
            dueDate, returnDate);

        Assert.Equal(20.00m, fee);
    }

    // 3. RENEWAL TESTS

    [Fact]
    public void ValidateRenewal_WhenNotRenewedAndNotOverdue_DoesNotThrow()
    {
        var dueDate = new DateTime(2026, 10, 20);
        var today = new DateTime(2026, 10, 15);

        var exception = Record.Exception(() =>
            LoanRules.ValidateRenewal(
                renewed: false,
                dueDate: dueDate,
                today: today));

        Assert.Null(exception);
    }

    [Fact]
    public void ValidateRenewal_WhenAlreadyRenewed_ThrowsException()
    {
        var dueDate = new DateTime(2026, 10, 20);
        var today = new DateTime(2026, 10, 15);

        var exception = Assert.Throws<InvalidOperationException>(
            () => LoanRules.ValidateRenewal(
                renewed: true,
                dueDate: dueDate,
                today: today));

        Assert.Equal(
            "This loan has already been renewed.",
            exception.Message);
    }

    [Fact]
    public void ValidateRenewal_WhenOverdue_ThrowsException()
    {
        var dueDate = new DateTime(2026, 10, 10);
        var today = new DateTime(2026, 10, 15);

        var exception = Assert.Throws<InvalidOperationException>(
            () => LoanRules.ValidateRenewal(
                renewed: false,
                dueDate: dueDate,
                today: today));

        Assert.Equal(
            "An overdue loan cannot be renewed.",
            exception.Message);
    }

    // 4. ADDITIONAL BORROWING RULE TESTS

    [Fact]
    public void InactiveMember_CannotBorrow()
    {
        var member = new Member
        {
            Name = "Inactive Member",
            Email = "inactive@test.com",
            MembershipType = MembershipType.Standard,
            IsActive = false
        };

        Assert.Throws<InvalidOperationException>(
            () => LoanRules.ValidateBorrowing(
                member, 0, false));
    }

    [Fact]
    public void MemberWithOverdueLoan_CannotBorrow()
    {
        var member = new Member
        {
            Name = "Overdue Member",
            Email = "overdue@test.com",
            MembershipType = MembershipType.Premium,
            IsActive = true
        };

        Assert.Throws<InvalidOperationException>(
            () => LoanRules.ValidateBorrowing(
                member, 0, true));
    }
}
