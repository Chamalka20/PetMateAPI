using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PetMateAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddAppointments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Appointments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VetId = table.Column<int>(type: "integer", nullable: false),
                    VetName = table.Column<string>(type: "text", nullable: false),
                    VetImageUrl = table.Column<string>(type: "text", nullable: true),
                    ClinicName = table.Column<string>(type: "text", nullable: true),
                    ClinicAddress = table.Column<string>(type: "text", nullable: true),
                    ClinicLatitude = table.Column<double>(type: "double precision", nullable: true),
                    ClinicLongitude = table.Column<double>(type: "double precision", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    UserName = table.Column<string>(type: "text", nullable: false),
                    UserPhone = table.Column<string>(type: "text", nullable: true),
                    HomeAddress = table.Column<string>(type: "text", nullable: true),
                    HomeLatitude = table.Column<double>(type: "double precision", nullable: true),
                    HomeLongitude = table.Column<double>(type: "double precision", nullable: true),
                    HomeAddressNotes = table.Column<string>(type: "text", nullable: true),
                    PetId = table.Column<string>(type: "text", nullable: false),
                    PetName = table.Column<string>(type: "text", nullable: false),
                    PetType = table.Column<string>(type: "text", nullable: true),
                    PetBreed = table.Column<string>(type: "text", nullable: true),
                    AppointmentDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TimeSlot = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ServiceType = table.Column<string>(type: "text", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    ConsultationFee = table.Column<double>(type: "double precision", nullable: false),
                    HomeVisitFee = table.Column<double>(type: "double precision", nullable: true),
                    PaymentStatus = table.Column<int>(type: "integer", nullable: false),
                    PaymentMethod = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CancelledAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CancellationReason = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appointments", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Appointments");
        }
    }
}
