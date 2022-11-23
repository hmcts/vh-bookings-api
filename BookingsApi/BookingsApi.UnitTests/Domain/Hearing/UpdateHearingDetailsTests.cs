using System;
using System.Collections.Generic;
using System.Linq;
using BookingsApi.Domain;
using BookingsApi.Domain.Validations;
using FluentAssertions;
using NUnit.Framework;
using Testing.Common.Builders.Domain;

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
            var newVenue = new RefDataBuilder().HearingVenues.Last();
            var newDateTime = DateTime.Today.AddDays(10).AddHours(14);
            var newDuration = 150;
            var hearingRoomName = "Room03 Edit";
            var otherInformation = "OtherInformation03 Edit";
            var updatedBy = "testuser";
            var caseName = "CaseName Update";
            var caseNumber = "CaseNumber Update";
            const bool questionnaireNotRequired = false;
            const bool audioRecordingRequired = true;

            var casesToUpdate = new List<Case>
            {
                new Case(caseNumber, caseName)
            };

            hearing.UpdateHearingDetails(newVenue, newDateTime, newDuration,
                hearingRoomName, otherInformation, updatedBy, casesToUpdate, questionnaireNotRequired, audioRecordingRequired);

            hearing.UpdatedDate.Should().BeAfter(beforeUpdatedDate);
            var updatedCases = hearing.GetCases();
            updatedCases.First().Name.Should().Be(caseName);
            updatedCases.First().Number.Should().Be(caseNumber);
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
            const bool questionnaireNotRequired = false;
            const bool audioRecordingRequired = true;

            Action action = () => hearing.UpdateHearingDetails(newVenue, newDateTime, newDuration,
                string.Empty, string.Empty, updatedBy, cases, questionnaireNotRequired, audioRecordingRequired);
            action.Should().Throw<DomainRuleException>()
                .And.ValidationFailures.Should()
                .Contain(x => x.Name == "ScheduledDuration")
                .And.Contain(x => x.Name == "ScheduledDateTime")
                .And.Contain(x => x.Name == "Venue");

            hearing.UpdatedDate.Should().Be(beforeUpdatedDate);
        }

        [Test]
        public void Should_deallocate_when_scheduled_datetime_changes()
        {
            var hearing = new VideoHearingBuilder().Build();
            var allocatedUser = new JusticeUser
            {
                Username = "user@email.com"
            };
            hearing.SetProtected(nameof(hearing.Allocations), new List<Allocation>
            {
                new()
                {
                    Hearing = hearing,
                    JusticeUser = allocatedUser
                }
            });
            hearing.AllocatedTo.Should().NotBeNull();
            hearing.AllocatedTo.Username.Should().Be(allocatedUser.Username);
            var newDateTime = DateTime.Today.AddDays(1);
            var updatedBy = "testuser";
            
            hearing.UpdateHearingDetails(hearing.HearingVenue, 
                newDateTime, 
                hearing.ScheduledDuration, 
                hearing.HearingRoomName,
                hearing.OtherInformation, 
                updatedBy, 
                new List<Case>(), 
                hearing.QuestionnaireNotRequired, 
                hearing.AudioRecordingRequired);

            hearing.AllocatedTo.Should().BeNull();
        }

        [Test]
        public void Should_not_deallocate_when_scheduled_datetime_has_not_changed()
        {
            var hearing = new VideoHearingBuilder().Build();
            var allocatedUser = new JusticeUser
            {
                Username = "user@email.com"
            };
            hearing.SetProtected(nameof(hearing.Allocations), new List<Allocation>
            {
                new()
                {
                    Hearing = hearing,
                    JusticeUser = allocatedUser
                }
            });
            hearing.AllocatedTo.Should().NotBeNull();
            hearing.AllocatedTo.Username.Should().Be(allocatedUser.Username);
            var updatedBy = "testuser";
            
            hearing.UpdateHearingDetails(hearing.HearingVenue, 
                hearing.ScheduledDateTime, 
                hearing.ScheduledDuration, 
                hearing.HearingRoomName,
                hearing.OtherInformation, 
                updatedBy, 
                new List<Case>(), 
                hearing.QuestionnaireNotRequired, 
                hearing.AudioRecordingRequired);

            hearing.AllocatedTo.Should().NotBeNull();
            hearing.AllocatedTo.Username.Should().Be(allocatedUser.Username);
        }
    }
}