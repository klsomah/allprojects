using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EducationalFundingCo.Migrations
{
    public partial class Onboarding : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AlterColumn<decimal>(
            //    name: "Income",
            //    table: "Payments",
            //    nullable: true,
            //    oldClrType: typeof(decimal),
            //    oldType: "decimal(18,2)");

            //migrationBuilder.AlterColumn<decimal>(
            //    name: "Amount",
            //    table: "Payments",
            //    nullable: true,
            //    oldClrType: typeof(decimal),
            //    oldType: "decimal(18,2)");

            //migrationBuilder.AddColumn<decimal>(
            //    name: "LateFees",
            //    table: "Payments",
            //    nullable: true);

            //migrationBuilder.AddColumn<decimal>(
            //    name: "ProcessingFee",
            //    table: "Payments",
            //    nullable: true);

            //migrationBuilder.AddColumn<string>(
            //    name: "Address",
            //    table: "Contracts",
            //    nullable: true);

            //migrationBuilder.AddColumn<string>(
            //    name: "City",
            //    table: "Contracts",
            //    nullable: true);

            //migrationBuilder.AddColumn<DateTime>(
            //    name: "DateOfBirth",
            //    table: "Contracts",
            //    nullable: true);

            //migrationBuilder.AddColumn<DateTime>(
            //    name: "DateSigned",
            //    table: "Contracts",
            //    nullable: true);

            //migrationBuilder.AddColumn<bool>(
            //    name: "IsSigned",
            //    table: "Contracts",
            //    nullable: true);

            //migrationBuilder.AddColumn<string>(
            //    name: "MetaDataArray",
            //    table: "Contracts",
            //    nullable: true);

            //migrationBuilder.AddColumn<string>(
            //    name: "PaymentStatus",
            //    table: "Contracts",
            //    nullable: true);

            //migrationBuilder.AddColumn<string>(
            //    name: "Phone",
            //    table: "Contracts",
            //    nullable: true);

            //migrationBuilder.AddColumn<string>(
            //    name: "PublicToken",
            //    table: "Contracts",
            //    nullable: true);

            //migrationBuilder.AddColumn<string>(
            //    name: "SignatureUrl",
            //    table: "Contracts",
            //    nullable: true);

            //migrationBuilder.AddColumn<string>(
            //    name: "State",
            //    table: "Contracts",
            //    nullable: true);

            //migrationBuilder.AddColumn<string>(
            //    name: "Zipcode",
            //    table: "Contracts",
            //    nullable: true);

            //migrationBuilder.AlterColumn<string>(
            //    name: "Name",
            //    table: "AspNetUserTokens",
            //    nullable: false,
            //    oldClrType: typeof(string),
            //    oldType: "nvarchar(128)",
            //    oldMaxLength: 128);

            //migrationBuilder.AlterColumn<string>(
            //    name: "LoginProvider",
            //    table: "AspNetUserTokens",
            //    nullable: false,
            //    oldClrType: typeof(string),
            //    oldType: "nvarchar(128)",
            //    oldMaxLength: 128);

            //migrationBuilder.AlterColumn<string>(
            //    name: "ProviderKey",
            //    table: "AspNetUserLogins",
            //    nullable: false,
            //    oldClrType: typeof(string),
            //    oldType: "nvarchar(128)",
            //    oldMaxLength: 128);

            //migrationBuilder.AlterColumn<string>(
            //    name: "LoginProvider",
            //    table: "AspNetUserLogins",
            //    nullable: false,
            //    oldClrType: typeof(string),
            //    oldType: "nvarchar(128)",
            //    oldMaxLength: 128);

            //migrationBuilder.AlterColumn<decimal>(
            //    name: "IncomePercentage",
            //    table: "AcademyPrograms",
            //    nullable: false,
            //    oldClrType: typeof(int),
            //    oldType: "int");

            //migrationBuilder.CreateTable(
            //    name: "ConfigValues",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        KeyPair = table.Column<string>(nullable: true),
            //        ValuePair = table.Column<string>(nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_ConfigValues", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "EmploymentQuestionnaires",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        ContractId = table.Column<int>(nullable: true),
            //        EmployerName = table.Column<string>(nullable: true),
            //        Income = table.Column<decimal>(nullable: false),
            //        Address = table.Column<string>(nullable: true),
            //        City = table.Column<string>(nullable: true),
            //        State = table.Column<string>(nullable: true),
            //        Zipcode = table.Column<string>(nullable: true),
            //        EmploymentStartDate = table.Column<DateTime>(nullable: true),
            //        HRContactPerson = table.Column<string>(nullable: true),
            //        HRContactNumber = table.Column<string>(nullable: true),
            //        OfferLetterLink = table.Column<string>(nullable: true),
            //        CreatedOn = table.Column<DateTime>(nullable: true),
            //        UpdatedOn = table.Column<DateTime>(nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_EmploymentQuestionnaires", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_EmploymentQuestionnaires_Contracts_ContractId",
            //            column: x => x.ContractId,
            //            principalTable: "Contracts",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Restrict);
            //    });

            migrationBuilder.CreateTable(
                name: "LearningSolution",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 250, nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: true),
                    ModifiedOn = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LearningSolution", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OTPVerification",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(maxLength: 250, nullable: true),
                    OTPCode = table.Column<int>(maxLength: 10, nullable: false),
                    OTPGeneratedOn = table.Column<DateTime>(nullable: true),
                    OTPValidatedOn = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OTPVerification", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "USState",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 100, nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: true),
                    ModifiedOn = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USState", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "School",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedOn = table.Column<DateTime>(nullable: true),
                    ModifiedOn = table.Column<DateTime>(nullable: true),
                    FirstName = table.Column<string>(maxLength: 100, nullable: true),
                    LastName = table.Column<string>(maxLength: 100, nullable: true),
                    Email = table.Column<string>(maxLength: 250, nullable: true),
                    Name = table.Column<string>(maxLength: 200, nullable: true),
                    IsBasedInUS = table.Column<bool>(nullable: false),
                    Address1 = table.Column<string>(maxLength: 250, nullable: true),
                    Address2 = table.Column<string>(maxLength: 250, nullable: true),
                    City = table.Column<string>(maxLength: 150, nullable: true),
                    State = table.Column<string>(maxLength: 150, nullable: true),
                    USStateId = table.Column<int>(nullable: true),
                    ZipCode = table.Column<string>(nullable: true),
                    IsProspectiveStudent = table.Column<bool>(nullable: false),
                    CPSFirstName = table.Column<string>(maxLength: 100, nullable: true),
                    CPSLastName = table.Column<string>(maxLength: 100, nullable: true),
                    CPSEmail = table.Column<string>(maxLength: 250, nullable: true),
                    IsCPSBasedInUS = table.Column<bool>(nullable: false),
                    CPSName = table.Column<string>(maxLength: 200, nullable: true),
                    CPSProgram = table.Column<string>(maxLength: 200, nullable: true),
                    RecordStatus = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_School", x => x.Id);
                    table.ForeignKey(
                        name: "FK_School_USState_USStateId",
                        column: x => x.USStateId,
                        principalTable: "USState",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SchoolLearningSolution",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SchoolId = table.Column<int>(nullable: true),
                    LearningSolutionId = table.Column<int>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: true),
                    ModifiedOn = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchoolLearningSolution", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SchoolLearningSolution_LearningSolution_LearningSolutionId",
                        column: x => x.LearningSolutionId,
                        principalTable: "LearningSolution",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SchoolLearningSolution_School_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "School",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            //migrationBuilder.CreateIndex(
            //    name: "IX_EmploymentQuestionnaires_ContractId",
            //    table: "EmploymentQuestionnaires",
            //    column: "ContractId");

            migrationBuilder.CreateIndex(
                name: "IX_School_USStateId",
                table: "School",
                column: "USStateId");

            migrationBuilder.CreateIndex(
                name: "IX_SchoolLearningSolution_LearningSolutionId",
                table: "SchoolLearningSolution",
                column: "LearningSolutionId");

            migrationBuilder.CreateIndex(
                name: "IX_SchoolLearningSolution_SchoolId",
                table: "SchoolLearningSolution",
                column: "SchoolId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConfigValues");

            migrationBuilder.DropTable(
                name: "EmploymentQuestionnaires");

            migrationBuilder.DropTable(
                name: "OTPVerification");

            migrationBuilder.DropTable(
                name: "SchoolLearningSolution");

            migrationBuilder.DropTable(
                name: "LearningSolution");

            migrationBuilder.DropTable(
                name: "School");

            migrationBuilder.DropTable(
                name: "USState");

            migrationBuilder.DropColumn(
                name: "LateFees",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "ProcessingFee",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "City",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "DateSigned",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "IsSigned",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "MetaDataArray",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "PublicToken",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "SignatureUrl",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "State",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "Zipcode",
                table: "Contracts");

            migrationBuilder.AlterColumn<decimal>(
                name: "Income",
                table: "Payments",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "Payments",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AspNetUserTokens",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserTokens",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "ProviderKey",
                table: "AspNetUserLogins",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserLogins",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<int>(
                name: "IncomePercentage",
                table: "AcademyPrograms",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal));
        }
    }
}
