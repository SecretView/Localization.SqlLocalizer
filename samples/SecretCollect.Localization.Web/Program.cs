using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using SecretCollect.Localization.SqlLocalizer.Data;

namespace SecretCollect.Localization.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args)
                .Build()
                .MigrateDatabase<LocalizationContext>()
                .Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseApplicationInsights()
                .UseStartup<Startup>();
    }
}
