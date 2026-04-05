using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetMateAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddVets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AvailableDays",
                table: "Vets",
                newName: "Services");

            migrationBuilder.AddColumn<int>(
                name: "ExperienceYears",
                table: "Vets",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Vets",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirstNameLower",
                table: "Vets",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Vets",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastNameLower",
                table: "Vets",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Vets",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Vets",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameLower",
                table: "Vets",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Price",
                table: "Vets",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RewardPoints",
                table: "Vets",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WaitingTimeMinutes",
                table: "Vets",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WorkingDays",
                table: "Vets",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WorkingTime",
                table: "Vets",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExperienceYears",
                table: "Vets");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Vets");

            migrationBuilder.DropColumn(
                name: "FirstNameLower",
                table: "Vets");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Vets");

            migrationBuilder.DropColumn(
                name: "LastNameLower",
                table: "Vets");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Vets");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Vets");

            migrationBuilder.DropColumn(
                name: "NameLower",
                table: "Vets");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Vets");

            migrationBuilder.DropColumn(
                name: "RewardPoints",
                table: "Vets");

            migrationBuilder.DropColumn(
                name: "WaitingTimeMinutes",
                table: "Vets");

            migrationBuilder.DropColumn(
                name: "WorkingDays",
                table: "Vets");

            migrationBuilder.DropColumn(
                name: "WorkingTime",
                table: "Vets");

            migrationBuilder.RenameColumn(
                name: "Services",
                table: "Vets",
                newName: "AvailableDays");
        }
    }
}
