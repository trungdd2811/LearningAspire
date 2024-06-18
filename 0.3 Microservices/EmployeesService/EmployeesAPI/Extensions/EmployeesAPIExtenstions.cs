namespace Employees.API.Extensions
{
    public static class EmployeesAPIExtenstions
    {
        public static IHostApplicationBuilder AddApplicationServices(this IHostApplicationBuilder builder)
        {
            var services = builder.Services;

            builder.AddSqlServerDbContext<EmployeeDbContext>(Constants.EmployeesDB);
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
