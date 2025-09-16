using System.Reflection;
using System.Text;
using Serilog;
using Microsoft.OpenApi.Models;
using EterationCaseStudy.Api.Auth;
using EterationCaseStudy.Application.Abstractions;
using EterationCaseStudy.Application.Common.Security;
using EterationCaseStudy.Domain.Repositories;
using EterationCaseStudy.Domain.Entities;
using EterationCaseStudy.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using EterationCaseStudy.Api.Middlewares;
using FluentValidation;
using MediatR;
using EterationCaseStudy.Application.Common.Behaviors;

var builder = WebApplication.CreateBuilder(args);
var logsPath = Path.Combine(AppContext.BaseDirectory, "Logs");
Directory.CreateDirectory(logsPath);
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(Path.Combine(logsPath, "api-.log"), rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7, shared: true)
    .CreateLogger();
builder.Host.UseSerilog();

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Eteration Case Study API", Version = "v1" });

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter 'Bearer {token}'",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
    };
    c.AddSecurityDefinition("Bearer", securityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, Array.Empty<string>() }
    });
});

builder.Services.AddInfrastructure(builder.Configuration);

var appAssembly = Assembly.Load("EterationCaseStudy.Application");
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(appAssembly));
builder.Services.AddValidatorsFromAssembly(appAssembly);
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, CurrentUser>();

var jwtSection = builder.Configuration.GetSection("Jwt");
var key = jwtSection.GetValue<string>("Key") ?? "super_secret_key";
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.FromMinutes(1)
    };
});
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Eteration Case Study API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseGlobalExceptionHandling();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<RequestResponseLoggingMiddleware>();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var users = scope.ServiceProvider.GetRequiredService<IUserRepository>();
    var roles = scope.ServiceProvider.GetRequiredService<IRoleRepository>();
    var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
    var any = (await users.ListAsync()).Any();
    if (!any)
    {
        var adminRole = (await roles.ListAsync(r => r.Name == "Admin")).FirstOrDefault();
        var userRole = (await roles.ListAsync(r => r.Name == "User")).FirstOrDefault();
        if (adminRole == null || userRole == null)
        {
            adminRole ??= new Role("Admin") { Id = 1 };
            userRole ??= new Role("User") { Id = 2 };
            await roles.AddAsync(adminRole);
            await roles.AddAsync(userRole);
            await uow.SaveChangesAsync();
        }

        var admin = new User("admin", "admin@eterationcasestudy.com", "Admin");
        admin.SetPasswordHash(PasswordHasher.Hash("Admin123"));
        admin.SetRole(adminRole!);
        await users.AddAsync(admin);
        await uow.SaveChangesAsync();
    }
}

app.Run();
