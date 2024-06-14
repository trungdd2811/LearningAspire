using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

public partial class AddEmployeeAndHierarchy : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Employees",
            columns: table => new
            {
                Id = table.Column<Guid>(nullable: false),
                IdentityGuid = table.Column<string>(maxLength: 36, nullable: false),
                Name = table.Column<string>(maxLength: 200, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Employees", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "EmployeeHierarchies",
            columns: table => new
            {
                DirectReportId = table.Column<Guid>(nullable: false),
                ManagerId = table.Column<Guid>(nullable: false)
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
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_EmployeeHierarchies_ManagerId",
            table: "EmployeeHierarchies",
            column: "ManagerId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "EmployeeHierarchies");

        migrationBuilder.DropTable(
            name: "Employees");
    }
}
