using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class FixBookCopyRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookCopies_Books_BookId1",
                table: "BookCopies");

            migrationBuilder.DropIndex(
                name: "IX_BookCopies_BookId1",
                table: "BookCopies");

            migrationBuilder.DropColumn(
                name: "BookId1",
                table: "BookCopies");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BookId1",
                table: "BookCopies",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_BookCopies_BookId1",
                table: "BookCopies",
                column: "BookId1");

            migrationBuilder.AddForeignKey(
                name: "FK_BookCopies_Books_BookId1",
                table: "BookCopies",
                column: "BookId1",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
