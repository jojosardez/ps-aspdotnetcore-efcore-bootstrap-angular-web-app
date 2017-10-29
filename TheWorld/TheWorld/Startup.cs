using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TheWorld.Models;
using TheWorld.Services;

namespace TheWorld
{
    public class Startup
    {
        private IHostingEnvironment _env;
        private IConfigurationRoot _config;

        public Startup(IHostingEnvironment env)
        {
            _env = env;

            var builder = new ConfigurationBuilder()
                .SetBasePath(_env.ContentRootPath)
                .AddJsonFile("config.json")
                .AddEnvironmentVariables();

            _config = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(_config);

            // Dependency injection
            if (_env.IsDevelopment())
            {
                services.AddScoped<IMailService, DebugMailService>();
            }

            // Inject Entity Framework DB Context
            services.AddDbContext<WorldContext>();

            // Repository pattern
            services.AddScoped<IWorldRepository, WorldRepository>();

            // Seeder
            services.AddTransient<WorldContextSeedData>();

            // Using MVC (Microsoft.AspNetCore.Mvc)
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, WorldContextSeedData seeder, ILoggerFactory factory)
        {
            // Error page. For debugging
            if (env.IsEnvironment("Development"))
            {
                app.UseDeveloperExceptionPage();
                // Note: AddDebug is part of Microsoft.Extensions.Logging.Debug
                factory.AddDebug(LogLevel.Information);
            }
            else
            {
                factory.AddDebug(LogLevel.Error);
            }

            // Returning HTML content
            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync("<html><body><h3>Hello World!</h3></body></html>");
            //});

            // Using static files (Microsoft.AspNetCore.StaticFiles)
            //app.UseDefaultFiles();
            app.UseStaticFiles();

            // Using MVC (Microsoft.AspNetCore.Mvc)
            app.UseMvc(config =>
            {
                config.MapRoute(
                    name: "Default",
                    template: "{controller}/{action}/{id?}",
                    defaults: new {controller = "App", action = "Index"});
            });

            // Seed the database
            seeder.EnsureSeedData().Wait();
        }
    }
}
