using System.Collections.Generic;
using BookingsApi.Contract.V1.Requests;
using FluentAssertions;
using BookingsApi.Common;
using BookingsApi.Mappings;
using BookingsApi.Domain.RefData;
using BookingsApi.UnitTests.Utilities;
using NUnit.Framework;

namespace BookingsApi.UnitTests.Mappings
{
    public class ParticipantRequestToNewParticipantMapperTest : TestBase
    {
        private CaseType _caseType;

        [SetUp]
        public void Setup()
        {
            _caseType = new CaseType(1, "Money claims")
            {
                CaseRoles = new List<CaseRole>
                {
                    new CaseRole(0, "Applicant")
                    {
                        HearingRoles = new List<HearingRole>
                        {
                            new HearingRole(1, "Litigant in person"),
                            new HearingRole(2, "Representative") { UserRole = new UserRole(1, "individual")}
                        }
                    },
                    new CaseRole(0, "Respondent")
                    {
                        HearingRoles = new List<HearingRole>
                        {
                            new HearingRole(1, "Litigant in person"),
                            new HearingRole(2, "Representative") { UserRole = new UserRole(1, "representative")}
                        }
                    }
                }
            };
        }
        
        [Test]
        public void Should_raise_bad_request_exception_on_invalid_case_role()
        {
            var request = new ParticipantRequest
            {
                CaseRoleName = "Missing case role",
                HearingRoleName = "Representative"
            };

            When(() => ParticipantRequestToNewParticipantMapper.Map(request, _caseType))
                .Should().Throw<BadRequestException>().WithMessage("Invalid case role [Missing case role]");
        }
        
        [Test]
        public void Should_raise_bad_request_exception_on_invalid_hearing_role()
        {
            var request = new ParticipantRequest
            {
                CaseRoleName = "Applicant",
                HearingRoleName = "Missing hearing role"
            };

            When(() => ParticipantRequestToNewParticipantMapper.Map(request, _caseType))
                .Should().Throw<BadRequestException>().WithMessage("Invalid hearing role [Missing hearing role]");
        }

        [Test]
        public void Should_map_and_return_newparticipant_with_organistaion()
        {
            var request = new ParticipantRequest
            {
                Title = "Mr",
                FirstName = "Test",
                LastName = "Tester",
                Username = "TestTester",
                CaseRoleName = "Respondent",
                HearingRoleName = "Representative",
                OrganisationName = "Test Corp Ltd",
                ContactEmail = "contact@contact.com"
            };

            var newParticipant = ParticipantRequestToNewParticipantMapper.Map(request, _caseType);
            newParticipant.Should().NotBeNull();
            var person = newParticipant.Person;
            person.Should().NotBeNull();
            person.Title.Should().Be("Mr");
            person.FirstName.Should().Be("Test");
            person.LastName.Should().Be("Tester");
            person.Username.Should().Be("TestTester");
            person.ContactEmail.Should().Be("contact@contact.com");
            person.Organisation.Name.Should().Be("Test Corp Ltd");

            var caseRole = newParticipant.CaseRole;
            caseRole.Name.Should().Be("Respondent");
        }

        [Test]
        public void Should_map_username_to_contact_email_when_username_empty()
        {
            var request = new ParticipantRequest
            {
                Title = "Mr",
                FirstName = "Test",
                LastName = "Tester",
                Username = "",
                CaseRoleName = "Respondent",
                HearingRoleName = "Representative",
                OrganisationName = "Test Corp Ltd",
                ContactEmail = "contact@contact.com"
            };
            
            var newParticipant = ParticipantRequestToNewParticipantMapper.Map(request, _caseType);
            newParticipant.Should().NotBeNull();
            var person = newParticipant.Person;
            person.ContactEmail.Should().Be("contact@contact.com");
            person.Username.Should().Be("contact@contact.com");
        }
    }
}
