
using LibraryManagementSystem.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Data
{
    public class LibraryDbContext : DbContext
    {
        public LibraryDbContext(
            DbContextOptions<LibraryDbContext> options)
            : base(options)
        {
        }

        public DbSet<Book> Books => Set<Book>();
        public DbSet<BookCopy> BookCopies => Set<BookCopy>();
        public DbSet<Member> Members => Set<Member>();
        public DbSet<Loan> Loans => Set<Loan>();
        public DbSet<Reservation> Reservations => Set<Reservation>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Book: ISBN must be unique.
            modelBuilder.Entity<Book>()
                .HasIndex(b => b.ISBN)
                .IsUnique();

            // BookCopy: CopyCode must be unique.
            modelBuilder.Entity<BookCopy>()
                .HasIndex(c => c.CopyCode)
                .IsUnique();

            // Member: Email must be unique.
            modelBuilder.Entity<Member>()
                .HasIndex(m => m.Email)
                .IsUnique();

            // One Book has many BookCopies.
            modelBuilder.Entity<BookCopy>()
                .HasOne(c => c.Book)
                .WithMany(b => b.Copies)
                .HasForeignKey(c => c.BookId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            // One BookCopy has many historical Loans.
            modelBuilder.Entity<Loan>()
                .HasOne(l => l.BookCopy)
                .WithMany()
                .HasForeignKey(l => l.BookCopyId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            // One Member has many Loans.
            modelBuilder.Entity<Loan>()
                .HasOne(l => l.Member)
                .WithMany()
                .HasForeignKey(l => l.MemberId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            // Only one active loan per BookCopy.
            modelBuilder.Entity<Loan>()
                .HasIndex(l => l.BookCopyId)
                .HasFilter("[ReturnDate] IS NULL")
                .IsUnique()
                .HasDatabaseName("UX_Loans_OneActiveLoanPerCopy");

            // Reservations reference a Book.
            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Book)
                .WithMany()
                .HasForeignKey(r => r.BookId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            // Reservations reference a Member.
            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Member)
                .WithMany()
                .HasForeignKey(r => r.MemberId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            // LateFee supports two decimal places.
            modelBuilder.Entity<Loan>()
                .Property(l => l.LateFee)
                .HasPrecision(10, 2);
        }
    }
}
