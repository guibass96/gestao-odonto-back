using GBarber.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GBarber.WebApi.ConfigDependency.DB
{


    public static class DependencyInjectionDB
    {
        public static IServiceCollection AddApplicationServicesDB(this IServiceCollection services,WebApplicationBuilder builder)
        {
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");


            services.AddDbContext<AppDbContext>(options =>
                 options.UseNpgsql(
                     connectionString,
                     b => b.MigrationsAssembly("GBarber.SQL") 
                 ));

            return services;
        }
    }
}
