using Microsoft.EntityFrameworkCore.Migrations;

namespace EducationalFundingCo.Migrations
{
    public partial class ConfigValue_SchoolIdFk : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SchoolId",
                table: "ConfigValues",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ConfigValues_SchoolId",
                table: "ConfigValues",
                column: "SchoolId");

            migrationBuilder.AddForeignKey(
                name: "FK_ConfigValues_School_SchoolId",
                table: "ConfigValues",
                column: "SchoolId",
                principalTable: "School",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConfigValues_School_SchoolId",
                table: "ConfigValues");

            migrationBuilder.DropIndex(
                name: "IX_ConfigValues_SchoolId",
                table: "ConfigValues");

            migrationBuilder.DropColumn(
                name: "SchoolId",
                table: "ConfigValues");
        }
    }
}
