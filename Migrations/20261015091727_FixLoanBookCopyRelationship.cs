using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class FixLoanBookCopyRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Loans_BookCopies_BookCopyId1",
                table: "Loans");

            migrationBuilder.DropForeignKey(
                name: "FK_Loans_Members_MemberId1",
                table: "Loans");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Books_BookId1",
                table: "Reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Members_MemberId1",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Loans_BookCopyId1",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "BookCopyId1",
                table: "Loans");

            migrationBuilder.AlterColumn<int>(
                name: "MemberId1",
                table: "Reservations",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "BookId1",
                table: "Reservations",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "MemberId1",
                table: "Loans",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Loans_Members_MemberId1",
                table: "Loans",
                column: "MemberId1",
                principalTable: "Members",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Books_BookId1",
                table: "Reservations",
                column: "BookId1",
                principalTable: "Books",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Members_MemberId1",
                table: "Reservations",
                column: "MemberId1",
                principalTable: "Members",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Loans_Members_MemberId1",
                table: "Loans");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Books_BookId1",
                table: "Reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Members_MemberId1",
                table: "Reservations");

            migrationBuilder.AlterColumn<int>(
                name: "MemberId1",
                table: "Reservations",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "BookId1",
                table: "Reservations",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "MemberId1",
                table: "Loans",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BookCopyId1",
                table: "Loans",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Loans_BookCopyId1",
                table: "Loans",
                column: "BookCopyId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Loans_BookCopies_BookCopyId1",
                table: "Loans",
                column: "BookCopyId1",
                principalTable: "BookCopies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Loans_Members_MemberId1",
                table: "Loans",
                column: "MemberId1",
                principalTable: "Members",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Books_BookId1",
                table: "Reservations",
                column: "BookId1",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Members_MemberId1",
                table: "Reservations",
                column: "MemberId1",
                principalTable: "Members",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
