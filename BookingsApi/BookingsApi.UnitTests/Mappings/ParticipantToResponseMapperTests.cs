using BookingsApi.Contract.Responses;
using BookingsApi.Mappings;
using BookingsApi.Domain;
using BookingsApi.Domain.Participants;
using BookingsApi.Domain.RefData;
using FluentAssertions;
using NUnit.Framework;
using Testing.Common.Builders.Domain;

namespace BookingsApi.UnitTests.Mappings
{
    public class ParticipantToResponseMapperTests
    {
        private readonly ParticipantToResponseMapper _mapper = new ParticipantToResponseMapper();
        
        [Test]
        public void Should_map_judge()
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
            response.Organisation.Should().BeNullOrWhiteSpace();
            
        }

        [Test]
        public void Should_map_individual()
        {
            var caseRole = new CaseRole(1, "Claimant");
            var hearingRole = new HearingRole(1, "Litigant in person") {UserRole = new UserRole(5, "Individual")};

            var person = new PersonBuilder().WithOrganisation().Build();
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
            response.Organisation.Should().Be(person.Organisation.Name);
        }
        
        [Test]
        public void Should_map_representative()
        {
            var caseRole = new CaseRole(1, "Claimant");
            var hearingRole = new HearingRole(2, "Representative") {UserRole = new UserRole(6, "Representative")};

            var person = new PersonBuilder().WithOrganisation().Build();
            var representative = new Representative(person, hearingRole, caseRole)
            {
                Representee = "Mr A. Daijif",
                DisplayName = "I. Vidual",
                CreatedBy = "unit@test.com"
            };
            representative.SetProtected(nameof(representative.CaseRole), caseRole);
            representative.SetProtected(nameof(representative.HearingRole), hearingRole);

            var response = _mapper.MapParticipantToResponse(representative);

            AssertParticipantCommonDetails(response, representative, caseRole, hearingRole);
            AssertRepresentativeResponse(response, representative);
            response.Organisation.Should().Be(person.Organisation.Name);
        }
        
        [Test]
        public void Should_map_judicial_office_holder()
        {
            var caseRole = new CaseRole(7, "Judicial Office Holder");
            var hearingRole = new HearingRole(14, "Judicial Office Holder") {UserRole = new UserRole(7, "Judicial Office Holder")};

            var person = new PersonBuilder().Build();
            var joh = new JudicialOfficeHolder(person, hearingRole, caseRole)
            {
                DisplayName = "JOH",
                CreatedBy = "unit@test.com"
            };
            joh.SetProtected(nameof(joh.CaseRole), caseRole);
            joh.SetProtected(nameof(joh.HearingRole), hearingRole);

            var response = _mapper.MapParticipantToResponse(joh);
            
            AssertParticipantCommonDetails(response, joh, caseRole, hearingRole);
            AssertRepresentativeResponse(response, null);
            response.Organisation.Should().BeNullOrWhiteSpace();
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

        private static void AssertRepresentativeResponse(ParticipantResponse response, Representative representative)
        {
            if (representative == null)
            {
                response.Representee.Should().BeNullOrWhiteSpace();
            }
            else
            {
                response.Representee.Should().Be(representative.Representee);
            }
        }
    }
}