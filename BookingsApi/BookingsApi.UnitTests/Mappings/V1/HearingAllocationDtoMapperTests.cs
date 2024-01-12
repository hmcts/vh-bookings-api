using BookingsApi.Domain;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.Participants;
using BookingsApi.Infrastructure.Services;

namespace BookingsApi.UnitTests.Mappings.V1
{
    public class HearingAllocationDtoMapperTests
    {
        [Test]
        public void Should_Map_With_Judge()
        {
            var hearing = GetHearing();
            
            var result = HearingAllocationDtoMapper.MapToDto(hearing);

            var expectedCase = hearing.GetCases()[0];
            var expectedJudge = hearing.Participants?.FirstOrDefault(p => p is Judge);
            
            result.HearingId.Should().Be(hearing.Id);
            result.GroupId.Should().Be(hearing.SourceId);
            result.ScheduledDateTime.Should().Be(hearing.ScheduledDateTime);
            result.ScheduledDuration.Should().Be(hearing.ScheduledDuration);
            result.CaseType.Should().Be(hearing.CaseType.Name);
            result.CaseNumber.Should().Be(expectedCase.Number);
            result.CaseName.Should().Be(expectedCase.Name);
            result.HearingVenueName.Should().Be(hearing.HearingVenue.Name);
            result.RecordAudio.Should().Be(hearing.AudioRecordingRequired);
            result.JudgeDisplayName.Should().Be(expectedJudge?.DisplayName);
        }

        [Test]
        public void Should_Map_With_Judiciary_Judge()
        {
            var hearing = GetHearingWithJudiciaryParticipants();
            
            var result = HearingAllocationDtoMapper.MapToDto(hearing);

            var expectedCase = hearing.GetCases()[0];
            var expectedJudge = hearing.JudiciaryParticipants.FirstOrDefault(p => p.HearingRoleCode == JudiciaryParticipantHearingRoleCode.Judge);
            
            result.HearingId.Should().Be(hearing.Id);
            result.GroupId.Should().Be(hearing.SourceId);
            result.ScheduledDateTime.Should().Be(hearing.ScheduledDateTime);
            result.ScheduledDuration.Should().Be(hearing.ScheduledDuration);
            result.CaseType.Should().Be(hearing.CaseType.Name);
            result.CaseNumber.Should().Be(expectedCase.Number);
            result.CaseName.Should().Be(expectedCase.Name);
            result.HearingVenueName.Should().Be(hearing.HearingVenue.Name);
            result.RecordAudio.Should().Be(hearing.AudioRecordingRequired);
            result.JudgeDisplayName.Should().Be(expectedJudge?.DisplayName);
        }
        
        private static VideoHearing GetHearing()
        {
            var hearing = new VideoHearingBuilder()
                .WithCase()
                .Build();

            return hearing;
        }

        private static VideoHearing GetHearingWithJudiciaryParticipants()
        {
            var hearing = new VideoHearingBuilder(addJudge: false)
                .WithCase()
                .WithJudiciaryJudge()
                .WithJudiciaryPanelMember()
                .Build();
            
            return hearing;
        }
    }
}
