using BookingsApi.Mappings;
using BookingsApi.Domain;
using BookingsApi.Domain.RefData;
using Castle.Core.Internal;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using BookingsApi.UnitTests.Utilities;
using Microsoft.IdentityModel.Tokens;
using Testing.Common.Builders.Domain;

namespace BookingsApi.UnitTests.Mappings
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
            var @case = caseNumber.IsNullOrEmpty() ? hearingsByCaseNumber[0].GetCases().FirstOrDefault():
                                                        hearingsByCaseNumber[0].GetCases().FirstOrDefault(c => c.Number.ToLower().Trim() == caseNumber.ToLower().Trim());

            var result = hearingMapper.MapHearingToDetailedResponse(hearingsByCaseNumber, caseNumber);
           
            var judgeParticipant = hearingsByCaseNumber[0].GetParticipants().FirstOrDefault(s => s.HearingRole?.UserRole != null && s.HearingRole.UserRole.Name == "Judge");
            var courtroomAccountName = judgeParticipant != null ? judgeParticipant.DisplayName : string.Empty;
            var courtroomAccount = (judgeParticipant != null && judgeParticipant.Person != null) ? judgeParticipant.Person.Username : string.Empty;
            result[0].CaseName.Should().Be(@case.Name);
            result[0].CaseNumber.Should().Be(@case.Number);
            result[0].Id.Should().Be(hearingsByCaseNumber[0].Id);
            result[0].ScheduledDateTime.Should().Be(hearingsByCaseNumber[0].ScheduledDateTime);
            result[0].HearingVenueName.Should().Be(hearingsByCaseNumber[0].HearingVenueName);
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
            var newHearingRole = new HearingRole(1, "Name") { UserRole = new UserRole(1, "Winger"), };
            hearingsByCaseNumber[0].GetParticipants().FirstOrDefault(s => s.HearingRole?.UserRole?.Name == "Judge").HearingRole = newHearingRole;

            var result = hearingMapper.MapHearingToDetailedResponse(hearingsByCaseNumber, string.Empty);
             
            result[0].CaseName.Should().Be(@case.Name);
            result[0].CaseNumber.Should().Be(@case.Number);
            result[0].Id.Should().Be(hearingsByCaseNumber[0].Id);
            result[0].ScheduledDateTime.Should().Be(hearingsByCaseNumber[0].ScheduledDateTime);
            result[0].HearingVenueName.Should().Be(hearingsByCaseNumber[0].HearingVenueName);
            result[0].CourtroomAccount.Should().Be(string.Empty);
            result[0].CourtroomAccountName.Should().Be(string.Empty);
            result[0].HearingRoomName.Should().Be(hearingsByCaseNumber[0].HearingRoomName);
        }

        private static VideoHearing GetHearing(bool addCase)
        {
            var hearing = new VideoHearingBuilder().Build();

            if(addCase)
            {
                hearing.AddCase("Test 001 ", "Case name", true);
            }
            return hearing;
        }
    }
}
