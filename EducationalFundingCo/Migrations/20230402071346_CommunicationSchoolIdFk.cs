using Microsoft.EntityFrameworkCore.Migrations;

namespace EducationalFundingCo.Migrations
{
    public partial class CommunicationSchoolIdFk : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SchoolId",
                table: "Communications",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Communications_SchoolId",
                table: "Communications",
                column: "SchoolId");

            migrationBuilder.AddForeignKey(
                name: "FK_Communications_School_SchoolId",
                table: "Communications",
                column: "SchoolId",
                principalTable: "School",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Communications_School_SchoolId",
                table: "Communications");

            migrationBuilder.DropIndex(
                name: "IX_Communications_SchoolId",
                table: "Communications");

            migrationBuilder.DropColumn(
                name: "SchoolId",
                table: "Communications");
        }
    }
}
