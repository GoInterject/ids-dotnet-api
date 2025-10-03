using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Interject.DataApi
{
    /// <summary>
    /// Logs minimal, safe metadata when a request results in 401/403.
    /// Does NOT log credentials, tokens, or raw Authorization headers.
    /// </summary>
    public class AuthFailureTelemetryMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AuthFailureTelemetryMiddleware> _log;

        public AuthFailureTelemetryMiddleware(RequestDelegate next, ILogger<AuthFailureTelemetryMiddleware> log)
        {
            _next = next;
            _log = log;
        }

        public async Task Invoke(HttpContext ctx)
        {
            await _next(ctx);

            var status = ctx.Response?.StatusCode ?? 0;
            if (status == StatusCodes.Status401Unauthorized || status == StatusCodes.Status403Forbidden)
            {
                // NOTE: If behind a proxy, consider ForwardedHeaders middleware and reading X-Forwarded-For instead.
                var ip = ctx.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                var ua = ctx.Request.Headers.TryGetValue("User-Agent", out var u) ? u.ToString() : "";
                // Pick a couple of SAFE claims if present (avoid logging emails/tokens/JWTs)
                string sub = ctx.User?.Claims?.FirstOrDefault(c => c.Type == "sub")?.Value ?? "";
                string clientId = ctx.User?.Claims?.FirstOrDefault(c => c.Type == "ids_client_id")?.Value ?? "";

                // Trim noisy values
                if (sub.Length > 64) sub = sub[..64];
                if (clientId.Length > 64) clientId = clientId[..64];

                // No sensitive data: do NOT log Authorization or raw JWTs
                _log.LogWarning("auth_failure status={status} method={method} path={path} ip={ip} userAgent={ua} sub={sub} ids_client_id={clientId}",
                    status, ctx.Request?.Method, ctx.Request?.Path.Value, ip, ua, sub, clientId);
            }
        }
    }
}
