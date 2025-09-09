
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.IO;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;


namespace Interject.DataApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Interject Data Api", Version = "v1" });
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            });


            // Uncomment this to add security
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.Authority = Configuration["Authority"];//Interject's auth provider
                // options.Audience = $"{Configuration["Authority"]}/resources"; //Interject's auth provider
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = Configuration["Authority"],//Interject's auth provider
                    // ValidAudience = $"{Configuration["Authority"]}/resources" //Interject's auth provider
                    ValidateAudience = false
                };
            });

            services.AddAuthorization();

            services.AddCors();

            ApplicationOptions appOptions = new();
            Configuration.GetSection(ApplicationOptions.Application).Bind(appOptions);
            services.AddSingleton(appOptions);

            var connectionStrings = Configuration.GetSection("ConnectionStrings").Get<Dictionary<string, string>>();
            services.AddSingleton(connectionStrings);

            services.AddRateLimiter(options =>
            {
                // Default rejection code for all policies
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                // Partition by client IP address (string "unknown" fallback)
                options.AddPolicy("ping", httpContext =>
                {
                    var ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: ip,
                        factory: key => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 5,                     // 5 requests
                            Window = TimeSpan.FromMinutes(1),    // per 60 seconds
                            QueueLimit = 0,                      // do not queue
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                            AutoReplenishment = true
                        });
                });

                // Log and return a small JSON body on rejections
                options.OnRejected = async (context, token) =>
                {
                    var logger = context.HttpContext.RequestServices
                        .GetRequiredService<ILoggerFactory>()
                        .CreateLogger("RateLimit");

                    var ip = context.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                    var path = context.HttpContext.Request.Path.ToString();
                    logger.LogWarning("Rate limit exceeded ip={ip} path={path}", ip, path);

                    // Add Retry-After if provided by limiter
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

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.RoutePrefix = string.Empty;
                    c.DefaultModelsExpandDepth(-1);
                });
            }

            // app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(builder =>
            {
                builder
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(origin => true)
                .AllowCredentials();
            });

            app.UseRateLimiter();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}