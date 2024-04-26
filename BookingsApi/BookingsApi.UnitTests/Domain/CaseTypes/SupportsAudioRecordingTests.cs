using BookingsApi.Domain.RefData;

namespace BookingsApi.UnitTests.Domain.CaseTypes;

public class SupportsAudioRecordingTests
{
    [TestCase(CaseType.CacdServiceId, false)]
    [TestCase(CaseType.CccServiceId, false)]
    [TestCase("VIHTMP9", true)]
    public void Should_return_true_when_service_id_is_not_CCC_or_CACD(string serviceId, bool expected)
    {
        var caseType = new CaseType(1, "Case1")
        {
            ServiceId = serviceId
        };

        Assert.AreEqual(expected, caseType.SupportsAudioRecording());
    }
}