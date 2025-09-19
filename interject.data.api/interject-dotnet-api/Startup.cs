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
            string authority = Configuration["Authority"];
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.Authority = authority;//Interject's auth provider
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = authority,//Interject's auth provider
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

            app.UseMiddleware<CustomMiddleware>();

            app.UseRouting();

            app.Use(async (context, next) =>
            {
                foreach (var header in context.Request.Headers)
                {
                    Console.WriteLine($"{header.Key}: {header.Value}");
                }
                await next.Invoke();
            });

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