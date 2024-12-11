using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DriveX_Backend.Migrations
{
    /// <inheritdoc />
    public partial class overdue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "OverDueAmount",
                table: "RentalRequests",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "OverDueDuration",
                table: "RentalRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OverDueAmount",
                table: "RentalRequests");

            migrationBuilder.DropColumn(
                name: "OverDueDuration",
                table: "RentalRequests");
        }
    }
}
