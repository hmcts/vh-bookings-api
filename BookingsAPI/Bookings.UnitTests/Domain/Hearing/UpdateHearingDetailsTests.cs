using System;
using System.Collections.Generic;
using System.Linq;
using Bookings.Domain;
using Bookings.Domain.Validations;
using FluentAssertions;
using NUnit.Framework;
using Testing.Common.Builders.Domain;

namespace Bookings.UnitTests.Domain.Hearing
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

            var casesToUpdate = new List<Case>
            {
                new Case(caseNumber, caseName)
            };

            hearing.UpdateHearingDetails(newVenue, newDateTime, newDuration,
                hearingRoomName, otherInformation, updatedBy, casesToUpdate, questionnaireNotRequired);

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

            Action action = () => hearing.UpdateHearingDetails(newVenue, newDateTime, newDuration,
                string.Empty, string.Empty, updatedBy, cases, questionnaireNotRequired);
            action.Should().Throw<DomainRuleException>()
                .And.ValidationFailures.Should()
                .Contain(x => x.Name == "ScheduledDuration")
                .And.Contain(x => x.Name == "ScheduledDateTime")
                .And.Contain(x => x.Name == "Venue");

            hearing.UpdatedDate.Should().Be(beforeUpdatedDate);
        }
    }
}