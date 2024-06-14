using Employees.Domain.AggregateModel;
using Microsoft.EntityFrameworkCore;

public class EmployeeDbContext : DbContext
{
    public DbSet<Employee> Employees { get; set; }

    public EmployeeDbContext(DbContextOptions<EmployeeDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply configurations
        modelBuilder.ApplyConfiguration(new EmployeeConfiguration());
    }
}
