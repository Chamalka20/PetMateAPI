using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetMateAPI.Migrations
{
    /// <inheritdoc />
    public partial class ChangePetIdToIn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ← Replace AlterColumn with raw SQL using USING cast
            migrationBuilder.Sql(
                @"ALTER TABLE ""Appointments"" 
                  ALTER COLUMN ""PetId"" TYPE integer 
                  USING ""PetId""::integer"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"ALTER TABLE ""Appointments"" 
                  ALTER COLUMN ""PetId"" TYPE text 
                  USING ""PetId""::text"
            );
        }
    }
}