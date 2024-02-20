using BookingsApi.UnitTests.Utilities;

namespace BookingsApi.UnitTests.Domain.Hearing
{
    public class HearingTests : TestBase
    {
        protected static void AssertHearingUpdatedAuditDetailsAreUpdated(BookingsApi.Domain.Hearing hearing, 
            DateTime beforeUpdatedDate, 
            string updatedBy)
        {
            hearing.UpdatedDate.Should().BeAfter(beforeUpdatedDate);
            hearing.UpdatedBy.Should().Be(string.IsNullOrEmpty(updatedBy) ? "System" : updatedBy);
        }
    }
}
