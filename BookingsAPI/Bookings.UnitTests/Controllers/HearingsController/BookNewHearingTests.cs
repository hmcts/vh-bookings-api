using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Bookings.Api.Contract.Requests;
using Bookings.DAL.Commands;
using Bookings.DAL.Queries;
using Bookings.Domain;
using Bookings.Domain.RefData;
using Bookings.UnitTests.Controllers.HearingsController.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Testing.Common.Assertions;

namespace Bookings.UnitTests.Controllers.HearingsController
{
    public class BookNewHearingTests : HearingsControllerTests
    {
        private readonly BookNewHearingRequest request = RequestBuilder.Build();

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


        [SetUp]
        public void TestInitialize()
        {
            var caseType = new CaseType(1, "Civil")
            {
                CaseRoles = CaseRoles,
                HearingTypes = new List<HearingType> { new HearingType("Application to Set Judgment Aside") }
            };

            QueryHandlerMock
            .Setup(x => x.Handle<GetCaseTypeQuery, CaseType>(It.IsAny<GetCaseTypeQuery>()))
            .ReturnsAsync(caseType);

            QueryHandlerMock
            .Setup(x => x.Handle<GetHearingVenuesQuery, List<HearingVenue>>(It.IsAny<GetHearingVenuesQuery>()))
            .ReturnsAsync(new List<HearingVenue> { new HearingVenue(1, "Birmingham Civil and Family Justice Centre") });

            var hearing = GetHearing("123");

            QueryHandlerMock
             .Setup(x => x.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>()))
             .ReturnsAsync(hearing);
        }


        [Test]
        public async Task Should_successfully_book_new_hearing()
        { 
            var response = await Controller.BookNewHearing(request);

            response.Should().NotBeNull();
            var result = (CreatedAtActionResult)response;
            result.StatusCode.Should().Be((int)HttpStatusCode.Created);

            QueryHandlerMock.Verify(x => x.Handle<GetCaseTypeQuery, CaseType>(It.IsAny<GetCaseTypeQuery>()), Times.Once);

            QueryHandlerMock.Verify(x => x.Handle<GetHearingVenuesQuery, List<HearingVenue>>(It.IsAny<GetHearingVenuesQuery>()), Times.Once);

            QueryHandlerMock.Verify(x => x.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>()), Times.Once);

            RandomGenerator.Verify(x => x.GetWeakDeterministic(It.IsAny<long>(), It.IsAny<uint>(), It.IsAny<uint>()), Times.Exactly(2));
            
            CommandHandlerMock.Verify(c => c.Handle(It.Is<CreateVideoHearingCommand>(c => c.Endpoints.Count == 1 
                                                                                        && c.Endpoints[0].DisplayName == request.Endpoints[0].DisplayName
                                                                                        && c.Endpoints[0].Sip == "@WhereAreYou.com")), Times.Once);
        }

        [Test]
        public async Task Should_successfully_book_hearing_without_endpoint()
        {
            var newRequest = RequestBuilder.Build();
            newRequest.Endpoints = null;
            var response = await Controller.BookNewHearing(newRequest);

            response.Should().NotBeNull();
            var result = (CreatedAtActionResult)response;
            result.StatusCode.Should().Be((int)HttpStatusCode.Created);

            QueryHandlerMock.Verify(x => x.Handle<GetCaseTypeQuery, CaseType>(It.IsAny<GetCaseTypeQuery>()), Times.Once);

            QueryHandlerMock.Verify(x => x.Handle<GetHearingVenuesQuery, List<HearingVenue>>(It.IsAny<GetHearingVenuesQuery>()), Times.Once);

            QueryHandlerMock.Verify(x => x.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>()), Times.Once);

            RandomGenerator.Verify(x => x.GetWeakDeterministic(It.IsAny<long>(), It.IsAny<uint>(), It.IsAny<uint>()), Times.Never);

            CommandHandlerMock.Verify(c => c.Handle(It.Is<CreateVideoHearingCommand>(c => c.Endpoints.Count == 0)), Times.Once);
        }

        [Test]
        public async Task Should_return_badrequest_without_matching_casetype()
        {
            QueryHandlerMock
           .Setup(x => x.Handle<GetCaseTypeQuery, CaseType>(It.IsAny<GetCaseTypeQuery>()))
           .ReturnsAsync((CaseType)null);

            var result = await Controller.BookNewHearing(request);

            result.Should().NotBeNull();
            var objectResult = (BadRequestObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            ((SerializableError)objectResult.Value).ContainsKeyAndErrorMessage(nameof(request.CaseTypeName), "Case type does not exist");
        }

        [Test]
        public async Task Should_return_badrequest_without_matching_hearingtype()
        {
            var caseType = new CaseType(1, "Civil")
            {
                CaseRoles = CaseRoles,
                HearingTypes = new List<HearingType> { new HearingType("Not matching") }
            };

            QueryHandlerMock
            .Setup(x => x.Handle<GetCaseTypeQuery, CaseType>(It.IsAny<GetCaseTypeQuery>()))
            .ReturnsAsync(caseType);

            var result = await Controller.BookNewHearing(request);

            result.Should().NotBeNull();
            var objectResult = (BadRequestObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            ((SerializableError)objectResult.Value).ContainsKeyAndErrorMessage(nameof(request.HearingTypeName), "Hearing type does not exist");
        }

        [Test]
        public async Task Should_return_badrequest_without_matching_hearingvenue()
        {
            QueryHandlerMock
           .Setup(x => x.Handle<GetHearingVenuesQuery, List<HearingVenue>>(It.IsAny<GetHearingVenuesQuery>()))
           .ReturnsAsync(new List<HearingVenue> { new HearingVenue(1, "Not matching") });

            var result = await Controller.BookNewHearing(request);

            result.Should().NotBeNull();
            var objectResult = (BadRequestObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            ((SerializableError)objectResult.Value).ContainsKeyAndErrorMessage(nameof(request.HearingVenueName), "Hearing venue does not exist");
        }

        [Test]
        public async Task Should_return_badrequest_with_empty_request()
        {
            var result = await Controller.BookNewHearing(null);

            result.Should().NotBeNull();
            var objectResult = (BadRequestObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            ((SerializableError)objectResult.Value).ContainsKeyAndErrorMessage(nameof(BookNewHearingRequest), "BookNewHearingRequest is null");
        }

        [Test]
        public async Task Should_return_badrequest_without_valid_request()
        {
            var newRequest = RequestBuilder.Build();
            newRequest.CaseTypeName = string.Empty;

            var result = await Controller.BookNewHearing(newRequest);

            result.Should().NotBeNull();
            var objectResult = (BadRequestObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            var errors = (Dictionary<string, object>) ((SerializableError)objectResult.Value);
            errors.Should().ContainKey("CaseTypeName");
            ((string[])errors["CaseTypeName"])[0].Should().Be("Please provide a case type name");
        }

        [Test]
        public async Task Should_log_exception_when_thrown_with_request_details()
        {
            var newRequest = RequestBuilder.Build();
            QueryHandlerMock.Setup(qh => qh.Handle<GetCaseTypeQuery, CaseType>(It.IsAny<GetCaseTypeQuery>()))
                .Throws<Exception>();

            Assert.ThrowsAsync<Exception>(async () => await Controller.BookNewHearing(newRequest));
        }
    }
}
