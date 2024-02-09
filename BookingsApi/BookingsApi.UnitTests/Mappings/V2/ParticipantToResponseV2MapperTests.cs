using System.Collections.Generic;
using BookingsApi.Contract.V1.Responses;
using BookingsApi.Contract.V2.Responses;
using BookingsApi.DAL.Helper;
using BookingsApi.Domain;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.Participants;
using BookingsApi.Domain.RefData;
using BookingsApi.Mappings.V2;

namespace BookingsApi.UnitTests.Mappings.V2
{
    public class ParticipantToResponseV2MapperTests
    {
        private readonly ParticipantToResponseV2Mapper _mapper = new ParticipantToResponseV2Mapper();

        
        [Test]
        public void Should_map_participant_judge()
        {
            var caseRole = new CaseRole(5, "Judge");
            var hearingRole = new HearingRole(13, "Judge") { UserRole = new UserRole(4, "Judge")};

            var person = new PersonBuilder().Build();
            var judge = new Judge(person, hearingRole, caseRole)
            {
                DisplayName = "Judge",
                CreatedBy = "unit@hmcts.net"
            };
            judge.SetProtected(nameof(judge.CaseRole), caseRole);
            judge.SetProtected(nameof(judge.HearingRole), hearingRole);

            var response = _mapper.MapParticipantToResponse(judge);
            
            AssertParticipantCommonDetails(response, judge, caseRole, hearingRole);
            response.Organisation.Should().BeNullOrWhiteSpace();
        }

        

        private static void AssertParticipantCommonDetails(ParticipantResponseV2 response, Participant participant,
            CaseRole caseRole, HearingRole hearingRole)
        {
            response.Id.Should().Be(participant.Id);
            response.DisplayName.Should().Be(participant.DisplayName);
            response.HearingRoleName.Should().Be(hearingRole.Name);
            response.HearingRoleCode.Should().Be(hearingRole.Code);
            response.UserRoleName.Should().Be(hearingRole.UserRole.Name);

            var person = participant.Person;

            response.Title.Should().Be(person.Title);
            response.FirstName.Should().Be(person.FirstName);
            response.MiddleNames.Should().Be(person.MiddleNames);
            response.LastName.Should().Be(person.LastName);
            
            response.ContactEmail.Should().Be(person.ContactEmail);
            response.TelephoneNumber.Should().Be(person.TelephoneNumber);
            response.Username.Should().Be(person.Username);
        }
    }
}