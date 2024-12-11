﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DriveX_Backend.Migrations
{
    /// <inheritdoc />
    public partial class returneddate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ReturnedDate",
                table: "RentalRequests",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReturnedDate",
                table: "RentalRequests");
        }
    }
}
