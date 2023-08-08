using BookingsApi.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Testing.Common.Configuration;

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
            var configRoot = ConfigRootBuilder.Build();
            
            _databaseConnectionString = configRoot.GetConnectionString("VhBookings");
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<BookingsDbContext>();
            dbContextOptionsBuilder.UseSqlServer(_databaseConnectionString);
            
            BookingsDbContextOptions = dbContextOptionsBuilder.Options;
            var context = new BookingsDbContext(BookingsDbContextOptions);
            context.Database.Migrate();
            
            Hooks = new TestDataManager(BookingsDbContextOptions, "Bookings Api Integration Test");
        }
        
        [TearDown]
        public async Task TearDown()
        {
            await Hooks.ClearSeededHearings();
            await Hooks.ClearJudiciaryPersonsAsync();
            await Hooks.ClearJusticeUserRolesAsync();
            await Hooks.ClearSeededJusticeUsersAsync();
            await Hooks.ClearAllocationsAsync();
        }
    }
}