# Rate Limiting: Gateway and Middleware Options (.NET)

## Quick start

- **Gateway (NGINX):** see [Option 1: API Gateway (NGINX)](#option-1-api-gateway-nginx).  
- **App middleware (.NET 8):** see [Option 2: Application Middleware (.NET 8 built-in)](#option-2-application-middleware-net-8-built-in).

**Example used throughout:** limit `GET /api/v1/Status/Ping` to **5 requests/min per IP**.

---

## Decision guide: Gateway vs Middleware

| Prefer a Gateway when… | Prefer Middleware when… |
|---|---|
| You can put infrastructure **in front of** the app (NGINX/Traefik/API Gateway). | You don’t control infra or need a **drop-in app-level** solution. |
| You want to **absorb/deflect** bursts before they hit the runtime. | You want **per-route/per-tenant** logic close to business code. |
| You may add **WAF, bot, caching** at the edge. | You need **custom keys** (e.g., JWT claim) or feature flags. |

> You can also **combine** both: Gateway rate limiting + app-level safety checks.

---

## Option 1: API Gateway (NGINX)

**Goal:** limit `GET /api/v1/Status/Ping` to **5 req/min per IP**.

### Where to put it
- `limit_req_zone` lives in the **`http { ... }`** context (nginx.conf or an included file).
- The `location` block goes in your **`server { ... }` that fronts the API.

### Snippet

```nginx
# Define a zone that tracks counters per client IP.
# 10m of shared memory holds ~160k states (rough estimate).
limit_req_zone $binary_remote_addr zone=rl_ping:10m rate=5r/m;

# Return 429 on reject (default is 503)
limit_req_status 429;

server {
    listen 80;
    server_name your.api.example.com;

    # Only rate-limit the Ping endpoint
    location = /api/v1/Status/Ping {
        limit_req zone=rl_ping burst=5 nodelay;  # allow brief bursts
        proxy_set_header Host $host;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_pass http://127.0.0.1:5000;
    }

    # Everything else unchanged
    location / {
        proxy_set_header Host $host;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_pass http://127.0.0.1:5000;
    }
}
```

### Traefik & Cloud Gateways

- **Traefik**: attach a `middlewares.ratelimit` with `average=5`, `period=1m`, `burst=5` to the Ping route.
- **AWS API Gateway**: use **Usage Plans** / **Route throttling** (requests per second + burst).
- **Azure API Management**: apply a **rate-limit policy** on the Ping operation (e.g., `calls="5"` per `interval="60"` seconds).
- **WAF/CDN**: pair with Cloudflare/AWS WAF/Azure WAF for volumetric attacks and bot filtering.

---

## Option 2: Application Middleware (.NET 8 built-in)

**What it does**
- Per-IP **fixed 60s window**, **5 req/min**, path-scoped to `/api/v1/Status/Ping`.
- **Hard limit** by default → returns **HTTP 429** and a small JSON error.
- The policy is applied only to the Ping action via attribute.

### Code (already wired in this repo)

**Controller** (`StatusController`):

```csharp
using Microsoft.AspNetCore.RateLimiting;

[HttpGet("Ping")]
[EnableRateLimiting("ping")] // policy name
public IActionResult Ping()
{
    return Ok(new { status = "pong", ts = DateTimeOffset.UtcNow.ToString("o") });
}
```

**Startup.cs — ConfigureServices**:

```csharp
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddPolicy("ping", httpContext =>
    {
        var ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: ip,
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                AutoReplenishment = true
            });
    });

    options.OnRejected = async (context, token) =>
    {
        var logger = context.HttpContext.RequestServices
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger("RateLimit");

        var ip = context.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var path = context.HttpContext.Request.Path.ToString();
        logger.LogWarning("Rate limit exceeded ip={ip} path={path}", ip, path);

        if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
        {
            context.HttpContext.Response.Headers["Retry-After"] =
                Math.Ceiling(retryAfter.TotalSeconds).ToString();
        }

        context.HttpContext.Response.ContentType = "application/json";
        await context.HttpContext.Response.WriteAsync(
            "{\"error\":\"rate_limited\",\"message\":\"Too many requests (5/min). Try again later.\"}",
            token);
    };
});
```

**Startup.cs — Configure**:

```csharp
app.UseRouting();

app.UseCors(builder =>
{
    builder
        .AllowAnyMethod()
        .AllowAnyHeader()
        .SetIsOriginAllowed(origin => true)
        .AllowCredentials();
});

// Attribute-scoped rate limiting
app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});
```

> **Note on client IPs:** This example keys by `RemoteIpAddress`. If you deploy behind a reverse proxy, configure forwarded headers and only trust proxies you control before using `X-Forwarded-For`.

### Testing

**PowerShell**
```powershell
1..7 | % { curl.exe -i "http://localhost:5000/api/v1/Status/Ping" }
```

**Bash**
```bash
for i in {1..7}; do curl -i "http://localhost:5000/api/v1/Status/Ping"; done
```

**Expected**
- First 5: `200 OK` with JSON.
- 6th+: `429 Too Many Requests`, JSON body:
```json
{"error":"rate_limited","message":"Too many requests (5/min). Try again later."}
```
- `Retry-After: 60` header may appear.

---

## Soft-limit (optional)

The built-in middleware enforces a **hard** limit. If you want to **log only** (let requests through), you can add a tiny custom middleware and **remove** the `[EnableRateLimiting]` attribute from Ping.

**Minimal soft-limit middleware** (example):

```csharp
public class SoftRateLimitMiddleware
{
    private static readonly TimeSpan Window = TimeSpan.FromMinutes(1);
    private const int Limit = 5;

    private class Counter { public DateTimeOffset WindowStart; public int Count; }
    private static readonly ConcurrentDictionary<string, Counter> Store = new();

    private readonly RequestDelegate _next;
    private readonly ILogger<SoftRateLimitMiddleware> _log;
    public SoftRateLimitMiddleware(RequestDelegate next, ILogger<SoftRateLimitMiddleware> log)
    { _next = next; _log = log; }

    public async Task Invoke(HttpContext ctx)
    {
        if (ctx.Request.Path.Equals("/api/v1/Status/Ping", StringComparison.OrdinalIgnoreCase))
        {
            var ip = ctx.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var now = DateTimeOffset.UtcNow;

            var c = Store.AddOrUpdate(ip,
                _ => new Counter { WindowStart = now, Count = 1 },
                (_, existing) =>
                {
                    if (now - existing.WindowStart >= Window)
                    { existing.WindowStart = now; existing.Count = 1; }
                    else { existing.Count++; }
                    return existing;
                });

            if (c.Count > Limit)
                _log.LogWarning("Soft limit exceeded ip={ip} path={path} count={count}/{limit}",
                    ip, ctx.Request.Path, c.Count, Limit);
        }

        await _next(ctx);
    }
}
```

**Wire it** in `Startup.Configure` **before** `UseAuthentication()`:

```csharp
app.UseMiddleware<SoftRateLimitMiddleware>();
```

> Keep one approach at a time: either the built-in hard limit with `[EnableRateLimiting]` **or** a custom soft limiter.

---

## Troubleshooting

- **429 never appears**: Ensure `app.UseRateLimiter()` is in the pipeline and Ping has `[EnableRateLimiting("ping")]`.
- **All requests look like the proxy IP**: Configure forwarded headers and trust known proxies before keying on `X-Forwarded-For`.
- **Window edge “spikes”**: Fixed windows can feel bursty; gateways often smooth with `burst`. In-app, consider token-bucket approaches if needed.
- **Observability**: Rejections log WARN with IP/path; add metrics as needed.

---

## Appendix: Production options & headers

- **Bucket-based** libraries (e.g., Bucket4j in Java, token-bucket algos in .NET) for smoother control.
- **Distributed limits** (e.g., Redis) for multi-instance deployments.
- **Headers**: You may emit `X-RateLimit-Limit`, `X-RateLimit-Remaining`, and `Retry-After`. This guide’s minimal example sets `Retry-After` on hard rejections.
- **Security**: Rate limiting complements authentication/authorization and WAFs; it doesn’t replace them.
