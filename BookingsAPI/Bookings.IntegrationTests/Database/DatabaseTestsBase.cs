using Bookings.API;
using Bookings.DAL;
using Bookings.IntegrationTests.Helper;
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
        protected VideoHearingHooks Hooks { get; private set; }
        
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _databaseConnectionString = new ConfigurationBuilder().AddUserSecrets<Startup>().Build()
                .GetConnectionString("VhBookings");

            var dbContextOptionsBuilder = new DbContextOptionsBuilder<BookingsDbContext>();
            dbContextOptionsBuilder.EnableSensitiveDataLogging();
            dbContextOptionsBuilder.UseSqlServer(_databaseConnectionString);
            BookingsDbContextOptions = dbContextOptionsBuilder.Options;
            
            Hooks = new VideoHearingHooks(BookingsDbContextOptions);
            
            var context = new BookingsDbContext(BookingsDbContextOptions);
            context.Database.Migrate();
        }
    }
}