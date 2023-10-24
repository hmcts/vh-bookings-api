using System.Collections.Generic;
using BookingsApi.Domain;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.Validations;

namespace BookingsApi.UnitTests.Domain.Hearing
{
    public class UpdateHearingDetailsTests
    {
        [Test]
        public void Should_update_hearing_details()
        {
            var hearing = new VideoHearingBuilder().Build();
            hearing.AddCase("0875", "Test Case Add", false);
            var beforeUpdatedDate = hearing.UpdatedDate;
            var newVenue = new RefDataBuilder().HearingVenues[^1];
            var newDateTime = DateTime.Today.AddDays(10).AddHours(14);
            var newDuration = 150;
            var hearingRoomName = "Room03 Edit";
            var otherInformation = "OtherInformation03 Edit";
            var updatedBy = "testuser";
            var caseName = "CaseName Update";
            var caseNumber = "CaseNumber Update";
            const bool audioRecordingRequired = true;

            var casesToUpdate = new List<Case>
            {
                new Case(caseNumber, caseName)
            };

            hearing.UpdateHearingDetails(newVenue, newDateTime, newDuration,
                hearingRoomName, otherInformation, updatedBy, casesToUpdate, audioRecordingRequired);

            hearing.UpdatedDate.Should().BeAfter(beforeUpdatedDate);
            var updatedCases = hearing.GetCases();
            updatedCases[0].Name.Should().Be(caseName);
            updatedCases[0].Number.Should().Be(caseNumber);
        }

        [Test]
        public void Should_throw_exception_when_validation_fails()
        {
            var hearing = new VideoHearingBuilder().Build();
            hearing.AddCase("0875", "Test Case Add", false);
            var beforeUpdatedDate = hearing.UpdatedDate;
            HearingVenue newVenue = null;
            var newDateTime = DateTime.Today.AddDays(-10);
            var newDuration = -10;
            var updatedBy = "testuser";
            var cases = new List<Case>();
            const bool audioRecordingRequired = true;

            Action action = () => hearing.UpdateHearingDetails(newVenue, newDateTime, newDuration,
                string.Empty, string.Empty, updatedBy, cases, audioRecordingRequired);
            action.Should().Throw<DomainRuleException>()
                .And.ValidationFailures.Should()
                .Contain(x => x.Name == "ScheduledDuration")
                .And.Contain(x => x.Name == "ScheduledDateTime")
                .And.Contain(x => x.Name == "Venue");

            hearing.UpdatedDate.Should().Be(beforeUpdatedDate);
        }

        [Test]
        public void Should_skip_update_if_no_change_requested()
        {
            // arrange
            var dateTime = DateTime.UtcNow.AddMinutes(25);
            var hearing = new VideoHearingBuilder(dateTime).Build();
            hearing.UpdateStatus(BookingStatus.Created, "test@test.com", null);
            
            // act
           var action = () =>  hearing.UpdateHearingDetails(hearing.HearingVenue, hearing.ScheduledDateTime, hearing.ScheduledDuration,
                hearing.HearingRoomName, hearing.OtherInformation, hearing.UpdatedBy, hearing.GetCases().ToList(),
                hearing.AudioRecordingRequired);

           action.Should().NotThrow<DomainRuleException>();
        }
    }
}