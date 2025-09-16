using EterationCaseStudy.Domain.Repositories;
using EterationCaseStudy.Infrastructure.Persistence;
using EterationCaseStudy.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EterationCaseStudy.Infrastructure
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var useInMemory = configuration.GetValue<bool>("UseInMemoryDatabase");
            var connectionString = configuration.GetConnectionString("Default");

            if (useInMemory || string.IsNullOrWhiteSpace(connectionString))
            {
                services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("EterationCaseStudyDb"));
            }
            else
            {
                services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));
            }

            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
