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

    }
}
