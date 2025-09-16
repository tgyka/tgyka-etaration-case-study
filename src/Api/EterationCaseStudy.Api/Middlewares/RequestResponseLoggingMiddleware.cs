using System.Diagnostics;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace EterationCaseStudy.Api.Middlewares
{
    public class RequestResponseLoggingMiddleware
    {
        private const int MaxBodyLength = 4096;
        private static readonly HashSet<string> BodyLoggableContentTypes =
            new(StringComparer.OrdinalIgnoreCase) { "application/json", "text/plain", "text/html", "text/css", "text/xml" };

        private readonly RequestDelegate _next;
        private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

        public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var sw = Stopwatch.StartNew();

            var reqInfo = await ReadRequestAsync(context.Request);

            var originalBody = context.Response.Body;
            await using var memStream = new MemoryStream();
            context.Response.Body = memStream;

            try
            {
                await _next(context);
                sw.Stop();
            }
            finally
            {
                var respInfo = await ReadResponseAsync(context.Response);

                _logger.LogInformation(
                    "HTTP {Method} {Path}{Query} => {StatusCode} in {Elapsed} ms | reqLen={ReqLen} respLen={RespLen}{ReqBody}{RespBody}",
                    reqInfo.Method,
                    reqInfo.Path,
                    reqInfo.QueryString,
                    respInfo.StatusCode,
                    sw.ElapsedMilliseconds,
                    reqInfo.Length,
                    respInfo.Length,
                    reqInfo.LogBody,
                    respInfo.LogBody);

                memStream.Position = 0;
                await memStream.CopyToAsync(originalBody);
                context.Response.Body = originalBody;
            }
        }

        private static bool ShouldLogBody(string? contentType)
        {
            if (string.IsNullOrWhiteSpace(contentType)) return false;
            var semi = contentType.IndexOf(';');
            var pure = semi > 0 ? contentType[..semi] : contentType;
            return BodyLoggableContentTypes.Contains(pure);
        }

        private static async Task<(string Method, string Path, string QueryString, int Length, string LogBody)> ReadRequestAsync(HttpRequest request)
        {
            var method = request.Method;
            var path = request.Path.HasValue ? request.Path.Value! : string.Empty;
            var query = request.QueryString.HasValue ? request.QueryString.Value! : string.Empty;

            if (!ShouldLogBody(request.ContentType))
                return (method, path, query, 0, string.Empty);

            request.EnableBuffering();
            using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
            var buffer = new char[Math.Min(MaxBodyLength, (int)(request.ContentLength ?? MaxBodyLength))];
            var read = await reader.ReadBlockAsync(buffer, 0, buffer.Length);
            request.Body.Position = 0;
            var body = new string(buffer, 0, read);

            if ((request.ContentLength ?? read) > MaxBodyLength)
                body += "... (truncated)";

            return (method, path, query, read, $" | reqBody={body}");
        }

        private static async Task<(int StatusCode, int Length, string LogBody)> ReadResponseAsync(HttpResponse response)
        {
            var status = response.StatusCode;
            var length = 0;
            string logBody = string.Empty;

            if (ShouldLogBody(response.ContentType))
            {
                response.Body.Position = 0;
                using var reader = new StreamReader(response.Body, Encoding.UTF8, leaveOpen: true);
                var text = await reader.ReadToEndAsync();
                length = text.Length;
                if (text.Length > MaxBodyLength)
                    text = text.Substring(0, MaxBodyLength) + "... (truncated)";
                logBody = $" | respBody={text}";
            }

            return (status, length, logBody);
        }
    }
}