namespace Employees.API.Extensions
{
    public static class EmployeesAPIExtenstions
    {
        public static IHostApplicationBuilder AddApplicationServices(this IHostApplicationBuilder builder)
        {
            var services = builder.Services;
            builder.AddSqlServerDbContext<EmployeeDbContext>(Constants.EmployeesDB, opts =>
            {
                opts.ConnectionString = "Data Source=IDL-LT-127\\SQLEXPRESS;Database=employees-sqldb;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False";
            },
                opt =>
            {
                opt.UseModel(Employees.Infrastructure.CompiledModels.EmployeeDbContextModel.Instance);
                opt.LogTo(Console.WriteLine, LogLevel.Information);
            });

            builder.AddServiceDefaults();

            services.AddControllers()
                    .AddJsonOptions(options =>
                    {
                        options.JsonSerializerOptions.TypeInfoResolverChain.Insert(0, EmployeesJsonSerializerContext.Default);
                    });
            services.AddEndpointsApiExplorer();
            //services.AddSwaggerGen(c =>
            //{
            //    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Employees API", Version = "v1" });
            //});
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblyContaining(typeof(Program));
            });

            //inject EmployeesRepository    
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            return builder;
        }
    }


}
