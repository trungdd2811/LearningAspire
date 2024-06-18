﻿// <auto-generated />
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

#pragma warning disable 219, 612, 618
#nullable disable

namespace Employees.Infrastructure.CompiledModels
{
    public partial class EmployeeDbContextModel
    {
        partial void Initialize()
        {
            var employee = EmployeeEntityType.Create(this);

            EmployeeEntityType.CreateAnnotations(employee);

            AddAnnotation("ProductVersion", "8.0.4");
            AddAnnotation("Relational:MaxIdentifierLength", 128);
            AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);
            AddRuntimeAnnotation("Relational:RelationalModel", CreateRelationalModel());
        }

        private IRelationalModel CreateRelationalModel()
        {
            var relationalModel = new RelationalModel(this);

            var employee = FindEntityType("Employees.Domain.AggregateModel.Employee")!;

            var defaultTableMappings = new List<TableMappingBase<ColumnMappingBase>>();
            employee.SetRuntimeAnnotation("Relational:DefaultMappings", defaultTableMappings);
            var employeesDomainAggregateModelEmployeeTableBase = new TableBase("Employees.Domain.AggregateModel.Employee", null, relationalModel);
            var idColumnBase = new ColumnBase<ColumnMappingBase>("Id", "int", employeesDomainAggregateModelEmployeeTableBase);
            employeesDomainAggregateModelEmployeeTableBase.Columns.Add("Id", idColumnBase);
            var nameColumnBase = new ColumnBase<ColumnMappingBase>("Name", "nvarchar(200)", employeesDomainAggregateModelEmployeeTableBase);
            employeesDomainAggregateModelEmployeeTableBase.Columns.Add("Name", nameColumnBase);
            relationalModel.DefaultTables.Add("Employees.Domain.AggregateModel.Employee", employeesDomainAggregateModelEmployeeTableBase);
            var employeesDomainAggregateModelEmployeeMappingBase = new TableMappingBase<ColumnMappingBase>(employee, employeesDomainAggregateModelEmployeeTableBase, true);
            employeesDomainAggregateModelEmployeeTableBase.AddTypeMapping(employeesDomainAggregateModelEmployeeMappingBase, false);
            defaultTableMappings.Add(employeesDomainAggregateModelEmployeeMappingBase);
            RelationalModel.CreateColumnMapping((ColumnBase<ColumnMappingBase>)idColumnBase, employee.FindProperty("Id")!, employeesDomainAggregateModelEmployeeMappingBase);
            RelationalModel.CreateColumnMapping((ColumnBase<ColumnMappingBase>)nameColumnBase, employee.FindProperty("Name")!, employeesDomainAggregateModelEmployeeMappingBase);

            var tableMappings = new List<TableMapping>();
            employee.SetRuntimeAnnotation("Relational:TableMappings", tableMappings);
            var employeesTable = new Table("Employees", null, relationalModel);
            var idColumn = new Column("Id", "int", employeesTable);
            employeesTable.Columns.Add("Id", idColumn);
            var nameColumn = new Column("Name", "nvarchar(200)", employeesTable);
            employeesTable.Columns.Add("Name", nameColumn);
            var pK_Employees = new UniqueConstraint("PK_Employees", employeesTable, new[] { idColumn });
            employeesTable.PrimaryKey = pK_Employees;
            var pK_EmployeesUc = RelationalModel.GetKey(this,
                "Employees.Domain.AggregateModel.Employee",
                new[] { "Id" });
            pK_Employees.MappedKeys.Add(pK_EmployeesUc);
            RelationalModel.GetOrCreateUniqueConstraints(pK_EmployeesUc).Add(pK_Employees);
            employeesTable.UniqueConstraints.Add("PK_Employees", pK_Employees);
            relationalModel.Tables.Add(("Employees", null), employeesTable);
            var employeesTableMapping = new TableMapping(employee, employeesTable, true);
            employeesTable.AddTypeMapping(employeesTableMapping, false);
            tableMappings.Add(employeesTableMapping);
            RelationalModel.CreateColumnMapping(idColumn, employee.FindProperty("Id")!, employeesTableMapping);
            RelationalModel.CreateColumnMapping(nameColumn, employee.FindProperty("Name")!, employeesTableMapping);
            return relationalModel.MakeReadOnly();
        }
    }
}
