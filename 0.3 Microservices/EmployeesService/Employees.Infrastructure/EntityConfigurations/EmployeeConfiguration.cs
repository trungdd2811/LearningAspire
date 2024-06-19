using Employees.Domain.AggregateModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        // Set the table name
        builder.ToTable("Employees");

        // Set the primary key
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200);

        // Configure the relationships
        // Assuming a self-referencing many-to-many relationship for Managers and DirectReports
        // This requires a join table. Here's an example configuration:

        builder.HasMany(e => e.DirectReports)
            .WithMany(e => e.Managers)
            .UsingEntity<Dictionary<string, object>>(
                "EmployeeHierarchy",
                j => j.HasOne<Employee>().WithMany().HasForeignKey("DirectReportId"),
                j => j.HasOne<Employee>().WithMany().HasForeignKey("ManagerId"),
                j =>
                {
                    j.HasKey("DirectReportId", "ManagerId");
                    j.ToTable("EmployeeHierarchies"); // Name of the join table
                });
    }
}
