
-- seed.sql
-- Run after applying EF Core migrations.
-- Assumes EF Core uses the default integer mapping for enums.

SET NOCOUNT ON;

-- Insert sample books
IF NOT EXISTS (SELECT 1 FROM dbo.Books WHERE ISBN = '9780132350884')
BEGIN
    INSERT INTO dbo.Books
        (Title, Author, ISBN, Genre, Description, PublishedYear)
    VALUES
        ('Clean Code', 'Robert C. Martin', '9780132350884',
         'Programming', 2008);
END;

IF NOT EXISTS (SELECT 1 FROM dbo.Books WHERE ISBN = '9780134685991')
BEGIN
    INSERT INTO dbo.Books
        (Title, Author, ISBN, Genre, Description, PublishedYear)
    VALUES
        ('Effective Java', 'Joshua Bloch', '9780134685991',
         'Programming', 2018);
END;

IF NOT EXISTS (SELECT 1 FROM dbo.Books WHERE ISBN = '9780451524935')
BEGIN
    INSERT INTO dbo.Books
        (Title, Author, ISBN, Genre, Description, PublishedYear)
    VALUES
        ('1984', 'George Orwell', '9780451524935',
         'Fiction', 1949);
END;

-- Insert sample members
IF NOT EXISTS (
    SELECT 1 FROM dbo.Members WHERE Email = 'alice@example.com'
)
BEGIN
    INSERT INTO dbo.Members
        (Name, Email, MembershipType, IsActive)
    VALUES
        ('Alice Johnson', 'alice@example.com', 0, 1);
END;

IF NOT EXISTS (
    SELECT 1 FROM dbo.Members WHERE Email = 'bob@example.com'
)
BEGIN
    INSERT INTO dbo.Members
        (Name, Email, MembershipType, IsActive)
    VALUES
        ('Bob Smith', 'bob@example.com', 0, 1);
END;

-- Insert sample book copies
IF NOT EXISTS (
    SELECT 1 FROM dbo.BookCopies WHERE CopyCode = 'CC-001'
)
BEGIN
    INSERT INTO dbo.BookCopies (BookId, CopyCode, Status)
    SELECT Id, 'CC-001', 0
    FROM dbo.Books
    WHERE ISBN = '9780132350884';
END;

IF NOT EXISTS (
    SELECT 1 FROM dbo.BookCopies WHERE CopyCode = 'CC-002'
)
BEGIN
    INSERT INTO dbo.BookCopies (BookId, CopyCode, Status)
    SELECT Id, 'CC-002', 0
    FROM dbo.Books
    WHERE ISBN = '9780132350884';
END;

IF NOT EXISTS (
    SELECT 1 FROM dbo.BookCopies WHERE CopyCode = 'CC-003'
)
BEGIN
    INSERT INTO dbo.BookCopies (BookId, CopyCode, Status)
    SELECT Id, 'CC-003', 0
    FROM dbo.Books
    WHERE ISBN = '9780134685991';
END;

IF NOT EXISTS (
    SELECT 1 FROM dbo.BookCopies WHERE CopyCode = 'CC-004'
)
BEGIN
    INSERT INTO dbo.BookCopies (BookId, CopyCode, Status)
    SELECT Id, 'CC-004', 0
    FROM dbo.Books
    WHERE ISBN = '9780451524935';
END;

-- Verify inserted records
SELECT * FROM dbo.Books;
SELECT * FROM dbo.BookCopies;
SELECT * FROM dbo.Members;
