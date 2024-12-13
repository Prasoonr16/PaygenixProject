using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaygenixProject.Migrations
{
    /// <inheritdoc />
    public partial class modifymodels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastLogin",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "PayrollPeriod",
                table: "ComplianceReports",
                newName: "StartPeriod");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndPeriod",
                table: "ComplianceReports",
                type: "date",
                maxLength: 50,
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "EndPeriod",
                table: "ComplianceReports");

            migrationBuilder.RenameColumn(
                name: "StartPeriod",
                table: "ComplianceReports",
                newName: "PayrollPeriod");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastLogin",
                table: "Users",
                type: "datetime2",
                nullable: true);
        }
    }
}
