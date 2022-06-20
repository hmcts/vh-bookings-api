using System.Threading.Tasks;
using BookingsApi.DAL;
using BookingsApi.IntegrationTests.Helper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace BookingsApi.IntegrationTests.Database
{
    public abstract class DatabaseTestsBase
    {
        private string _databaseConnectionString;
        protected DbContextOptions<BookingsDbContext> BookingsDbContextOptions;
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
            await Hooks.ClearJudiciaryPersonsAsync();
        }
    }
}