using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BloodConnect.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDonorOptionalFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BloodGroup",
                table: "Donors",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "District",
                table: "Donors",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FatherHusbandName",
                table: "Donors",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SourceOfInfo",
                table: "Donors",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TimesDonatedBefore",
                table: "Donors",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BloodGroup",
                table: "Donors");

            migrationBuilder.DropColumn(
                name: "District",
                table: "Donors");

            migrationBuilder.DropColumn(
                name: "FatherHusbandName",
                table: "Donors");

            migrationBuilder.DropColumn(
                name: "SourceOfInfo",
                table: "Donors");

            migrationBuilder.DropColumn(
                name: "TimesDonatedBefore",
                table: "Donors");
        }
    }
}
