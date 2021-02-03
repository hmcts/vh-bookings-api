using System.Collections.Generic;
using BookingsApi.Common.Services;
using BookingsApi.Contract.Requests;
using BookingsApi.DAL.Commands;
using BookingsApi.Domain;
using BookingsApi.Domain.RefData;
using BookingsApi.Mappings;
using BookingsApi.UnitTests.Controllers.HearingsController.Helpers;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace BookingsApi.UnitTests.Mappings
{
    public class BookNewHearingRequestToCreateVideoHearingCommandMapperTests
    {
        private BookNewHearingRequest _request;
        private CaseType _caseType;
        private HearingType _hearingType;
        private HearingVenue _venue;
        private List<Case> _cases;
        private IRandomGenerator _randomGenerator;
        private readonly string _sipAddressStem = "dummytest.stem";
        [SetUp]
        public void SetUp()
        {
            _request = RequestBuilder.Build();
            _hearingType = GetHearingType();
            _caseType = GetCaseType(_hearingType);
            _venue = GetHearingVenue();
            _cases = GetCases();
            _randomGenerator = Mock.Of<IRandomGenerator>();
        }
        [Test]
        public void Should_Return_CreateVideoHearingCommand_Successfully()
        {
            var command = BookNewHearingRequestToCreateVideoHearingCommandMapper.Map(_request, _caseType, _hearingType,
                _venue, _cases, _randomGenerator, _sipAddressStem);
            command.Should().BeOfType<CreateVideoHearingCommand>();
            command.Should().NotBeNull();
            command.CaseType.Should().BeEquivalentTo(_caseType);
            command.HearingType.Should().BeEquivalentTo(_hearingType);
            command.Venue.Should().BeEquivalentTo(_venue);
            command.Cases.Should().BeEquivalentTo(_cases);
        }
        [Test]
        public void Should_Return_CreateVideoHearingCommand_Without_Endpoints_Successfully()
        {
            _request.Endpoints = null;
            var command = BookNewHearingRequestToCreateVideoHearingCommandMapper.Map(_request, _caseType, _hearingType,
                _venue, _cases, _randomGenerator, _sipAddressStem);
            command.Should().BeOfType<CreateVideoHearingCommand>();
            command.Should().NotBeNull();
            command.Endpoints.Should().BeNullOrEmpty();
        }
        [Test]
        public void Should_Return_CreateVideoHearingCommand_Without_LinkedParticipants_Successfully()
        {
            _request.LinkedParticipants = null;
            var command = BookNewHearingRequestToCreateVideoHearingCommandMapper.Map(_request, _caseType, _hearingType,
                _venue, _cases, _randomGenerator, _sipAddressStem);
            command.Should().BeOfType<CreateVideoHearingCommand>();
            command.Should().NotBeNull();
            command.LinkedParticipants.Should().BeNullOrEmpty();
        }
        private CaseType GetCaseType(HearingType hearingType)
        {
            return new CaseType(1, "Civil")
            {
                CaseRoles = CaseRoles,
                HearingTypes = new List<HearingType> { hearingType }
            };
        }
        private List<CaseRole> CaseRoles => new List<CaseRole> 
        {
            CreateCaseAndHearingRoles(1, "Claimant",new List<string>{ "Litigant in person", "Representative"}),
            CreateCaseAndHearingRoles(2, "Defendant",new List<string>{ "Litigant in person", "Representative"}),
            CreateCaseAndHearingRoles(3, "Judge", new List<string>{ "Judge"}),
            CreateCaseAndHearingRoles(4, "Judicial Office Holder", new List<string> { "Judicial Office Holder" })
        };
        private CaseRole CreateCaseAndHearingRoles(int caseId, string caseRoleName, List<string> roles)
        {
            var hearingRoles = new List<HearingRole>();
            foreach (var role in roles)
            {
                hearingRoles.Add(new HearingRole(1, role) { UserRole = new UserRole(1, "TestUser") });
            }
            var caseRole = new CaseRole(caseId, caseRoleName) { HearingRoles = hearingRoles };
            return caseRole;
        }
        private HearingType GetHearingType()
        {
            return new HearingType("Application to Set Judgment Aside");
        }
        private HearingVenue GetHearingVenue()
        {
            return new HearingVenue(1, "Birmingham Civil and Family Justice Centre");
        }
        private List<Case> GetCases()
        {
            return new List<Case> {new Case("1", "New case")};
        }
    }
}