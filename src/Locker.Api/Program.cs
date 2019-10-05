using Locker.DataAccess.Features.Shared;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Locker.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                BsonClassMapConfiguration.ConfigureDbClassMap();
                var dbSeeder = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();

                dbSeeder.ConfigureDbCollections().Wait();
                dbSeeder.Seed().Wait();
            }

            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost
                .CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}