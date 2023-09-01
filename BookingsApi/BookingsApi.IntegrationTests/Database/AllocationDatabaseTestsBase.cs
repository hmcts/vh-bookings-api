using BookingsApi.Common.Services;
using BookingsApi.DAL.Services;
using BookingsApi.Domain.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace BookingsApi.IntegrationTests.Database
{
    public abstract class AllocationDatabaseTestsBase : DatabaseTestsBase
    {
        protected IHearingAllocationService HearingAllocationService;
        protected BookingsDbContext Context;

        [SetUp]
        protected void BaseSetup()
        {
            Context = new BookingsDbContext(BookingsDbContextOptions);
            var randomNumberGenerator = new RandomNumberGenerator();
            var allocationConfiguration = GetDefaultAllocationSettings();
            HearingAllocationService = new HearingAllocationService(Context, 
                randomNumberGenerator, 
                new OptionsWrapper<AllocateHearingConfiguration>(allocationConfiguration),
                new NullLogger<HearingAllocationService>());
        }
        
        private static AllocateHearingConfiguration GetDefaultAllocationSettings()
        {
            return new AllocateHearingConfiguration
            {
                AllowHearingToStartBeforeWorkStartTime = false,
                AllowHearingToEndAfterWorkEndTime = false,
                MinimumGapBetweenHearingsInMinutes = 30,
                MaximumConcurrentHearings = 3
            };
        }
    }
}
