using BookingsApi.Client;
using Microsoft.Extensions.Configuration;
using Testing.Common.Configuration;
using Testing.Common.Stubs;

namespace BookingsApi.IntegrationTests;

public class ApiTest
{
    protected VhApiWebApplicationFactory Application = null!;
    protected DbContextOptions<BookingsDbContext> BookingsDbContextOptions { get; set; }
    protected TestDataManager Hooks { get; private set; }
    private IConfigurationRoot _configRoot;
    private string _databaseConnectionString;
    protected Person GenericJudge { get; private set; }

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        RegisterSettings();
        RunMigrations();
        GenericJudge = Hooks.SeedGenericJudgePerson().Result;
        Application = new VhApiWebApplicationFactory();
    }

    [TearDown]
    public async Task TearDown()
    {
        await Hooks.ClearSeededHearings();
        await Hooks.ClearSeededJusticeUsersAsync();
        await Hooks.ClearJudiciaryPersonsAsync();
    }

    private void RegisterSettings()
    {
        _configRoot = ConfigRootBuilder.Build();
    }

    private void RunMigrations()
    {
        _databaseConnectionString = _configRoot.GetConnectionString("VhBookings");
        var dbContextOptionsBuilder = new DbContextOptionsBuilder<BookingsDbContext>();
        dbContextOptionsBuilder.UseSqlServer(_databaseConnectionString);
        BookingsDbContextOptions = dbContextOptionsBuilder.Options;
        Hooks = new TestDataManager(BookingsDbContextOptions, "Bookings Api Integration Test",
            new FeatureTogglesStub().UseVodafoneToggle());

        var context = new BookingsDbContext(BookingsDbContextOptions);
        context.Database.Migrate();
    }
}