using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaygenixProject.Migrations
{
    /// <inheritdoc />
    public partial class updatedDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PayrollIssued",
                table: "ComplianceReports");

            migrationBuilder.AddColumn<DateTime>(
                name: "PayrollPeriod",
                table: "ComplianceReports",
                type: "datetime2",
                maxLength: 50,
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PayrollPeriod",
                table: "ComplianceReports");

            migrationBuilder.AddColumn<DateOnly>(
                name: "PayrollIssued",
                table: "ComplianceReports",
                type: "date",
                maxLength: 50,
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));
        }
    }
}
