using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace SecretCollect.Localization.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration) => Configuration = configuration;

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddGlobalization(optionsBuilder => optionsBuilder.UseSqlServer(Configuration.GetConnectionString("LocalizationDb")));
            services.AddMvc()
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization();
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
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseRequestLocalization();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(routes =>
            {
                routes.MapControllerRoute(
                    name: "SearchRecords",
                    pattern: "Keys/Search",
                    defaults: new { controller = "Keys", action = "Search" }
                );
                routes.MapControllerRoute(
                    name: "MissingKeys",
                    pattern: "Keys/Missing",
                    defaults: new { controller = "Keys", action = "Missing" }
                );
                routes.MapControllerRoute(
                    name: "addKey",
                    pattern: "Keys/Add",
                    defaults: new { controller = "Keys", action = "Add" }
                );
                routes.MapControllerRoute(
                    name: "EditKey",
                    pattern: "Keys/{baseKey}/{mainKey}",
                    defaults: new { controller = "Keys", action = "Edit" }
                );
                routes.MapControllerRoute(
                    name: "SubKey",
                    pattern: "Keys/{baseKey}",
                    defaults: new { controller = "Keys", action = "SubKey" }
                );
                routes.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}"
                );
            });
        }
    }
}
