using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rise.Persistence;
using Rise.Persistence.Triggers;

namespace Rise.Server.Tests.Fixtures
{
    public class ApiWebApplicationFactory : WebApplicationFactory<Program>
    {

        public IConfiguration Configuration { get; private set; } = default!;
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration(config =>
            {
                Configuration = new ConfigurationBuilder()
                  .AddUserSecrets("d8435739-e257-4e40-b03f-9b9a66bbc18c")
                  .Build();

                config.AddConfiguration(Configuration);
            });

            // For stubbing services
            builder.ConfigureServices(services =>
            {
                // Check if service exists
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions));

                if (descriptor == null)
                {
                    // Connection
                    services.AddDbContext<ApplicationDbContext>(options =>
                    {
                        options.UseNpgsql(Configuration.GetConnectionString("PostgreSQL"));
                        options.UseTriggers(options => options.AddTrigger<EntityBeforeSaveTrigger>());
                    });

                    // Create and seed database
                    services.AddSingleton<TestDatabaseInitializer>();
                }
            }
            );
        }
    }

    /// <summary>
    /// Singleton service to initialize the database once
    /// </summary>
    internal class TestDatabaseInitializer
    {
        private static readonly object _lock = new();
        private static bool _databaseInitialized = false;

        public TestDatabaseInitializer(IServiceProvider serviceProvider)
        {
            lock (_lock)
            {
                if (!_databaseInitialized)
                {
                    using (var scope = serviceProvider.CreateScope())
                    {
                        var scopedServices = scope.ServiceProvider;
                        var db = scopedServices.GetRequiredService<ApplicationDbContext>();

                        db.Database.EnsureDeleted();
                        db.Database.EnsureCreated();

                        // Ensure seeding is consistent 
                        new Seeder(db).Seed();
                    }

                    _databaseInitialized = true;
                }
            }
        }
    }
}
