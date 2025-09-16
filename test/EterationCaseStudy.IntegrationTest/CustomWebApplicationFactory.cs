using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using EterationCaseStudy.Infrastructure.Persistence;
using EterationCaseStudy.Domain.Entities;
using EterationCaseStudy.Application.Common.Security;
using Microsoft.AspNetCore.Authentication;

namespace EterationCaseStudy.IntegrationTest;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        Environment.SetEnvironmentVariable("UseInMemoryDatabase", "true");
        builder.ConfigureAppConfiguration((ctx, cfg) =>
        {
            cfg.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "super_secret_key",
                ["Jwt:Issuer"] = "EterationCaseStudy",
                ["Jwt:Audience"] = "EterationClients",
            });
        });
        builder.ConfigureServices(services =>
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "Test";
                options.DefaultChallengeScheme = "Test";
            }).AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });

            var toRemove = services.Where(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>)
                                            || d.ServiceType == typeof(AppDbContext)).ToList();
            foreach (var d in toRemove)
            {
                services.Remove(d);
            }

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase($"IntegrationTests_{Guid.NewGuid()}");
            });

            using var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            if (!db.Roles.Any())
            {
                var adminRole = new Role("Admin") { Id = 1 };
                var userRole = new Role("User") { Id = 2 };
                db.Roles.AddRange(adminRole, userRole);
                var admin = new User("admin", "admin@eterationcasestudy.com", "Admin");
                admin.SetPasswordHash(PasswordHasher.Hash("Admin123"));
                admin.SetRole(adminRole);
                db.Users.Add(admin);
                db.SaveChanges();
            }
        });
    }
}
