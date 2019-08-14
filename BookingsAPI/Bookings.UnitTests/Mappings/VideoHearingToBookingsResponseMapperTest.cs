using System;
using Bookings.API.Mappings;
using Bookings.Domain;
using Bookings.Domain.RefData;
using FluentAssertions;
using NUnit.Framework;
using Bookings.UnitTests.Utilities;
using Testing.Common.Builders.Domain;
using System.Collections.Generic;
using System.Linq;

namespace Bookings.UnitTests.Mappings
{
    public class VideoHearingToBookingsResponseMapperTest : TestBase
    {
        private readonly VideoHearingsToBookingsResponseMapper _mapper = new VideoHearingsToBookingsResponseMapper();
        
        [Test]
        public void should_return_mapped_hearings_grouped_by_date()
        {
            var hearings = new[]
            {
                MockHearingAtDate(DateTime.Now.AddDays(1), true),
                MockHearingAtDate(DateTime.Now.AddDays(2), false),
                MockHearingAtDate(DateTime.Now.AddDays(3), false)
            };
            var mappedHearings = _mapper.MapHearingResponses(hearings);
            mappedHearings.Count.Should().Be(3);
            
            var firstGroup = mappedHearings[0]; 
            firstGroup.ScheduledDate.Should().Be(hearings[0].ScheduledDateTime.Date);
            firstGroup.Hearings.Count.Should().Be(1);
            firstGroup.Hearings.First().QuestionnaireNotRequired.Should().Be(true);
        }

        private VideoHearing MockHearingAtDate(DateTime datetime, bool questionnaireNotRequired)
        {
            var mockedHearing = MockHearingWithCase();
            mockedHearing.CaseType = new CaseType(1, "Civil Money Claims");
            var caseToUpdate = new Case("UpdateCaseNumber", "UpdateCasename");
            var updatedCases = new List<Case>();

            updatedCases.Add(caseToUpdate);
            mockedHearing.UpdateHearingDetails(
                mockedHearing.HearingVenue, 
                datetime, 
                mockedHearing.ScheduledDuration, 
                mockedHearing.HearingRoomName,
                mockedHearing.OtherInformation,
                "admin@hearings.reform.hmcts.net",
                updatedCases,
                questionnaireNotRequired
            );
            return mockedHearing;
        }
        
        [Test]
        public void should_map_properties_of_hearing()
        {
            var mockedHearing = MockHearingWithCase();
            mockedHearing.CaseType = new CaseType(1, "Civil Money Claims");
            mockedHearing.GetParticipants()[2].HearingRole = new HearingRole(5, "Judge") { UserRole = new UserRole(5, "Judge") };

            var mapped = _mapper.MapHearingResponse(mockedHearing);
            mapped.Should().NotBeNull();
            mapped.ScheduledDuration.Should().Be(80);
            mapped.JudgeName.Should().NotBeNullOrEmpty();
            mapped.HearingNumber.Should().Be("234");
            mapped.HearingName.Should().Be("X vs Y");
            mapped.CaseTypeName.Should().Be("Civil Money Claims");
            mapped.CourtAddress.Should().Be("Birmingham Civil and Family Justice Centre");
            mapped.CourtRoom.Should().Be("Roome03");
        }

        [Test]
        public void should_throw_exception_if_hearing_is_missing_case()
        {
            var mockedHearing = new VideoHearingBuilder().Build();

            When(() => _mapper.MapHearingResponse(mockedHearing))
                .Should().Throw<ArgumentException>().WithMessage("Hearing is missing case");
        }

        /// <summary>Test that all the data we require is being validated</summary>
        /// <remarks>If we don't do this, in case some piece of code isn't loading navigation properties we may hide bugs</remarks>
        [Test]
        public void should_throw_exception_if_required_navigation_properties_are_empty()
        {
            var hearingWithoutCaseType = MockHearingWithCase();
            hearingWithoutCaseType.SetProtected(nameof(hearingWithoutCaseType.CaseType), null);
            When(() => _mapper.MapHearingResponse(hearingWithoutCaseType))
                .Should().Throw<ArgumentException>().WithMessage("Hearing is missing case type");
            
            var hearingWithoutHearingType = MockHearingWithCase();
            hearingWithoutHearingType.SetProtected(nameof(hearingWithoutCaseType.HearingType), null);
            When(() => _mapper.MapHearingResponse(hearingWithoutHearingType))
                .Should().Throw<ArgumentException>().WithMessage("Hearing is missing hearing type");
        }

        [Test]
        public void should_set_judge_name_to_missing_if_empty()
        {
            var mockedHearing = MockHearingWithCase();
            mockedHearing.CaseType = new CaseType(1, "Civil Money Claims");

            var target = new VideoHearingsToBookingsResponseMapper();
            var mapped = target.MapHearingResponse(mockedHearing);
            mapped.JudgeName.Should().BeEmpty();
        }

        private VideoHearing MockHearingWithCase()
        {
            var hearing = new VideoHearingBuilder().Build();
            hearing.AddCase("234", "X vs Y", true);
            return hearing;
        }
    }
}