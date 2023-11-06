using System.Collections.Generic;
using System.Net;
using BookingsApi.Domain;
using BookingsApi.Domain.RefData;
using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Queries;
using BookingsApi.UnitTests.Controllers.HearingsController.Helpers;
using Microsoft.AspNetCore.Mvc;
using Testing.Common.Assertions;
using BookingsApi.Infrastructure.Services.IntegrationEvents.Events;
using BookingsApi.Contract.V1.Requests;
using BookingsApi.Validations;
using BookingsApi.Domain.Participants;

namespace BookingsApi.UnitTests.Controllers.HearingsController
{
    public class BookNewHearingTests : HearingsControllerTests
    {
        private readonly BookNewHearingRequest request = RequestBuilderV1.Build();
        private VideoHearing _videoHearing;

        private static List<CaseRole> CaseRoles => new List<CaseRole>
        {
            CreateCaseAndHearingRoles(1, "Applicant",new List<string>{ "Litigant in person", "Representative"}),
            CreateCaseAndHearingRoles(2, "Respondent",new List<string>{ "Litigant in person", "Representative"}),
            CreateCaseAndHearingRoles(3, "Judge", new List<string>{ "Judge"}),
            CreateCaseAndHearingRoles(4, "Judicial Office Holder", new List<string> { "Judicial Office Holder" })
        };

        private static CaseRole CreateCaseAndHearingRoles(int caseId, string caseRoleName, List<string> roles)
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
            CommandHandlerMock.Reset();
            QueryHandlerMock.Reset();
            var caseType = new CaseType(1, "Civil")
            {
                CaseRoles = CaseRoles,
                HearingTypes = new List<HearingType> { new HearingType("Automated Test") { Code = "AutomatedTest" } }
            };

            QueryHandlerMock
            .Setup(x => x.Handle<GetCaseRolesForCaseTypeQuery, CaseType>(It.IsAny<GetCaseRolesForCaseTypeQuery>()))
            .ReturnsAsync(caseType);

            QueryHandlerMock
            .Setup(x => x.Handle<GetHearingVenuesQuery, List<HearingVenue>>(It.IsAny<GetHearingVenuesQuery>()))
            .ReturnsAsync(new List<HearingVenue> { new HearingVenue(1, "Birmingham Civil and Family Justice Centre", venueCode: "TestVenueCode") });

            _videoHearing = GetHearing("123");

