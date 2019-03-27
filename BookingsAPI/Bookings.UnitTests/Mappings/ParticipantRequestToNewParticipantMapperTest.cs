using System;
using System.Collections.Generic;
using System.Linq;
using Bookings.Api.Contract.Requests;
using FluentAssertions;
using Bookings.Common;
using Bookings.Api.Contract.Responses;
using Bookings.API.Mappings;
using Bookings.API.Utilities;
using Bookings.Domain;
using Bookings.Domain.RefData;
using Bookings.UnitTests.Utilities;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
                            new HearingRole(2, "Solicitor")
                        }
                    },
                    new CaseRole(0, "Respondent")
                    {
                        HearingRoles = new List<HearingRole>
                        {
                            new HearingRole(1, "Respondent LIP"),
                            new HearingRole(2, "Solicitor")
                        }
                    }
                }
            };
        }
        
        [Test]
        public void should_raise_bad_request_exception_on_invalid_case_role()
        {
            var request = new ParticipantRequest
            {
                CaseRoleName = "Missing case role",
                HearingRoleName = "Solicitor"
            };

            When(() => new ParticipantRequestToNewParticipantMapper().MapRequestToNewParticipant(request, _caseType))
                .Should().Throw<BadRequestException>().WithMessage("Invalid case role [Missing case role]");
        }
        
        [Test]
        public void should_raise_bad_request_exception_on_invalid_hearing_role()
        {
            var request = new ParticipantRequest
            {
                CaseRoleName = "Claimant",
                HearingRoleName = "Missing hearing role"
            };

            When(() => new ParticipantRequestToNewParticipantMapper().MapRequestToNewParticipant(request, _caseType))
                .Should().Throw<BadRequestException>().WithMessage("Invalid hearing role [Missing hearing role]");
        }
    }
}
