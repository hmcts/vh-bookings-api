﻿using System.Collections.Generic;
using Bookings.Api.Contract.Requests;
using FluentAssertions;
using Bookings.Common;
using Bookings.API.Mappings;
using Bookings.Domain.RefData;
using Bookings.UnitTests.Utilities;
using NUnit.Framework;

namespace Bookings.UnitTests.Mappings
{
    public class ParticipantRequestToNewParticipantMapperTest  : TestBase
    {
        private CaseType _caseType;

        [SetUp]
        public void Setup()
        {
            _caseType = new CaseType(1, "Money claims")
            {
                CaseRoles = new List<CaseRole>
                {
                    new CaseRole(0, "Claimant")
                    {
                        HearingRoles = new List<HearingRole>
                        {
                            new HearingRole(1, "Claimant LIP"),
                            new HearingRole(2, "Representative") { UserRole = new UserRole(1, "individual")}
                        }
                    },
                    new CaseRole(0, "Respondent")
                    {
                        HearingRoles = new List<HearingRole>
                        {
                            new HearingRole(1, "Respondent LIP"),
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

            When(() => new ParticipantRequestToNewParticipantMapper().MapRequestToNewParticipant(request, _caseType))
                .Should().Throw<BadRequestException>().WithMessage("Invalid case role [Missing case role]");
        }
        
        [Test]
        public void Should_raise_bad_request_exception_on_invalid_hearing_role()
        {
            var request = new ParticipantRequest
            {
                CaseRoleName = "Claimant",
                HearingRoleName = "Missing hearing role"
            };

            When(() => new ParticipantRequestToNewParticipantMapper().MapRequestToNewParticipant(request, _caseType))
                .Should().Throw<BadRequestException>().WithMessage("Invalid hearing role [Missing hearing role]");
        }

        [Test]
        public void Should_map_and_return_newparticipant_with_address_and_organistaion()
        {
            var request = new ParticipantRequest
            {
                Title = "Mr",
                FirstName = "Test",
                LastName = "Tester",
                Username = "TestTester",
                CaseRoleName = "Claimant",
                HearingRoleName = "Representative",
                HouseNumber = "123A",
                Street = "Test Street",
                Postcode = "SW1V 1AB",
                City = "Westminister",
                County = "London"
            };

            var newParticipant = new ParticipantRequestToNewParticipantMapper().MapRequestToNewParticipant(request, _caseType);
            newParticipant.Should().NotBeNull();
            var person = newParticipant.Person;
            person.Should().NotBeNull();
            person.Address.Should().NotBeNull();
            person.Organisation.Should().BeNull();
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
                OrganisationName = "Test Corp Ltd"
            };

            var newParticipant = new ParticipantRequestToNewParticipantMapper().MapRequestToNewParticipant(request, _caseType);
            newParticipant.Should().NotBeNull();
            var person = newParticipant.Person;
            person.Should().NotBeNull();
            person.Address.Should().BeNull();
            person.Organisation.Should().NotBeNull();
        }
    }
}
