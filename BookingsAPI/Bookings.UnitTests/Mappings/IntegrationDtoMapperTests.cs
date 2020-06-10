using System.Linq;
using Bookings.Domain;
using Bookings.Domain.Participants;
using Bookings.Domain.RefData;
using Bookings.Infrastructure.Services;
using FluentAssertions;
using NUnit.Framework;
using Testing.Common.Builders.Domain;

namespace Bookings.UnitTests.Mappings
{
    public class IntegrationDtoMapperTests
    {
        [Test]
        public void should_map_hearing_to_dto()
        {
            var hearing = GetHearing();
            var result = HearingDtoMapper.MapToDto(hearing);

            var @case = hearing.GetCases().First();
            result.CaseName.Should().Be(@case.Name);
            result.CaseNumber.Should().Be(@case.Number);
            result.CaseType.Should().Be(hearing.CaseType.Name);
            result.HearingId.Should().Be(hearing.Id);
            result.RecordAudio.Should().Be(hearing.AudioRecordingRequired);
            result.ScheduledDuration.Should().Be(hearing.ScheduledDuration);
            result.ScheduledDateTime.Should().Be(hearing.ScheduledDateTime);
            result.HearingVenueName.Should().Be(hearing.HearingVenueName);
        }
        
        [Test]
        public void should_map_individual_participant_to_dto()
        {
            var participant = (Individual) GetHearing().GetParticipants().First(x => x is Individual);
            var result = ParticipantDtoMapper.MapToDto(participant);

            var fullname = $"{participant.Person.Title} {participant.Person.FirstName} {participant.Person.LastName}";
            result.ParticipantId.Should().Be(participant.Id);
            result.Fullname.Should().Be(fullname);
            result.Username.Should().Be(participant.Person.Username);
            result.FirstName.Should().Be(participant.Person.FirstName);
            result.LastName.Should().Be(participant.Person.LastName);
            result.DisplayName.Should().Be(participant.DisplayName);
            result.HearingRole.Should().Be(participant.HearingRole.Name);
            result.UserRole.Should().Be(participant.HearingRole.UserRole.Name);
            result.CaseGroupType.Should().Be(participant.CaseRole.Group);

            result.Representee.Should().BeNullOrEmpty();
        }
        
        [Test]
        public void should_map_representative_participant_to_dto()
        {
            var participant = (Representative) GetHearing().GetParticipants().First(x => x is Representative);
            var result = ParticipantDtoMapper.MapToDto(participant);

            var fullname = $"{participant.Person.Title} {participant.Person.FirstName} {participant.Person.LastName}";
            result.ParticipantId.Should().Be(participant.Id);
            result.Fullname.Should().Be(fullname);
            result.Username.Should().Be(participant.Person.Username);
            result.FirstName.Should().Be(participant.Person.FirstName);
            result.LastName.Should().Be(participant.Person.LastName);
            result.DisplayName.Should().Be(participant.DisplayName);
            result.HearingRole.Should().Be(participant.HearingRole.Name);
            result.UserRole.Should().Be(participant.HearingRole.UserRole.Name);
            result.CaseGroupType.Should().Be(participant.CaseRole.Group);

            result.Representee.Should().Be(participant.Representee);
        }

        private static VideoHearing GetHearing()
        {
            var hearing = new VideoHearingBuilder().Build();
            hearing.AddCase("123", "Case name", true);
            foreach (var participant in hearing.Participants)
            {
                participant.HearingRole = new HearingRole(1, "Name") { UserRole = new UserRole(1, "User"), };
                participant.CaseRole = new CaseRole(1, "Name");
            }

            return hearing;
        }
    }
}