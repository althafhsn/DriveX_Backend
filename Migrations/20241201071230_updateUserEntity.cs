using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DriveX_Backend.Migrations
{
    /// <inheritdoc />
    public partial class updateUserEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "OngoingRevenue",
                table: "Users",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalRevenue",
                table: "Users",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OngoingRevenue",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TotalRevenue",
                table: "Users");
        }
    }
}
