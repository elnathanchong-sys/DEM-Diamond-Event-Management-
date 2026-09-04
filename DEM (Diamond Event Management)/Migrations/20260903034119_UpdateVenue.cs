using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DEM__Diamond_Event_Management_.Migrations
{
    /// <inheritdoc />
    public partial class UpdateVenue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "VenueCapacity",
                table: "Venue",
                newName: "Capacity");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Venue",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Venue",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "VenueName",
                table: "Venue",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Venue");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "Venue");

            migrationBuilder.DropColumn(
                name: "VenueName",
                table: "Venue");

            migrationBuilder.RenameColumn(
                name: "Capacity",
                table: "Venue",
                newName: "VenueCapacity");
        }
    }
}
