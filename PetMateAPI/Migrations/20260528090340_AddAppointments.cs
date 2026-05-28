using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetMateAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddAppointments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "EmergencyFee",
                table: "Appointments",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "TotalFee",
                table: "Appointments",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmergencyFee",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "TotalFee",
                table: "Appointments");
        }
    }
}
