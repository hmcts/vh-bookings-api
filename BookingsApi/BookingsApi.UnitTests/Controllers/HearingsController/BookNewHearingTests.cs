using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using BookingsApi.Contract.Requests;
using BookingsApi.Domain;
using BookingsApi.Domain.RefData;
using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Queries;
using BookingsApi.UnitTests.Controllers.HearingsController.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Testing.Common.Assertions;
using BookingsApi.Infrastructure.Services.IntegrationEvents.Events;
using System.Linq;
using BookingsApi.Validations;

namespace BookingsApi.UnitTests.Controllers.HearingsController
{
    public class BookNewHearingTests : HearingsControllerTests
    {
        private readonly BookNewHearingRequest request = RequestBuilder.Build();
        private VideoHearing _videoHearing;

        private List<CaseRole> CaseRoles => new List<CaseRole>
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
            var caseType = new CaseType(1, "Civil")
            {
                CaseRoles = CaseRoles,
                HearingTypes = new List<HearingType> { new HearingType("Automated Test") { Code = "AutomatedTest" } }
            };

            QueryHandlerMock
            .Setup(x => x.Handle<GetCaseTypeQuery, CaseType>(It.IsAny<GetCaseTypeQuery>()))
            .ReturnsAsync(caseType);

            QueryHandlerMock
            .Setup(x => x.Handle<GetHearingVenuesQuery, List<HearingVenue>>(It.IsAny<GetHearingVenuesQuery>()))
            .ReturnsAsync(new List<HearingVenue> { new HearingVenue(1, "Birmingham Civil and Family Justice Centre") });

            _videoHearing = GetHearing("123");

