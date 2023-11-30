using BookingsApi.Common.Services;
using BookingsApi.DAL.Helper;
using BookingsApi.Domain;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.Participants;
using Constants = BookingsApi.Contract.V1.Constants;

namespace BookingsApi.UnitTests.Mappings.V1
{
    public class CloneHearingToCommandMapperTests
    {
        private IRandomGenerator _randomGenerator;
        private readonly string _sipAddressStem = "@WhereAreYou.com";

        [SetUp]
        public void Setup()
        {
            _randomGenerator = new RandomGenerator();
        }
        
        [Test]
        public void should_map_hearing_to_command()
        {
            var totalDays = 5;
            var hearingDay = 2;
            const int duration = Constants.CloneHearings.DefaultScheduledDuration;
            var hearing = new VideoHearingBuilder().Build();
            hearing.AddEndpoint(new Endpoint("Endpoint1", $"{Guid.NewGuid():N}@hmcts.net", "1234", null));
            hearing.AddEndpoint(new Endpoint("Endpoint2", $"{Guid.NewGuid():N}@hmcts.net", "2345",
                hearing.GetParticipants().First(x => x.HearingRole.UserRole.IsRepresentative)));
            hearing.AddCase("HBS/1234","Case 1 Test", true);
       
            var individualsInHearing = hearing.Participants.Where(x => x.HearingRole.UserRole.IsIndividual).ToList();
            individualsInHearing[0].AddLink(individualsInHearing[1].Id, LinkedParticipantType.Interpreter);
            individualsInHearing[1].AddLink(individualsInHearing[0].Id, LinkedParticipantType.Interpreter);

            var newDate = hearing.ScheduledDateTime.AddDays(1);

            var command = CloneHearingToCommandMapper.CloneToCommand(hearing, newDate, _randomGenerator,
                _sipAddressStem, totalDays, hearingDay, duration);

            command.HearingRoomName.Should().Be(hearing.HearingRoomName);
            command.OtherInformation.Should().Be(hearing.OtherInformation);
            command.CreatedBy.Should().Be(hearing.CreatedBy);
            
            command.CaseType.Should().Be(hearing.CaseType);
            command.HearingType.Should().Be(hearing.HearingType);

            command.ScheduledDateTime.Should().Be(newDate);
            command.ScheduledDateTime.Hour.Should().Be(hearing.ScheduledDateTime.Hour);
            command.ScheduledDateTime.Minute.Should().Be(hearing.ScheduledDateTime.Minute);
            command.ScheduledDuration.Should().Be(duration);

            command.Venue.Should().Be(hearing.HearingVenue);

            command.Participants.Count.Should().Be(hearing.GetParticipants().Count);
            foreach (var newParticipant in command.Participants)
            {
                var existingPerson = hearing.GetPersons().SingleOrDefault(x => x.Username == newParticipant.Person.Username);
                existingPerson.Should().NotBeNull();
                var existingPat = hearing.Participants.Single(x => x.Person == existingPerson);
                newParticipant.DisplayName.Should().Be(existingPat.DisplayName);
                newParticipant.CaseRole.Should().Be(existingPat.CaseRole);
                newParticipant.HearingRole.Should().Be(existingPat.HearingRole);

                if (existingPat.GetType() != typeof(Representative)) continue;
                var rep = (Representative) existingPat; 
                newParticipant.Representee.Should().Be(rep.Representee);
            }
            
            command.Cases.Count.Should().Be(hearing.GetCases().Count);
            foreach (var @case in command.Cases)
            {
                hearing.GetCases().SingleOrDefault(x => x.Number == @case.Number).Should()
                    .NotBeNull();
                @case.Name.Should().Contain($"Day {hearingDay} of {totalDays}");
            }
            
            command.Endpoints.Count.Should().Be(hearing.GetEndpoints().Count);
            foreach (var ep in command.Endpoints)
            {
                hearing.GetEndpoints().SingleOrDefault(x =>
                    x.DisplayName == ep.DisplayName &&
                    x.DefenceAdvocate?.Person?.ContactEmail == ep.ContactEmail).Should().NotBeNull();
            }
            command.AudioRecordingRequired.Should().Be(hearing.AudioRecordingRequired);
        }
    }
}