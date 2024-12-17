using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaygenixProject.Migrations
{
    /// <inheritdoc />
    public partial class addedmanager : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ManagerUserID",
                table: "Employees",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_ManagerUserID",
                table: "Employees",
                column: "ManagerUserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Users_ManagerUserID",
                table: "Employees",
                column: "ManagerUserID",
                principalTable: "Users",
                principalColumn: "UserID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Users_ManagerUserID",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_ManagerUserID",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "ManagerUserID",
                table: "Employees");
        }
    }
}
