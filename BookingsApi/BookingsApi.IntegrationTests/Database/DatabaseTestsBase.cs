using System.Threading.Tasks;
using BookingsApi;
using Bookings.IntegrationTests.Helper;
using BookingsApi.DAL;
using FizzWare.NBuilder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace Bookings.IntegrationTests.Database
{
    public abstract class DatabaseTestsBase
    {
        private string _databaseConnectionString;
        protected DbContextOptions<BookingsDbContext> BookingsDbContextOptions;
        protected readonly BuilderSettings BuilderSettings = new BuilderSettings();
        protected TestDataManager Hooks { get; private set; }
        
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            var configRootBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .AddUserSecrets<Startup>();
            
            var configRoot = configRootBuilder.Build();
            _databaseConnectionString = configRoot.GetConnectionString("VhBookings");

            var dbContextOptionsBuilder = new DbContextOptionsBuilder<BookingsDbContext>();
            dbContextOptionsBuilder.EnableSensitiveDataLogging();
            dbContextOptionsBuilder.UseSqlServer(_databaseConnectionString);
            BookingsDbContextOptions = dbContextOptionsBuilder.Options;
            
            Hooks = new TestDataManager(BookingsDbContextOptions, "Bookings Api Integration Test");
            
            var context = new BookingsDbContext(BookingsDbContextOptions);
            context.Database.Migrate();
        }
        
        [TearDown]
        public async Task TearDown()
        {
            await Hooks.ClearSeededHearings();
        }
    }
}