namespace Employees.API.Extensions
{
    public static class EmployeesAPIExtenstions
    {
        public static IHostApplicationBuilder AddApplicationServices(this IHostApplicationBuilder builder)
        {
            var services = builder.Services;
            builder.AddSqlServerDbContext<EmployeeDbContext>(Constants.EmployeesDB,
            sqlEFCoreOpts =>
            {
                sqlEFCoreOpts.ConnectionString = @"Data Source=IDL-LT-127\SQLEXPRESS;Database=employees-sqldb;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False";
                sqlEFCoreOpts.DisableRetry = true;
            },
            dbContextOpts =>
            {
                //dbContextOpts.UseModel(Infrastructure.CompiledModels.EmployeeDbContextModel.Instance);
                dbContextOpts.EnableDetailedErrors();
            });

            builder.AddServiceDefaults();

            services.AddControllers()
                    .AddJsonOptions(options =>
                    {
                        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
                        //to keep json property names as it is
                        options.JsonSerializerOptions.PropertyNamingPolicy = null;

                        //only use this code for nativeAOT application
                        //but there are some compatible issues with Reflection
                        //options.JsonSerializerOptions.TypeInfoResolverChain.Insert(0, EmployeesJsonSerializerContext.Default);
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
