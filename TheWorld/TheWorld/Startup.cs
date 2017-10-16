using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TheWorld.Services;

namespace TheWorld
{
    public class Startup
    {
        private IHostingEnvironment _env;

        public Startup(IHostingEnvironment env)
        {
            _env = env;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // Dependency injection
            if (_env.IsDevelopment())
            {
                services.AddScoped<IMailService, DebugMailService>();
            }

            // Using MVC (Microsoft.AspNetCore.Mvc)
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            // Error page. For debugging
            if (env.IsEnvironment("Development"))
            {
                app.UseDeveloperExceptionPage();
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
        }
    }
}
