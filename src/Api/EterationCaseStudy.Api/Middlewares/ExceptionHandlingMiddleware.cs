using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;

namespace EterationCaseStudy.Api.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            using (_logger.BeginScope(new Dictionary<string, object?> { ["TraceId"] = context.TraceIdentifier }))
            {
                _logger.LogError(ex, "Unhandled exception");
            }

            if (context.Response.HasStarted)
            {
                return;
            }

            var status = MapStatusCode(ex);
            var problem = new ProblemDetails
            {
                Title = status >= (int)HttpStatusCode.InternalServerError ? "An unexpected error occurred." : ex.Message,
                Status = status,
                Instance = context.Request.Path,
                Detail = _env.IsDevelopment() ? ex.ToString() : null,
            };

            problem.Extensions["traceId"] = context.TraceIdentifier;

            context.Response.StatusCode = status;
            context.Response.ContentType = "application/problem+json";
            var json = JsonSerializer.Serialize(problem, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            });
            await context.Response.WriteAsync(json);
        }

        private static int MapStatusCode(Exception ex) => ex switch
        {
            ValidationException => (int)HttpStatusCode.BadRequest,
            ArgumentOutOfRangeException => (int)HttpStatusCode.BadRequest,
            ArgumentException => (int)HttpStatusCode.BadRequest,
            InvalidOperationException => (int)HttpStatusCode.BadRequest,
            KeyNotFoundException => (int)HttpStatusCode.NotFound,
            UnauthorizedAccessException => (int)HttpStatusCode.Forbidden,
            NotImplementedException => (int)HttpStatusCode.NotImplemented,
            _ => (int)HttpStatusCode.InternalServerError
        };
    }

    public static class ExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}
