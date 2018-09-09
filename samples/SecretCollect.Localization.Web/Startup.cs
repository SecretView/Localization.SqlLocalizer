using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
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

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "SearchRecords",
                    template: "Keys/Search",
                    defaults: new { controller = "Keys", action = "Search" }
                );
                routes.MapRoute(
                    name: "MissingKeys",
                    template: "Keys/Missing",
                    defaults: new { controller = "Keys", action = "Missing" }
                );
                routes.MapRoute(
                    name: "addKey",
                    template: "Keys/Add",
                    defaults: new { controller = "Keys", action = "Add" }
                );
                routes.MapRoute(
                    name: "EditKey",
                    template: "Keys/{baseKey}/{mainKey}",
                    defaults: new { controller = "Keys", action = "Edit" }
                );
                routes.MapRoute(
                    name: "SubKey",
                    template: "Keys/{baseKey}",
                    defaults: new { controller = "Keys", action = "SubKey" }
                );
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}"
                );
            });

            using (var scope = app.ApplicationServices.CreateScope())
            {
                var context = scope.ServiceProvider.GetService<SqlLocalizer.Data.LocalizationContext>();
                context.Database.Migrate();
            }
        }
    }
}
