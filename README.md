# Localization.SqlLocalizer
Repository to enable localizations using EF &amp; SQL in an ASP.NET Core application

**NuGet:** [![NuGet](https://img.shields.io/nuget/dt/SecretCollect.Localization.SqlLocalizer.svg)](https://www.nuget.org/packages/SecretCollect.Localization.SqlLocalizer/)


## How to use
In your Startup.cs, add the services by calling:

    services.AddGlobalization(...);

With the optionsBuilder delegate to setup the database server.

For example:

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddGlobalization(optionsBuilder => optionsBuilder.UseSqlServer(Configuration.GetConnectionString("LocalizationDb")));
        
        services.AddMvc()
            .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
            .AddDataAnnotationsLocalization();
    }

And you need to enable requestlocalization by calling:

    app.UseRequestLocalization();

In the AppSettings.json you need to specify which (non-UI) culture you wish to support, the cookie name and the cookie domain.

    {
      "Globalization": {
        "CookieName": "Language",
        "CookieDomain": "localhost",
        "SupportedCultures": [
            "en-US"
        ],
        "CacheTimeInMinutes": 15
      }
    }


### Notes on database migrations

But of course the database needs to be initialized, you thus need to call `context.Database.Migrate()`. As per https://github.com/aspnet/EntityFrameworkCore/issues/9033#issuecomment-317063370 it is bad form to do this in your request pipeline code. Therefore our sample application uses an extension method to migrate the database in the Program.cs:

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
