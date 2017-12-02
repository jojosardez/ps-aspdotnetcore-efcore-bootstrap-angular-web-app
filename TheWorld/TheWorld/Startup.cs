using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using TheWorld.Models;
using TheWorld.Services;
using TheWorld.ViewModels;

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

            // Register GeoCoordsService 
            services.AddTransient<GeoCoordsService>();

            // Seeder
            services.AddTransient<WorldContextSeedData>();

            // Using MVC (Microsoft.AspNetCore.Mvc)
            services.AddMvc(config =>
                    {
                        // To only allow https connection
                        if (_env.IsProduction())
                        {
                            config.Filters.Add(new RequireHttpsAttribute());
                        }
                    }
                )
                // Set properties to camel case (note: in this version of MVC, by default, it is already using CamelCasePropertyNamesContractResolver)
                .AddJsonOptions(config =>
                {
                    config.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                });

            // Add ASP .Net Core Identity
            services.AddIdentity<WorldUser, IdentityRole>(config =>
                {
                    config.User.RequireUniqueEmail = true;
                    config.Password.RequiredLength = 8;
                    config.Cookies.ApplicationCookie.LoginPath = "/Auth/Login";
                })
                .AddEntityFrameworkStores<WorldContext>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, WorldContextSeedData seeder, ILoggerFactory factory)
        {
            // Configuring AutoMapper mapping
            Mapper.Initialize(config =>
            {
                config.CreateMap<TripViewModel, Trip>().ReverseMap();
                config.CreateMap<StopViewModel, Stop>().ReverseMap();
            });

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


            // Use ASP .Net Core Identity
            app.UseIdentity();

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
