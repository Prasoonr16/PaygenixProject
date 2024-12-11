using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaygenixProject.Migrations
{
    /// <inheritdoc />
    public partial class newDB3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Activestatus",
                table: "Employees",
                newName: "ActiveStatus");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ActiveStatus",
                table: "Employees",
                newName: "Activestatus");
        }
    }
}
