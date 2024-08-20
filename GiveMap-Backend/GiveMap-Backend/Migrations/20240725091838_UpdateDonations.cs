using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GiveMap_Backend.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDonations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Condition",
                table: "Donations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ContactInfo",
                table: "Donations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Condition",
                table: "Donations");

            migrationBuilder.DropColumn(
                name: "ContactInfo",
                table: "Donations");
        }
    }
}
