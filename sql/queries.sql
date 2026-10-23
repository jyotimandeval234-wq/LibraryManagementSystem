
-- queries.sql

-- 1. Search books by title, author, ISBN, or genre
DECLARE @Search NVARCHAR(200) = N'Clean';

SELECT
    Id,
    Title,
    Author,
    ISBN,
    Genre,
    PublishedYear
FROM dbo.Books
WHERE Title LIKE N'%' + @Search + N'%'
   OR Author LIKE N'%' + @Search + N'%'
   OR ISBN LIKE N'%' + @Search + N'%'
   OR Genre LIKE N'%' + @Search + N'%'
ORDER BY Title;


-- 2. Find available book copies
-- Assumes CopyStatus value 0 means Available.
SELECT
    b.Id AS BookId,
    b.Title,
    b.Author,
    bc.Id AS BookCopyId,
    bc.CopyCode,
    bc.Status
FROM dbo.Books AS b
INNER JOIN dbo.BookCopies AS bc
    ON bc.BookId = b.Id
WHERE bc.Status = 0
ORDER BY b.Title, bc.CopyCode;


-- 3. List active loans with member and book details
SELECT
    l.Id AS LoanId,
    m.Id AS MemberId,
    m.Name AS MemberName,
    m.Email,
    b.Title AS BookTitle,
    bc.CopyCode,
    l.LoanDate,
    l.DueDate,
    l.ReturnDate,
    l.LateFee,
    l.FeePaid
FROM dbo.Loans AS l
INNER JOIN dbo.Members AS m
    ON m.Id = l.MemberId
INNER JOIN dbo.BookCopies AS bc
    ON bc.Id = l.BookCopyId
INNER JOIN dbo.Books AS b
    ON b.Id = bc.BookId
WHERE l.ReturnDate IS NULL
ORDER BY l.DueDate;


-- 4. Find overdue loans
SELECT
    l.Id AS LoanId,
    m.Name AS MemberName,
    m.Email,
    b.Title AS BookTitle,
    bc.CopyCode,
    l.LoanDate,
    l.DueDate,
    DATEDIFF(DAY, l.DueDate, GETDATE()) AS DaysOverdue,
    l.LateFee,
    l.FeePaid
FROM dbo.Loans AS l
INNER JOIN dbo.Members AS m
    ON m.Id = l.MemberId
INNER JOIN dbo.BookCopies AS bc
    ON bc.Id = l.BookCopyId
INNER JOIN dbo.Books AS b
    ON b.Id = bc.BookId
WHERE l.ReturnDate IS NULL
  AND l.DueDate < GETDATE()
ORDER BY l.DueDate;


-- 5. List reservations with member and book details
SELECT
    r.Id AS ReservationId,
    m.Name AS MemberName,
    m.Email,
    b.Title AS BookTitle,
    r.CreatedAt,
    r.Status
FROM dbo.Reservations AS r
INNER JOIN dbo.Members AS m
    ON m.Id = r.MemberId
INNER JOIN dbo.Books AS b
    ON b.Id = r.BookId
ORDER BY r.CreatedAt DESC;
