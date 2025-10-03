using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Microsoft.Extensions.Configuration;
using System;


namespace Interject.DataApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var hostBuilder = Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

            /*  -------------- Serilog (optional) --------------
            * To enable built-in structured logging:
            * 1) Set "InterjectLogging:UseBuiltIn": true in appsettings (e.g., appsettings.Development.json)
            * 2) Uncomment this entire block if it is commented out.
            */
            
            try
            {
                // Pre-read config to check the toggle and to have something to log with
                var preConfig = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .Build();

                bool useBuiltIn = preConfig.GetValue<bool>("InterjectLogging:UseBuiltIn");

                if (useBuiltIn)
                {
                    // Bootstrap a minimal logger early so config issues show somewhere
                    Log.Logger = new LoggerConfiguration()
                        .MinimumLevel.Information()
                        .WriteTo.Console()
                        .CreateLogger();

                    hostBuilder = hostBuilder.UseSerilog((ctx, services, cfg) =>
                    {
                        cfg.ReadFrom.Configuration(ctx.Configuration)   // read the "Serilog" section
                            .ReadFrom.Services(services);
                    });

                    Console.WriteLine("Serilog: enabled via InterjectLogging:UseBuiltIn=true");
                }
                else
                {
                    Console.WriteLine("Serilog: disabled (InterjectLogging:UseBuiltIn=false)");
                }
            }
            catch (Exception ex)
            {
                // Do not fail the app just because logging failed to configure
                Console.Error.WriteLine("Serilog configuration failed: " + ex);
            }
            /* -------------- end Serilog (optional) -------------- */
            return hostBuilder;
        }
    }
}
