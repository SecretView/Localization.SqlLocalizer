using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using System;
using Microsoft.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace SecretCollect.Localization.Web
{
    public static class IWebHostExtensions
    {
        public static async Task RunAsync(this Task<IWebHost> webHostTask, CancellationToken token = default)
        {
            var webHost = await webHostTask;
            await webHost.RunAsync(token);
        }

        public static async Task<IWebHost> MigrateDbContext<TContext>(this Task<IWebHost> webHostTask, Action<TContext, IServiceProvider> seeder)
            where TContext : DbContext
        {
            var webHost = await webHostTask;
            return await webHost.MigrateDbContext<TContext>(seeder);
        }

        public static async Task<IWebHost> MigrateDbContext<TContext>(this Task<IWebHost> webHostTask, Func<TContext, IServiceProvider, Task> seeder)
            where TContext : DbContext
        {
            var webHost = await webHostTask;
            return await webHost.MigrateDbContext<TContext>(seeder);
        }

        public static async Task<IWebHost> MigrateDbContext<TContext>(this IWebHost webHost, Action<TContext, IServiceProvider> seeder)
            where TContext : DbContext
        {
            Task asyncSeeder(TContext c, IServiceProvider s)
            {
                seeder(c, s);
                return Task.CompletedTask;
            }
            return await webHost.MigrateDbContext<TContext>(asyncSeeder);
        }

        public static async Task<IWebHost> MigrateDbContext<TContext>(this IWebHost webHost, Func<TContext, IServiceProvider, Task> seeder)
            where TContext : DbContext
        {
            using (var scope = webHost.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                var logger = services.GetRequiredService<ILogger<TContext>>();

                var context = services.GetService<TContext>();

                try
                {
                    logger.LogInformation($"Migrating database associated with context {typeof(TContext).Name}");

                    var retry = Policy.Handle<SqlException>()
                         .WaitAndRetryAsync(new TimeSpan[]
                         {
                             TimeSpan.FromSeconds(3),
                             TimeSpan.FromSeconds(5),
                             TimeSpan.FromSeconds(8),
                         });

                    await retry.ExecuteAsync(async () =>
                    {
                        //if the sql server container is not created on run docker compose this
                        //migration can't fail for network related exception. The retry options for DbContext only 
                        //apply to transient exceptions.

                        await context.Database.MigrateAsync();

                        await seeder(context, services);
                    });


                    logger.LogInformation($"Migrated database associated with context {typeof(TContext).Name}");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"An error occurred while migrating the database used on context {typeof(TContext).Name}");
                }
            }

            return webHost;
        }
    }
}
