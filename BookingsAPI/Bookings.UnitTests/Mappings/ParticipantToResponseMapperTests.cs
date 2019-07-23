using Bookings.Api.Contract.Responses;
using Bookings.API.Mappings;
using Bookings.Domain;
using Bookings.Domain.Participants;
using Bookings.Domain.RefData;
using FluentAssertions;
using NUnit.Framework;
using Testing.Common.Builders.Domain;

namespace Bookings.UnitTests.Mappings
{
    public class ParticipantToResponseMapperTests
    {
        private readonly ParticipantToResponseMapper _mapper = new ParticipantToResponseMapper();
        
        [Test]
        public void should_map_judge()
        {
            var caseRole = new CaseRole(5, "Judge");
            var hearingRole = new HearingRole(13, "Judge") {UserRole = new UserRole(4, "Judge")};

            var person = new PersonBuilder().Build();
            var judge = new Judge(person, hearingRole, caseRole)
            {
                DisplayName = "Judge",
                CreatedBy = "unit@test.com"
            };
            judge.SetProtected(nameof(judge.CaseRole), caseRole);
            judge.SetProtected(nameof(judge.HearingRole), hearingRole);

            var response = _mapper.MapParticipantToResponse(judge);
            
            AssertParticipantCommonDetails(response, judge, caseRole, hearingRole);
            AssertRepresentativeResponse(response, null);
            AssertAddressMapping(response, null);
            response.Organisation.Should().BeNullOrWhiteSpace();
            
        }

        [Test]
        public void should_map_individual()
        {
            var caseRole = new CaseRole(1, "Claimant");
            var hearingRole = new HearingRole(1, "Claimant LIP") {UserRole = new UserRole(5, "Individual")};

            var person = new PersonBuilder().WithAddress().WithOrganisation().Build();
            var individual = new Individual(person, hearingRole, caseRole)
            {
                DisplayName = "I. Vidual",
                CreatedBy = "unit@test.com"
            };
            individual.SetProtected(nameof(individual.CaseRole), caseRole);
            individual.SetProtected(nameof(individual.HearingRole), hearingRole);

            var response = _mapper.MapParticipantToResponse(individual);

            AssertParticipantCommonDetails(response, individual, caseRole, hearingRole);
            AssertRepresentativeResponse(response, null);
            AssertAddressMapping(response, person);
            response.Organisation.Should().Be(person.Organisation.Name);
        }
        
        [Test]
        public void should_map_representative()
        {
            var caseRole = new CaseRole(1, "Claimant");
            var hearingRole = new HearingRole(2, "Solicitor") {UserRole = new UserRole(6, "Representative")};

            var person = new PersonBuilder().WithOrganisation().Build();
            var representative = new Representative(person, hearingRole, caseRole)
            {
                SolicitorsReference = "HUHIUHFIH",
                Representee = "Mr A. Daijif",
                DisplayName = "I. Vidual",
                CreatedBy = "unit@test.com"
            };
            representative.SetProtected(nameof(representative.CaseRole), caseRole);
            representative.SetProtected(nameof(representative.HearingRole), hearingRole);

            var response = _mapper.MapParticipantToResponse(representative);

            AssertParticipantCommonDetails(response, representative, caseRole, hearingRole);
            AssertRepresentativeResponse(response, representative);
            AssertAddressMapping(response, null);
            response.Organisation.Should().Be(person.Organisation.Name);
        }

        private static void AssertParticipantCommonDetails(ParticipantResponse response, Participant participant,
            CaseRole caseRole, HearingRole hearingRole)
        {
            response.Id.Should().Be(participant.Id);
            response.DisplayName.Should().Be(participant.DisplayName);
            response.CaseRoleName.Should().Be(caseRole.Name);
            response.HearingRoleName.Should().Be(hearingRole.Name);
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

        private static void AssertAddressMapping(ParticipantResponse response, Person person)
        {
            if (person == null)
            {
                response.HouseNumber.Should().BeNullOrWhiteSpace();
                response.Street.Should().BeNullOrWhiteSpace();
                response.Postcode.Should().BeNullOrWhiteSpace();
                response.City.Should().BeNullOrWhiteSpace();
                response.County.Should().BeNullOrWhiteSpace();
            }
            else
            {
                response.HouseNumber.Should().Be(person.Address.HouseNumber);
                response.Street.Should().Be(person.Address.Street);
                response.Postcode.Should().Be(person.Address.Postcode);
                response.City.Should().Be(person.Address.City);
                response.County.Should().Be(person.Address.County);
                
            }
        }

        private static void AssertRepresentativeResponse(ParticipantResponse response, Representative representative)
        {
            if (representative == null)
            {
                response.SolicitorReference.Should().BeNullOrWhiteSpace();
                response.Representee.Should().BeNullOrWhiteSpace();
            }
            else
            {
                response.SolicitorReference.Should().Be(representative.SolicitorsReference);
                response.Representee.Should().Be(representative.Representee);
            }
        }
    }
}