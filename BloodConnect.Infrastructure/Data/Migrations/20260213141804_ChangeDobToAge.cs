using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BloodConnect.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeDobToAge : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                table: "Donors");

            migrationBuilder.AddColumn<int>(
                name: "Age",
                table: "Donors",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Age",
                table: "Donors");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfBirth",
                table: "Donors",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
