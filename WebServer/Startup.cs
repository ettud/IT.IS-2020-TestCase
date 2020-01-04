using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Patterns;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebServer.Services;

namespace WebServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddSingleton((sp) =>
            {
                var connectionString = Configuration.GetValue<string>("ConnectionStrings:DefaultConnection");
                var builder = new DbContextOptionsBuilder().UseNpgsql(connectionString);
                return new LogBasePresenter.LogBasePresenter(builder.Options);
            });
            services.AddSingleton<IDateTimeService>(new DateTimeService());
            services.AddSingleton((sp) => new AdminService(sp.GetRequiredService<LogBasePresenter.LogBasePresenter>()));
            services.AddSingleton((sp) => new ReportsService(sp.GetRequiredService<LogBasePresenter.LogBasePresenter>()));
            services.AddHttpClient();
            services.AddHttpContextAccessor();

            //services.AddMvc(option => option.EnableEndpointRouting = false);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();

            const bool disableBlazorEnableJustMvc = false;
            if (disableBlazorEnableJustMvc)
            {
                app.UseMvc(routes =>
                {
                    routes.MapRoute(
                        name: "default",
                        template: "{controller=Home}/{action=Index}/{id?}");
                });
            }
            else
            {
                app.UseRouting();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllerRoute("api", "api/{Controller}/{Action}");
                    endpoints.MapBlazorHub();
                    endpoints.MapFallbackToPage("/_Host");
                });
            }
        }
    }
}
