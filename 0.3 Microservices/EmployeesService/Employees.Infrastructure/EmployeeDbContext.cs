using Employees.Domain.AggregateModel;
using LearningAspire.Commons.Domain.SeedWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class EmployeeDbContext : DbContext, IUnitOfWork
{
    public DbSet<Employee> Employees { get; set; }
    private IMediator _mediator;
    public EmployeeDbContext(IMediator mediator, DbContextOptions<EmployeeDbContext> options)
        : base(options)
    {
#if !DEBUG
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
#else
        _mediator = mediator;//only run this code when generating the EF Core migrations in design mode
#endif

    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        base.OnModelCreating(modelBuilder);

        // Apply configurations
        modelBuilder.ApplyConfiguration(new EmployeeConfiguration());
    }
    public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        // Dispatch Domain Events collection. 
        // Choices:
        // A) Right BEFORE committing data (EF SaveChanges) into the DB will make a single transaction including  
        // side effects from the domain event handlers which are using the same DbContext with "InstancePerLifetimeScope" or "scoped" lifetime
        // B) Right AFTER committing data (EF SaveChanges) into the DB will make multiple transactions. 
        // You will need to handle eventual consistency and compensatory actions in case of failures in any of the Handlers. 

        await _mediator.DispatchDomainEventsAsync(this);

        // After executing this line all the changes (from the Command Handler and Domain Event Handlers) 
        // performed through the DbContext will be committed

        // _ = await base.SaveChangesAsync(cancellationToken);

        return true;
    }
}