            QueryHandlerMock
             .Setup(x => x.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>()))
             .ReturnsAsync(_videoHearing);
        }


        [TestCase(true)]
        [TestCase(false)]
        public async Task Should_successfully_book_new_hearing(bool referenceDataToggle)
        {
            if (referenceDataToggle)
                FeatureTogglesMock.Setup(r => r.ReferenceDataToggle()).Returns(true);

            var response = await Controller.BookNewHearing(request);

            response.Should().NotBeNull();
            var result = (CreatedAtActionResult)response;
            result.StatusCode.Should().Be((int)HttpStatusCode.Created);

            if (referenceDataToggle)
                QueryHandlerMock.Verify(x => x.Handle<GetCaseTypeQuery, CaseType>(It.Is<GetCaseTypeQuery>(e => e.CaseTypeQueryParameter == request.CaseTypeServiceId)), Times.Once);
            else
                QueryHandlerMock.Verify(x => x.Handle<GetCaseTypeQuery, CaseType>(It.Is<GetCaseTypeQuery>(e => e.CaseTypeQueryParameter == request.CaseTypeName)), Times.Once);        

            QueryHandlerMock.Verify(x => x.Handle<GetHearingVenuesQuery, List<HearingVenue>>(It.IsAny<GetHearingVenuesQuery>()), Times.Once);

            QueryHandlerMock.Verify(x => x.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>()), Times.Once);

            RandomGenerator.Verify(x => x.GetWeakDeterministic(It.IsAny<long>(), It.IsAny<uint>(), It.IsAny<uint>()), Times.Exactly(2));

            CommandHandlerMock.Verify(c => c.Handle(It.Is<CreateVideoHearingCommand>(c => c.Endpoints.Count == 1
                                                                                        && c.Endpoints[0].DisplayName == request.Endpoints[0].DisplayName
                                                                                        && c.Endpoints[0].Sip == "@WhereAreYou.com")), Times.Once);

            EventPublisherMock.Verify(x => x.PublishAsync(It.IsAny<HearingIsReadyForVideoIntegrationEvent>()), Times.Once);
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

            QueryHandlerMock.Verify(x => x.Handle<GetCaseTypeQuery, CaseType>(It.IsAny<GetCaseTypeQuery>()), Times.Once);

            QueryHandlerMock.Verify(x => x.Handle<GetHearingVenuesQuery, List<HearingVenue>>(It.IsAny<GetHearingVenuesQuery>()), Times.Once);

            QueryHandlerMock.Verify(x => x.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>()), Times.Once);

            RandomGenerator.Verify(x => x.GetWeakDeterministic(It.IsAny<long>(), It.IsAny<uint>(), It.IsAny<uint>()), Times.Exactly(2));

            CommandHandlerMock.Verify(c => c.Handle(It.Is<CreateVideoHearingCommand>(c => c.Endpoints.Count == 1
                && c.Endpoints[0].DisplayName == request.Endpoints[0].DisplayName
                && c.Endpoints[0].Sip == "@WhereAreYou.com")), Times.Once);

            EventPublisherMock.Verify(x => x.PublishAsync(It.IsAny<CreateAndNotifyUserIntegrationEvent>()), Times.Once);
            CommandHandlerMock.Verify(x => x.Handle(It.IsAny<UpdateHearingStatusCommand>()), Times.Never);
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

            QueryHandlerMock.Verify(x => x.Handle<GetCaseTypeQuery, CaseType>(It.IsAny<GetCaseTypeQuery>()), Times.Once);

            QueryHandlerMock.Verify(x => x.Handle<GetHearingVenuesQuery, List<HearingVenue>>(It.IsAny<GetHearingVenuesQuery>()), Times.Once);

            QueryHandlerMock.Verify(x => x.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>()), Times.Once);

            RandomGenerator.Verify(x => x.GetWeakDeterministic(It.IsAny<long>(), It.IsAny<uint>(), It.IsAny<uint>()), Times.Exactly(2));

            CommandHandlerMock.Verify(c => c.Handle(It.Is<CreateVideoHearingCommand>(c => c.Endpoints.Count == 1
                && c.Endpoints[0].DisplayName == request.Endpoints[0].DisplayName
                && c.Endpoints[0].Sip == "@WhereAreYou.com")), Times.Once);

            EventPublisherMock.Verify(x => x.PublishAsync(It.IsAny<CreateAndNotifyUserIntegrationEvent>()), Times.Once);
            CommandHandlerMock.Verify(x => x.Handle(It.IsAny<UpdateHearingStatusCommand>()), Times.Never);
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task Should_return_badrequest_without_matching_casetype(bool referenceDataToggle)
        {
            if (referenceDataToggle)
                FeatureTogglesMock.Setup(r => r.ReferenceDataToggle()).Returns(true);
            
            QueryHandlerMock
           .Setup(x => x.Handle<GetCaseTypeQuery, CaseType>(It.IsAny<GetCaseTypeQuery>()))
           .ReturnsAsync((CaseType)null);

            var result = await Controller.BookNewHearing(request);

            result.Should().NotBeNull();
            var objectResult = (BadRequestObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            if(referenceDataToggle)
                ((SerializableError)objectResult.Value).ContainsKeyAndErrorMessage(nameof(request.CaseTypeServiceId), "Case type does not exist");
            else
                ((SerializableError)objectResult.Value).ContainsKeyAndErrorMessage(nameof(request.CaseTypeName), "Case type does not exist");
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task Should_return_badrequest_without_matching_hearingtype(bool referenceDataToggle)
        {
            if (referenceDataToggle)
                FeatureTogglesMock.Setup(r => r.ReferenceDataToggle()).Returns(true);
            
            var caseType = new CaseType(1, "Civil")
            {
                CaseRoles = CaseRoles,
                HearingTypes = new List<HearingType> { new HearingType("Not matching") { Code = "NotMatching"} }
            };

            QueryHandlerMock
            .Setup(x => x.Handle<GetCaseTypeQuery, CaseType>(It.IsAny<GetCaseTypeQuery>()))
            .ReturnsAsync(caseType);

            var result = await Controller.BookNewHearing(request);

            result.Should().NotBeNull();
            var objectResult = (BadRequestObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            if (referenceDataToggle)
            {
                ((SerializableError)objectResult.Value).ContainsKeyAndErrorMessage(nameof(request.HearingTypeCode), "Hearing type does not exist");
            }
            else
            {
                ((SerializableError)objectResult.Value).ContainsKeyAndErrorMessage(nameof(request.HearingTypeName), "Hearing type does not exist");
            }
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
            var errors = (Dictionary<string, object>)((SerializableError)objectResult.Value);
            errors.Should().ContainKey("CaseTypeName");
            ((string[])errors["CaseTypeName"])[0].Should().Be("Please provide a case type name");
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Should_log_exception_when_thrown_with_request_details(bool referenceDataToggle)
        {
            if (referenceDataToggle)
                FeatureTogglesMock.Setup(r => r.ReferenceDataToggle()).Returns(true);
            
            var newRequest = RequestBuilder.Build();
            QueryHandlerMock.Setup(qh => qh.Handle<GetCaseTypeQuery, CaseType>(It.IsAny<GetCaseTypeQuery>()))
                .Throws<Exception>();

            Assert.ThrowsAsync<Exception>(async () => await Controller.BookNewHearing(newRequest));
            if (referenceDataToggle)
              Logger.Verify(c => c.TrackError(It.IsAny<Exception>(), It.Is<Dictionary<string,string>>(e => e.Count == 7)), Times.Once());
        }

        [TestCase("FirstNameWithSpaces ")]
        [TestCase(" FirstNameWithSpaces")]
        [TestCase(" FirstNameWithSpaces ")]
        [TestCase(" FirstName WithSpaces ")]
        public async Task Should_successfully_book_hearing_with_participant_first_name_starting_or_ending_with_spaces(string firstName)
        {
            var newRequest = RequestBuilder.Build();
            newRequest.Participants[0].FirstName = firstName;
            
            var response = await Controller.BookNewHearing(newRequest);
            
            response.Should().NotBeNull();
            var result = (CreatedAtActionResult)response;
            result.StatusCode.Should().Be((int)HttpStatusCode.Created);

            QueryHandlerMock.Verify(x => x.Handle<GetCaseTypeQuery, CaseType>(It.IsAny<GetCaseTypeQuery>()), Times.Once);

            QueryHandlerMock.Verify(x => x.Handle<GetHearingVenuesQuery, List<HearingVenue>>(It.IsAny<GetHearingVenuesQuery>()), Times.Once);

            QueryHandlerMock.Verify(x => x.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>()), Times.Once);

            RandomGenerator.Verify(x => x.GetWeakDeterministic(It.IsAny<long>(), It.IsAny<uint>(), It.IsAny<uint>()), Times.Exactly(2));

            CommandHandlerMock.Verify(c => c.Handle(It.Is<CreateVideoHearingCommand>(c => c.Endpoints.Count == 1
                                                                                          && c.Endpoints[0].DisplayName == request.Endpoints[0].DisplayName
                                                                                          && c.Endpoints[0].Sip == "@WhereAreYou.com"
                                                                                          && c.Participants.Any(p => p.Person.FirstName == "FirstNameWithSpaces" || p.Person.FirstName == "FirstName WithSpaces"))), Times.Once);

            EventPublisherMock.Verify(x => x.PublishAsync(It.IsAny<HearingIsReadyForVideoIntegrationEvent>()), Times.Once);
        }
        
        [TestCase("LastNameWithSpaces ")]
        [TestCase(" LastNameWithSpaces")]
        [TestCase(" LastNameWithSpaces ")]
        [TestCase(" LastName WithSpaces ")]
        public async Task Should_successfully_book_hearing_with_participant_last_name_starting_or_ending_with_spaces(string lastName)
        {
            var newRequest = RequestBuilder.Build();
            newRequest.Participants[0].LastName = lastName;
            
            var response = await Controller.BookNewHearing(newRequest);
            
            response.Should().NotBeNull();
            var result = (CreatedAtActionResult)response;
            result.StatusCode.Should().Be((int)HttpStatusCode.Created);

            QueryHandlerMock.Verify(x => x.Handle<GetCaseTypeQuery, CaseType>(It.IsAny<GetCaseTypeQuery>()), Times.Once);

            QueryHandlerMock.Verify(x => x.Handle<GetHearingVenuesQuery, List<HearingVenue>>(It.IsAny<GetHearingVenuesQuery>()), Times.Once);

            QueryHandlerMock.Verify(x => x.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>()), Times.Once);

            RandomGenerator.Verify(x => x.GetWeakDeterministic(It.IsAny<long>(), It.IsAny<uint>(), It.IsAny<uint>()), Times.Exactly(2));

            CommandHandlerMock.Verify(c => c.Handle(It.Is<CreateVideoHearingCommand>(c => c.Endpoints.Count == 1
                                                                                          && c.Endpoints[0].DisplayName == request.Endpoints[0].DisplayName
                                                                                          && c.Endpoints[0].Sip == "@WhereAreYou.com"
                                                                                          && c.Participants.Any(p => p.Person.LastName == "LastNameWithSpaces" || p.Person.LastName == "LastName WithSpaces"))), Times.Once);

            EventPublisherMock.Verify(x => x.PublishAsync(It.IsAny<HearingIsReadyForVideoIntegrationEvent>()), Times.Once);
        }
        
        [TestCase(null)]
        [TestCase("")]
        public async Task Should_return_badrequest_with_missing_first_name(string firstName)
        {
            var newRequest = RequestBuilder.Build();
            newRequest.Participants[0].FirstName = firstName;
            
            var result = await Controller.BookNewHearing(newRequest);

            result.Should().NotBeNull();
            var objectResult = (BadRequestObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            ((SerializableError)objectResult.Value).ContainsKeyAndErrorMessage("Participants[0].FirstName", ParticipantRequestValidation.NoFirstNameErrorMessage);
        }
        
        [TestCase(null)]
        [TestCase("")]
        public async Task Should_return_badrequest_with_missing_last_name(string lastName)
        {
            var newRequest = RequestBuilder.Build();
            newRequest.Participants[0].LastName = lastName;
            
            var result = await Controller.BookNewHearing(newRequest);

            result.Should().NotBeNull();
            var objectResult = (BadRequestObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            ((SerializableError)objectResult.Value).ContainsKeyAndErrorMessage("Participants[0].LastName", ParticipantRequestValidation.NoLastNameErrorMessage);
        }
    }
}
