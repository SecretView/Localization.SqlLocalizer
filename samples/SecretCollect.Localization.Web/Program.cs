using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using SecretCollect.Localization.SqlLocalizer;
using SecretCollect.Localization.SqlLocalizer.Data;
using System;
using System.Threading.Tasks;

namespace SecretCollect.Localization.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            await CreateWebHostBuilder(args)
                .Build()
                .MigrateDbContext<LocalizationContext>((context, serviceProvider) =>
                {
                    var preloader = serviceProvider.GetRequiredService<Preloader>();
                    preloader.CacheRecentlyUsed(maxAge: TimeSpan.FromDays(1), absoluteExpirationRelativeToNow: TimeSpan.FromDays(365));
                })
                .RunAsync();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseApplicationInsights()
                .UseStartup<Startup>();
    }
}