            QueryHandlerMock
             .Setup(x => x.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>()))
             .ReturnsAsync(_videoHearing);
        }

        [Test]
        public async Task Should_successfully_book_new_hearing_when_judge_not_present()
        {
            //remove judge from request
            request.Participants.Remove(request.Participants.Find(e => e.HearingRoleName == "Judge"));
            _videoHearing.Participants.Remove(_videoHearing.Participants.Single(x => x.HearingRole.Name == "Judge"));
            var response = await Controller.BookNewHearing(request);

            response.Should().NotBeNull();
            var result = (CreatedAtActionResult)response;
            result.StatusCode.Should().Be((int)HttpStatusCode.Created);

            QueryHandlerMock.Verify(x => x.Handle<GetCaseRolesForCaseTypeQuery, CaseType>(It.IsAny<GetCaseRolesForCaseTypeQuery>()), Times.Once);

            QueryHandlerMock.Verify(x => x.Handle<GetHearingVenuesQuery, List<HearingVenue>>(It.IsAny<GetHearingVenuesQuery>()), Times.Once);

            QueryHandlerMock.Verify(x => x.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>()), Times.Once);

            RandomGenerator.Verify(x => x.GetWeakDeterministic(It.IsAny<long>(), It.IsAny<uint>(), It.IsAny<uint>()), Times.Exactly(2));

            CommandHandlerMock.Verify(c => c.Handle(It.Is<CreateVideoHearingCommand>(c => c.Endpoints.Count == 1
                && c.Endpoints[0].DisplayName == request.Endpoints[0].DisplayName
                && c.Endpoints[0].Sip == "@WhereAreYou.com")), Times.Once);

            EventPublisherMock.Verify(x => x.PublishAsync(It.IsAny<NewParticipantWelcomeEmailEvent>()), Times.Exactly(_videoHearing.Participants.Count(x => x is Individual)));
            EventPublisherMock.Verify(x => x.PublishAsync(It.IsAny<HearingIsReadyForVideoIntegrationEvent>()), Times.Once);
            CommandHandlerMock.Verify(x => x.Handle(It.IsAny<UpdateHearingStatusCommand>()), Times.Never);
        }
        
        [Test]
        public async Task Should_successfully_book_hearing_without_endpoint()
        {
            var newRequest = RequestBuilderV1.Build();
            newRequest.Endpoints = null;
            var response = await Controller.BookNewHearing(newRequest);

            response.Should().NotBeNull();
            var result = (CreatedAtActionResult)response;
            result.StatusCode.Should().Be((int)HttpStatusCode.Created);

            QueryHandlerMock.Verify(x => x.Handle<GetCaseRolesForCaseTypeQuery, CaseType>(It.IsAny<GetCaseRolesForCaseTypeQuery>()), Times.Once);

            QueryHandlerMock.Verify(x => x.Handle<GetHearingVenuesQuery, List<HearingVenue>>(It.IsAny<GetHearingVenuesQuery>()), Times.Once);

            QueryHandlerMock.Verify(x => x.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>()), Times.Once);

            RandomGenerator.Verify(x => x.GetWeakDeterministic(It.IsAny<long>(), It.IsAny<uint>(), It.IsAny<uint>()), Times.Never);

            CommandHandlerMock.Verify(c => c.Handle(It.Is<CreateVideoHearingCommand>(c => c.Endpoints.Count == 0)), Times.Once);
            EventPublisherMock.Verify(x => x.PublishAsync(It.IsAny<HearingIsReadyForVideoIntegrationEvent>()), Times.Once);
        }

        [Test]
        public async Task Should_successfully_book_first_day_of_multiday_hearing_without_Judge()
        {
            
            //remove judge from request
            request.Participants.Remove(request.Participants.Find(e => e.HearingRoleName == "Judge"));
            request.IsMultiDayHearing = true;
            _videoHearing.Participants.Remove(_videoHearing.Participants.Single(x => x.HearingRole.Name == "Judge"));
            var response = await Controller.BookNewHearing(request);

            response.Should().NotBeNull();
            var result = (CreatedAtActionResult)response;
            result.StatusCode.Should().Be((int)HttpStatusCode.Created);

            QueryHandlerMock.Verify(x => x.Handle<GetCaseRolesForCaseTypeQuery, CaseType>(It.IsAny<GetCaseRolesForCaseTypeQuery>()), Times.Once);

            QueryHandlerMock.Verify(x => x.Handle<GetHearingVenuesQuery, List<HearingVenue>>(It.IsAny<GetHearingVenuesQuery>()), Times.Once);

            QueryHandlerMock.Verify(x => x.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>()), Times.Once);

            RandomGenerator.Verify(x => x.GetWeakDeterministic(It.IsAny<long>(), It.IsAny<uint>(), It.IsAny<uint>()), Times.Exactly(2));

            CommandHandlerMock.Verify(c => c.Handle(It.Is<CreateVideoHearingCommand>(c => c.Endpoints.Count == 1
                && c.Endpoints[0].DisplayName == request.Endpoints[0].DisplayName
                && c.Endpoints[0].Sip == "@WhereAreYou.com")), Times.Once);

            EventPublisherMock.Verify(x => x.PublishAsync(It.IsAny<NewParticipantWelcomeEmailEvent>()), Times.Exactly(_videoHearing.Participants.Count(x => x is Individual)));
            EventPublisherMock.Verify(x => x.PublishAsync(It.IsAny<HearingIsReadyForVideoIntegrationEvent>()), Times.Once);
            CommandHandlerMock.Verify(x => x.Handle(It.IsAny<UpdateHearingStatusCommand>()), Times.Never);
        }

        [Test]
        public async Task Should_return_badrequest_with_empty_request()
        {
            var result = await Controller.BookNewHearing(null);

            result.Should().NotBeNull();
            var objectResult = (ObjectResult)result;
            ((ValidationProblemDetails)objectResult.Value).ContainsKeyAndErrorMessage(nameof(BookNewHearingRequest), "BookNewHearingRequest is null");
        }
        

        [Test]
        public void Should_log_exception_when_thrown_with_request_details()
        {
            
            var newRequest = RequestBuilderV1.Build();
            QueryHandlerMock.Setup(qh => qh.Handle<GetCaseRolesForCaseTypeQuery, CaseType>(It.IsAny<GetCaseRolesForCaseTypeQuery>()))
                .Throws<Exception>();

            Assert.ThrowsAsync<Exception>(async () => await Controller.BookNewHearing(newRequest));
        }
    }
}
