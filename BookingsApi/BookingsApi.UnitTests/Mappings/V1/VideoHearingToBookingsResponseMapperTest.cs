using System;
using System.Collections.Generic;
using System.Linq;
using BookingsApi.Domain;
using BookingsApi.Domain.Helper;
using BookingsApi.Domain.RefData;
using BookingsApi.Mappings.V1;
using BookingsApi.UnitTests.Utilities;

namespace BookingsApi.UnitTests.Mappings.V1
{
    public class VideoHearingToBookingsResponseMapperTest : TestBase
    {
        private readonly VideoHearingsToBookingsResponseMapper _mapper = new VideoHearingsToBookingsResponseMapper();

        [Test]
        public void Should_return_mapped_hearings_grouped_by_date()
        {
            var hearings = new[]
            {
                MockHearingAtDate(DateTime.Now.AddDays(1), true, true),
                MockHearingAtDate(DateTime.Now.AddDays(2), true, true),
                MockHearingAtDate(DateTime.Now.AddDays(3), false, true)
            };
            var mappedHearings = _mapper.MapHearingResponses(hearings);
            mappedHearings.Count.Should().Be(3);

            var firstGroup = mappedHearings[0];
            firstGroup.ScheduledDate.Should().Be(hearings[0].ScheduledDateTime.Date);
            firstGroup.Hearings.Count.Should().Be(1);
            firstGroup.Hearings.First().QuestionnaireNotRequired.Should().BeFalse();
            firstGroup.Hearings.First().AudioRecordingRequired.Should().BeTrue();
            firstGroup.Hearings.First().CancelReason.Should().Be(hearings[0].CancelReason);
            firstGroup.Hearings.First().GroupId.Should().Be(hearings[0].Id);
        }

        private static VideoHearing MockHearingAtDate(DateTime datetime, bool audioRecordingRequired,
            bool isMultiDayFirstHearing = false)
        {
            var mockedHearing = MockHearingWithCase();
            mockedHearing.CaseType = new CaseType(1, "Generic");
            var caseToUpdate = new Case("UpdateCaseNumber", "UpdateCasename");
            var updatedCases = new List<Case>
            {
                caseToUpdate
            };

            mockedHearing.UpdateHearingDetails(
                mockedHearing.HearingVenue,
                datetime,
                mockedHearing.ScheduledDuration,
                mockedHearing.HearingRoomName,
                mockedHearing.OtherInformation,
                "admin@hmcts.net",
                updatedCases,
                false,
                audioRecordingRequired
            );
            mockedHearing.IsFirstDayOfMultiDayHearing = isMultiDayFirstHearing;
            return mockedHearing;
        }

        [Test]
        public void Should_map_properties_of_hearing()
        {
            var mockedHearing = MockHearingWithCase();
            mockedHearing.CaseType = new CaseType(1, "Generic");

            var mapped = _mapper.MapHearingResponse(mockedHearing);
            mapped.Should().NotBeNull();
            mapped.ScheduledDuration.Should().Be(80);
            mapped.JudgeName.Should().NotBeNullOrEmpty();
            mapped.HearingNumber.Should().Be("234");
            mapped.HearingName.Should().Be("X vs Y");
            mapped.CaseTypeName.Should().Be("Generic");
            mapped.CourtAddress.Should().Be("Birmingham Civil and Family Justice Centre");
            mapped.CourtRoom.Should().Be("Roome03");
            mapped.CourtRoomAccount.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void Should_map_properties_without_judge()
        {
            var mockedHearing = MockHearingWithCase();
            mockedHearing.CaseType = new CaseType(1, "Generic");
            mockedHearing.GetParticipants()[3].HearingRole = new HearingRole(5, "Winger") { UserRole = new UserRole(5, "Winger") };

            var mapped = _mapper.MapHearingResponse(mockedHearing);
            mapped.Should().NotBeNull();
            mapped.ScheduledDuration.Should().Be(80);
            mapped.JudgeName.Should().BeNullOrEmpty();
            mapped.HearingNumber.Should().Be("234");
            mapped.HearingName.Should().Be("X vs Y");
            mapped.CaseTypeName.Should().Be("Generic");
            mapped.CourtAddress.Should().Be("Birmingham Civil and Family Justice Centre");
            mapped.CourtRoom.Should().Be("Roome03");
            mapped.CourtRoomAccount.Should().BeNullOrEmpty();
        }

        [Test]
        public void Should_throw_exception_if_hearing_is_missing_case()
        {
            var mockedHearing = new VideoHearingBuilder().Build();

            When(() => _mapper.MapHearingResponse(mockedHearing))
                .Should().Throw<ArgumentException>().WithMessage("Hearing is missing case");
        }

        /// <summary>Test that all the data we require is being validated</summary>
        /// <remarks>If we don't do this, in case some piece of code isn't loading navigation properties we may hide bugs</remarks>
        [Test]
        public void Should_throw_exception_if_required_navigation_properties_are_empty()
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
        public void Should_set_judge_name_to_missing_if_empty()
        {
            var mockedHearing = MockHearingWithCase();
            var judge = mockedHearing.Participants.First(x => x.HearingRole.UserRole.IsJudge);
            judge.DisplayName = "";
            mockedHearing.CaseType = new CaseType(1, "Generic");

            var target = new VideoHearingsToBookingsResponseMapper();
            var mapped = target.MapHearingResponse(mockedHearing);
            mapped.JudgeName.Should().BeEmpty();
        }
        
        [Test]
        public void Should_set_allocatedVHO_to_Not_Allocated()
        {
            var mockedHearing = MockHearingWithCase();
            var judge = mockedHearing.Participants.First(x => x.HearingRole.UserRole.IsJudge);
            judge.DisplayName = "";
            mockedHearing.CaseType = new CaseType(1, "Generic");
            

            var target = new VideoHearingsToBookingsResponseMapper();
            var mapped = target.MapHearingResponse(mockedHearing);
            mapped.JudgeName.Should().BeEmpty();
            mapped.AllocatedTo.Should().Be(VideoHearingHelper.NotAllocated);
        }
        
        [Test]
        public void Should_set_allocatedVHO_to_Not_Required()
        {
            var mockedHearing = MockHearingWithCase();
            var judge = mockedHearing.Participants.First(x => x.HearingRole.UserRole.IsJudge);
            judge.DisplayName = "";
            mockedHearing.CaseType = new CaseType(3, "Generic");
            mockedHearing.CaseTypeId = 3;
            

            var target = new VideoHearingsToBookingsResponseMapper();
            var mapped = target.MapHearingResponse(mockedHearing);
            mapped.JudgeName.Should().BeEmpty();
            mapped.AllocatedTo.Should().Be(VideoHearingHelper.NotRequired);
        }

        private static VideoHearing MockHearingWithCase()
        {
            var hearing = new VideoHearingBuilder().Build();
            hearing.AddCase("234", "X vs Y", true);
            return hearing;
        }
    }
}