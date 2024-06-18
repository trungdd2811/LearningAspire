using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Employees.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EmployeesAddDBInitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeHierarchies",
                columns: table => new
                {
                    DirectReportId = table.Column<int>(type: "int", nullable: false),
                    ManagerId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeHierarchies", x => new { x.DirectReportId, x.ManagerId });
                    table.ForeignKey(
                        name: "FK_EmployeeHierarchies_Employees_DirectReportId",
                        column: x => x.DirectReportId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmployeeHierarchies_Employees_ManagerId",
                        column: x => x.ManagerId,
                        principalTable: "Employees",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeHierarchies_ManagerId",
                table: "EmployeeHierarchies",
                column: "ManagerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmployeeHierarchies");

            migrationBuilder.DropTable(
                name: "Employees");
        }
    }
}
