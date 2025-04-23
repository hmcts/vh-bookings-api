﻿using System.Collections.Generic;
using BookingsApi.Domain;
using BookingsApi.Domain.Participants;
using BookingsApi.Domain.RefData;
using BookingsApi.Mappings.V1;
using BookingsApi.UnitTests.Utilities;

namespace BookingsApi.UnitTests.Mappings.V1
{
    public class AudioRecordedHearingsBySearchResponseMapperTests : TestBase
    {
        private List<VideoHearing> hearingsByCaseNumber;
        private AudioRecordedHearingsBySearchResponseMapper hearingMapper;

        [SetUp]
        public void Init()
        {
            hearingsByCaseNumber = new List<VideoHearing>() { GetHearing(true) };
            hearingMapper = new AudioRecordedHearingsBySearchResponseMapper();
        }


        [TestCase("Test 001")]
        [TestCase(" Test 001")]
        [TestCase("Test 001 ")]
        [TestCase("")]
        [TestCase(null)]
        public void Should_map_all_properties(string caseNumber)
        {
            var @case = string.IsNullOrEmpty(caseNumber)
                ? hearingsByCaseNumber[0].GetCases()[0]
                : hearingsByCaseNumber[0].GetCases()
                    .FirstOrDefault(c => c.Number.ToLower().Trim() == caseNumber.ToLower().Trim());

            var result = hearingMapper.MapHearingToDetailedResponse(hearingsByCaseNumber, caseNumber);

            var judgeParticipant = hearingsByCaseNumber[0].GetJudge();
            var courtroomAccountName = judgeParticipant != null ? judgeParticipant.DisplayName : string.Empty;
            var courtroomAccount = judgeParticipant?.JudiciaryPerson != null ? judgeParticipant.JudiciaryPerson.Email : string.Empty;
            result[0].CaseName.Should().Be(@case.Name);
            result[0].CaseNumber.Should().Be(@case.Number);
            result[0].Id.Should().Be(hearingsByCaseNumber[0].Id);
            result[0].ScheduledDateTime.Should().Be(hearingsByCaseNumber[0].ScheduledDateTime);
            result[0].HearingVenueName.Should().Be(hearingsByCaseNumber[0].HearingVenue.Name);
            result[0].CourtroomAccount.Should().Be(courtroomAccount);
            result[0].CourtroomAccountName.Should().Be(courtroomAccountName);
            result[0].HearingRoomName.Should().Be(hearingsByCaseNumber[0].HearingRoomName);
        }

        [TestCase]
        public void Should_raise_exception_without_matching_casenumber()
        {  

            Action act = () => hearingMapper.MapHearingToDetailedResponse(hearingsByCaseNumber, "NotExists");

            act.Should().Throw<ArgumentException>().WithMessage("Hearing is missing case");
        }

        [TestCase]
        public void Should_raise_exception_without_casenumber()
        {
            hearingsByCaseNumber = new List<VideoHearing>() { GetHearing(false) };
 
            Action act = () => hearingMapper.MapHearingToDetailedResponse(hearingsByCaseNumber, string.Empty);

            act.Should().Throw<ArgumentException>().WithMessage("Hearing is missing case");
        }

        [TestCase]
        public void Should_map_all_without_judge()
        {
            var @case = hearingsByCaseNumber[0].GetCases().FirstOrDefault();
            hearingsByCaseNumber.ForEach(h =>
            {
                h.RemoveJudiciaryParticipantByPersonalCode(h.GetJudge().JudiciaryPerson.PersonalCode);
            });

            var result = hearingMapper.MapHearingToDetailedResponse(hearingsByCaseNumber, string.Empty);
             
            result[0].CaseName.Should().Be(@case.Name);
            result[0].CaseNumber.Should().Be(@case.Number);
            result[0].Id.Should().Be(hearingsByCaseNumber[0].Id);
            result[0].ScheduledDateTime.Should().Be(hearingsByCaseNumber[0].ScheduledDateTime);
            result[0].HearingVenueName.Should().Be(hearingsByCaseNumber[0].HearingVenue.Name);
            result[0].CourtroomAccount.Should().Be(string.Empty);
            result[0].CourtroomAccountName.Should().Be(string.Empty);
            result[0].HearingRoomName.Should().Be(hearingsByCaseNumber[0].HearingRoomName);
        }

        private static VideoHearing GetHearing(bool addCase)
        {
            var hearing = new VideoHearingBuilder(addJudge:true).Build();

            if(addCase)
            {
                hearing.AddCase("Test 001 ", "Case name", true);
            }
            return hearing;
        }
    }
}
