using BookingsApi.DAL;
using BookingsApi.IntegrationTests.Helper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using Testing.Common.Configuration;

namespace BookingsApi.IntegrationTests;

public class ApiTest
{
    protected VhApiWebApplicationFactory Application = null!;
    protected DbContextOptions<BookingsDbContext> BookingsDbContextOptions { get; set; }
    protected TestDataManager Hooks { get; private set; }
    private IConfigurationRoot _configRoot;
    private string _databaseConnectionString;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        RegisterSettings();
        RunMigrations();
        Application = new VhApiWebApplicationFactory();
    }

    private void RegisterSettings()
    {
        _configRoot = ConfigRootBuilder.Build();
    }

    private void RunMigrations()
    {
        _databaseConnectionString = _configRoot.GetConnectionString("VhBookings");
        var dbContextOptionsBuilder = new DbContextOptionsBuilder<BookingsDbContext>();
        TestContext.WriteLine($"Connection string: {_databaseConnectionString}");
        dbContextOptionsBuilder.UseSqlServer(_databaseConnectionString);
        BookingsDbContextOptions = dbContextOptionsBuilder.Options;
        Hooks = new TestDataManager(BookingsDbContextOptions, "Bookings Api Integration Test");

        var context = new BookingsDbContext(BookingsDbContextOptions);
        context.Database.Migrate();
    }
}