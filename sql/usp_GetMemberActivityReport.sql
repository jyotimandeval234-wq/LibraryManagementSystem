
-- usp_GetMemberActivityReport.sql

CREATE OR ALTER PROCEDURE dbo.usp_GetMemberActivityReport
    @Year INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        m.Id AS MemberId,
        m.Name AS MemberName,
        m.Email,
        l.Id AS LoanId,
        b.Id AS BookId,
        b.Title AS BookTitle,
        bc.CopyCode,
        l.LoanDate,
        l.DueDate,
        l.ReturnDate,
        l.Renewed,
        l.LateFee,
        l.FeePaid,
        CASE
            WHEN l.ReturnDate IS NULL
                 AND l.DueDate < GETDATE()
                THEN 'Overdue'
            WHEN l.ReturnDate IS NULL
                THEN 'Active'
            ELSE 'Returned'
        END AS LoanStatus
    FROM dbo.Loans AS l
    INNER JOIN dbo.Members AS m
        ON m.Id = l.MemberId
    INNER JOIN dbo.BookCopies AS bc
        ON bc.Id = l.BookCopyId
    INNER JOIN dbo.Books AS b
        ON b.Id = bc.BookId
    WHERE @Year IS NULL
       OR (
            l.LoanDate >= DATEFROMPARTS(@Year, 1, 1)
            AND l.LoanDate < DATEFROMPARTS(@Year + 1, 1, 1)
       )
    ORDER BY l.LoanDate DESC, m.Name;
END;
GO

-- Example: report for 2026
EXEC dbo.usp_GetMemberActivityReport @Year = 2026;
GO

-- Example: report for all years
EXEC dbo.usp_GetMemberActivityReport;
GO
