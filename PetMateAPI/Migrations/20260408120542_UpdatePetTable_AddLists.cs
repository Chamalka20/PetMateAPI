using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetMateAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePetTable_AddLists : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Convert existing MedicalConditions text -> text[]
            migrationBuilder.Sql(
                @"ALTER TABLE ""Pets"" 
                  ALTER COLUMN ""MedicalConditions"" 
                  TYPE text[] USING string_to_array(""MedicalConditions"", ',');");

            // Convert existing Allergies text -> text[]
            migrationBuilder.Sql(
                @"ALTER TABLE ""Pets"" 
                  ALTER COLUMN ""Allergies"" 
                  TYPE text[] USING string_to_array(""Allergies"", ',');");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Convert back text[] -> text (comma-separated)
            migrationBuilder.Sql(
                @"ALTER TABLE ""Pets"" 
                  ALTER COLUMN ""MedicalConditions"" 
                  TYPE text USING array_to_string(""MedicalConditions"", ',');");

            migrationBuilder.Sql(
                @"ALTER TABLE ""Pets"" 
                  ALTER COLUMN ""Allergies"" 
                  TYPE text USING array_to_string(""Allergies"", ',');");
        }
    }
}

