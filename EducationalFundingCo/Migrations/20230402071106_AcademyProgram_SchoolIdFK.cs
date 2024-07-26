using Microsoft.EntityFrameworkCore.Migrations;

namespace EducationalFundingCo.Migrations
{
    public partial class AcademyProgram_SchoolIdFK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SchoolId",
                table: "Contracts",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SchoolId",
                table: "AcademyPrograms",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_SchoolId",
                table: "Contracts",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_AcademyPrograms_SchoolId",
                table: "AcademyPrograms",
                column: "SchoolId");

            migrationBuilder.AddForeignKey(
                name: "FK_AcademyPrograms_School_SchoolId",
                table: "AcademyPrograms",
                column: "SchoolId",
                principalTable: "School",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Contracts_School_SchoolId",
                table: "Contracts",
                column: "SchoolId",
                principalTable: "School",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AcademyPrograms_School_SchoolId",
                table: "AcademyPrograms");

            migrationBuilder.DropForeignKey(
                name: "FK_Contracts_School_SchoolId",
                table: "Contracts");

            migrationBuilder.DropIndex(
                name: "IX_Contracts_SchoolId",
                table: "Contracts");

            migrationBuilder.DropIndex(
                name: "IX_AcademyPrograms_SchoolId",
                table: "AcademyPrograms");

            migrationBuilder.DropColumn(
                name: "SchoolId",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "SchoolId",
                table: "AcademyPrograms");
        }
    }
}
