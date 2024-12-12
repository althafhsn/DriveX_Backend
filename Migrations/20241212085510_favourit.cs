using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DriveX_Backend.Migrations
{
    /// <inheritdoc />
    public partial class favourit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Favourites_Cars_CarId1",
                table: "Favourites");

            migrationBuilder.DropForeignKey(
                name: "FK_Favourites_Users_UserId1",
                table: "Favourites");

            migrationBuilder.DropIndex(
                name: "IX_Favourites_CarId1",
                table: "Favourites");

            migrationBuilder.DropIndex(
                name: "IX_Favourites_UserId1",
                table: "Favourites");

            migrationBuilder.DropColumn(
                name: "CarId1",
                table: "Favourites");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Favourites");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CarId1",
                table: "Favourites",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UserId1",
                table: "Favourites",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Favourites_CarId1",
                table: "Favourites",
                column: "CarId1");

            migrationBuilder.CreateIndex(
                name: "IX_Favourites_UserId1",
                table: "Favourites",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Favourites_Cars_CarId1",
                table: "Favourites",
                column: "CarId1",
                principalTable: "Cars",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Favourites_Users_UserId1",
                table: "Favourites",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
