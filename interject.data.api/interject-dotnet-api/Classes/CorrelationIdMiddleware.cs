using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Serilog.Context;

namespace Interject.DataApi
{
    public class CorrelationIdMiddleware
    {
        public const string HeaderName = "X-Correlation-ID";
        private readonly RequestDelegate _next;

        public CorrelationIdMiddleware(RequestDelegate next) => _next = next;

        public async Task Invoke(HttpContext context)
        {
            // Read incoming correlation id or create one
            var correlationId = context.Request.Headers.TryGetValue(HeaderName, out var values)
                && !string.IsNullOrWhiteSpace(values.ToString())
                ? values.ToString()
                : Guid.NewGuid().ToString("n");

            // Expose on the response
            context.Response.OnStarting(() =>
            {
                context.Response.Headers[HeaderName] = correlationId;
                return Task.CompletedTask;
            });

            // Push into the logging scope for Serilog (only used if Serilog enabled)
            using (LogContext.PushProperty("CorrelationId", correlationId))
            {
                await _next(context);
            }
        }
    }
}
