using Interject.Classes;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.IO;
using System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Interject
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
                // options.Authority = "https://test-interject-authapi.azurewebsites.net"; //Interject's auth provider
                // options.Audience = "https://test-interject-authapi.azurewebsites.net/resources"; //Interject's auth provider
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = "https://test-interject-authapi.azurewebsites.net", //Interject's auth provider
                    ValidAudience = "https://test-interject-authapi.azurewebsites.net/resources",
                    ValidateIssuerSigningKey = true,
                    // IssuerSigningKey = new SymmetricSecurityKey("49C1A7E1-0C79-4A89-A3D6-A37998FB86B0".Sha256()),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = false
                };
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = async ctx =>
                     {
                         var putBreakpointHere = true;
                         var exceptionMessage = ctx.Exception;
                     },
                };
            });

            services.AddAuthorization();

            services.AddCors();

            ApplicationOptions appOptions = new();
            Configuration.GetSection(ApplicationOptions.Application).Bind(appOptions);
            services.AddSingleton<ApplicationOptions>(_ => new(appOptions));

            ConnectionStringOptions connections = new();
            Configuration.GetSection(ConnectionStringOptions.Connections).Bind(connections);
            services.AddSingleton<ConnectionStringOptions>(_ => new(connections));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<ErrorMiddleware>();

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

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}