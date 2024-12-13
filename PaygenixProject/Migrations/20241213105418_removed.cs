using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaygenixProject.Migrations
{
    /// <inheritdoc />
    public partial class removed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TaxAmount",
                table: "Payrolls");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TaxAmount",
                table: "Payrolls",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
