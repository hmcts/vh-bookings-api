using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using AcceptanceTests.Common.Api.Helpers;
using AcceptanceTests.Common.Model.Case;
using Bookings.AcceptanceTests.Contexts;
using Bookings.AcceptanceTests.Models;
using Bookings.Api.Contract.Queries;
using Bookings.Api.Contract.Requests;
using Bookings.Api.Contract.Responses;
using FluentAssertions;
using TechTalk.SpecFlow;
using Bookings.Api.Contract.Requests.Enums;
using UpdateBookingStatusRequest = Bookings.AcceptanceTests.Models.UpdateBookingStatusRequest;
using UpdateHearingRequest = Bookings.AcceptanceTests.Models.UpdateHearingRequest;
using static Testing.Common.Builders.Api.ApiUriFactory.HearingsEndpoints;


namespace Bookings.AcceptanceTests.Steps
{
    [Binding]
    public sealed class HearingsSteps
    {
        private readonly TestContext _context;
        
        public HearingsSteps(TestContext context)
        {
            _context = context;
        }

        [Given(@"I have a get details for a given hearing request with a valid hearing id")]
        public void GivenIHaveAGetDetailsForAGivenHearingRequestWithAValidHearingId()
        {
            _context.Request = _context.Get(GetHearingDetailsById(_context.TestData.Hearing.Id));
        }

        [Given(@"I have a valid book a new hearing request")]
        public void GivenIHaveAValidBookANewHearingRequest()
        {
            var bookNewHearingRequest = new CreateHearingRequestBuilder(_context.TestData.CaseName)
                .WithParticipant("Judicial Office Holder", "Judicial Office Holder")
                .WithContext(_context)
                .Build();

            _context.Request = _context.Post(BookNewHearing, bookNewHearingRequest);
        }

        [Given(@"I have a valid update hearing request")]
        public void GivenIHaveAValidUpdateHearingRequest()
        {
            var updateHearingRequest = UpdateHearingRequest.BuildRequest(_context.TestData.CaseName);
            _context.Request = _context.Put(UpdateHearingDetails(_context.TestData.Hearing.Id), updateHearingRequest);
        }

        [Given(@"I have a valid get hearing by username request")]
        public void GivenIHaveAValidGetHearingByUsernameRequest()
        {
            _context.Request = _context.Get(GetHearingsByUsername(_context.TestData.ParticipantsResponses.First().Username));
        }

        [Given(@"I have a remove hearing request with a valid hearing id")]
        public void GivenIHaveARemoveHearingRequestWithAValidHearingId()
        {
            _context.Request = _context.Delete(RemoveHearing(_context.TestData.Hearing.Id));
        }       

        [Then(@"hearing details should be retrieved")]
        public void ThenAHearingDetailsShouldBeRetrieved()
        {
            var model = RequestHelper.DeserialiseSnakeCaseJsonToResponse<HearingDetailsResponse>(_context.Response.Content);
            model.Should().NotBeNull();
            _context.TestData.Hearing = model;
            model.Should().BeEquivalentTo(_context.TestData.CreateHearingRequest,
                o => o.Excluding(x => x.Participants).Excluding(x => x.Endpoints));

            var expectedIndividuals = _context.TestData.CreateHearingRequest.Participants.FindAll(x => x.HearingRoleName.Contains("Claimant") || x.HearingRoleName.Contains("Defendant"));
            var actualIndividuals = model.Participants.FindAll(x => x.HearingRoleName.Contains("Claimant") || x.HearingRoleName.Contains("Defendant"));
            expectedIndividuals.Should().BeEquivalentTo(actualIndividuals, o =>
            {
                return o.Excluding(x => x.Representee).ExcludingMissingMembers();
            });

            var expectedRepresentatives = _context.TestData.CreateHearingRequest.Participants.FindAll(x => x.HearingRoleName.Contains("Representative"));
            var actualRepresentatives = model.Participants.FindAll(x => x.HearingRoleName.Contains("Representative"));
            ParticipantsDetailsMatch(expectedRepresentatives, actualRepresentatives);

            var expectedJudge = _context.TestData.CreateHearingRequest.Participants.FindAll(x => x.HearingRoleName.Contains("Judge"));
            var actualJudge = model.Participants.FindAll(x => x.HearingRoleName.Contains("Judge"));
            ParticipantsDetailsMatch(expectedJudge, actualJudge);
            _context.TestData.CreateHearingRequest.Endpoints.Should().BeEquivalentTo(model.Endpoints, x => x.ExcludingMissingMembers());
        }

        private static void ParticipantsDetailsMatch(IEnumerable<ParticipantRequest> expected, IEnumerable<ParticipantResponse> actual)
        {
            expected.Should().BeEquivalentTo(actual, o =>
            {
                o.ExcludingMissingMembers();
                return o;
            });
        }

        [Then(@"hearing details should be updated")]
        public void ThenHearingDetailsShouldBeUpdated()
        {
            var model = RequestHelper.DeserialiseSnakeCaseJsonToResponse<HearingDetailsResponse>(_context.Response.Content);
            model.Should().NotBeNull();
            model.ScheduledDuration.Should().Be(100);
            model.ScheduledDateTime.Should().Be(DateTime.Today.AddDays(3).AddHours(11).AddMinutes(45).ToUniversalTime());
            model.HearingVenueName.Should().Be("Manchester Civil and Family Justice Centre");
            model.OtherInformation.Should().Be("OtherInformation12345");
            model.HearingRoomName.Should().Be("HearingRoomName12345");
            model.QuestionnaireNotRequired.Should().Be(true);
            model.AudioRecordingRequired.Should().Be(true);

            foreach (var participant in model.Participants)
            {
                participant.CaseRoleName.Should().NotBeNullOrEmpty();
                participant.ContactEmail.Should().NotBeNullOrEmpty();
                participant.DisplayName.Should().NotBeNullOrEmpty();
                participant.FirstName.Should().NotBeNullOrEmpty();
                participant.HearingRoleName.Should().NotBeNullOrEmpty();
                participant.Id.Should().NotBeEmpty();
                participant.LastName.Should().NotBeNullOrEmpty();
                participant.MiddleNames.Should().NotBeNullOrEmpty();
                participant.TelephoneNumber.Should().NotBeNullOrEmpty();
                participant.Title.Should().NotBeNullOrEmpty();
                participant.UserRoleName.Should().NotBeNullOrEmpty();
            }
        }

        [Then(@"the hearing no longer exists")]
        public void ThenTheHearingNoLongerExists()
        {
            _context.Request = _context.Get(GetHearingDetailsById(_context.TestData.Hearing.Id));
            _context.Response = _context.Client().Execute(_context.Request);
            _context.Response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Given(@"I have a valid book a new hearing for a case type (.*)")]
        public void GivenIHaveAValidBookANewHearingForACaseType(string caseTypeName)
        {
            var request = new CreateHearingRequestBuilder(_context.TestData.CaseName).WithContext(_context).Build();
            request.ScheduledDateTime = DateTime.Now.AddDays(2);
            request.CaseTypeName = caseTypeName;
            _context.Request = _context.Post(BookNewHearing, request);
            _context.Response = _context.Client().Execute(_context.Request);
            _context.Response.StatusCode.Should().Be(HttpStatusCode.Created);
            var model = RequestHelper.DeserialiseSnakeCaseJsonToResponse<HearingDetailsResponse>(_context.Response.Content);
            model.Should().NotBeNull();
            _context.TestData.Hearing = model;
        }

        [Given(@"I have a get details for a given hearing request for case type (.*)")]
        public void GivenIHaveAGetDetailsForAGivenHearingRequestWithAValidCaseType(string caseTypeString)
        {
            _context.Request = _context.Get(GetHearingsByCaseType(CaseType.FromString(caseTypeString).Id));
            _context.Request.AddQueryParameter("Limit", "1000");
        }

        [Then(@"hearing details should be retrieved for the case type")]
        public void ThenHearingDetailsShouldBeRetrievedForTheCaseType()
        {
            var response = RequestHelper.DeserialiseSnakeCaseJsonToResponse<BookingsResponse>(_context.Response.Content);
            response.PrevPageUrl.Should().Contain(response.Limit.ToString());
            response.Hearings.Count.Should().BeGreaterThan(0);
            var hearing = HearingInResponse(response);
            hearing.Should().NotBeNull();
            hearing.CaseTypeName.Should().NotBeNullOrEmpty();
            hearing.HearingTypeName.Should().NotBeNullOrEmpty();
            hearing.ScheduledDateTime.Should().BeAfter(DateTime.UtcNow);
            hearing.ScheduledDuration.Should().NotBe(0);
            hearing.JudgeName.Should().NotBeNullOrEmpty();
            hearing.CourtAddress.Should().NotBeNullOrEmpty();
            hearing.HearingName.Should().NotBeNullOrEmpty();
            hearing.HearingNumber.Should().NotBeNullOrEmpty();
        }

        private BookingsHearingResponse HearingInResponse(BookingsResponse response)
        {
            foreach (var pageInHearingsResponse in response.Hearings)
            {
                foreach (var hearing in pageInHearingsResponse.Hearings)
                {
                    if (hearing.HearingId.Equals(_context.TestData.Hearing.Id))
                    {
                        return hearing;
                    }
                }
            }

            throw new DataException("Expected hearing not found in the response.");
        }

        [Given(@"I have a cancel hearing request with a valid hearing id")]
        public void GivenIHaveACancelHearingRequestWithAValidHearingId()
        {
            var updateHearingStatusRequest = UpdateBookingStatusRequest.BuildRequest(UpdateBookingStatus.Cancelled);
            _context.Request = _context.Patch(UpdateHearingDetails(_context.TestData.Hearing.Id), updateHearingStatusRequest);
        }

        [When(@"I have a failed confirmation hearing request with a valid hearing id")]
        public void GivenIHaveAFailedConfirmationHearingRequestWithAValidHearingId()
        {
            var updateHearingStatusRequest = UpdateBookingStatusRequest.BuildRequest(UpdateBookingStatus.Failed);
            _context.Request = _context.Patch(UpdateHearingDetails(_context.TestData.Hearing.Id), updateHearingStatusRequest);
        }

        [Given(@"I have a created hearing request with a valid hearing id")]
        public void GivenIHaveACreatedHearingRequestWithAValidHearingId()
        {
            var updateHearingStatusRequest = UpdateBookingStatusRequest.BuildRequest(UpdateBookingStatus.Created);
            _context.Request = _context.Patch(UpdateHearingDetails(_context.TestData.Hearing.Id), updateHearingStatusRequest);
        }

        [Then(@"hearing should be (.*)")]
        public void ThenHearingShouldBe(Domain.Enumerations.BookingStatus status)
        {
            _context.Request = _context.Get(GetHearingDetailsById(_context.TestData.Hearing.Id));
            _context.Response = _context.Client().Execute(_context.Request);
            var model = RequestHelper.DeserialiseSnakeCaseJsonToResponse<HearingDetailsResponse>(_context.Response.Content);
            model.UpdatedBy.Should().NotBeNullOrEmpty();
            model.Status.Should().Be(status);
            if(status == Domain.Enumerations.BookingStatus.Created)
            {
                model.ConfirmedBy.Should().NotBeNullOrEmpty();
            }
        }

        [Then(@"a list of hearing details should be retrieved")]
        public void ThenAListOfHearingDetailsShouldBeRetrieved()
        {
            var model = RequestHelper.DeserialiseSnakeCaseJsonToResponse<List<HearingDetailsResponse>>(_context.Response.Content);
            model.Should().NotBeNull();
            _context.TestData.Hearing.Id = model.First().Id;

            foreach (var hearing in model)
            {
                hearing.CaseTypeName.Should().NotBeNullOrEmpty();
                foreach (var theCase in hearing.Cases)
                {
                    theCase.Name.Should().NotBeNullOrEmpty();
                    theCase.Number.Should().NotBeNullOrEmpty();
                }
                hearing.HearingTypeName.Should().NotBeNullOrEmpty();
                hearing.HearingVenueName.Should().NotBeNullOrEmpty();
                foreach (var participant in hearing.Participants)
                {
                    participant.CaseRoleName.Should().NotBeNullOrEmpty();
                    participant.ContactEmail.Should().NotBeNullOrEmpty();
                    participant.DisplayName.Should().NotBeNullOrEmpty();
                    participant.FirstName.Should().NotBeNullOrEmpty();
                    participant.HearingRoleName.Should().NotBeNullOrEmpty();
                    participant.Id.Should().NotBeEmpty();
                    participant.LastName.Should().NotBeNullOrEmpty();
                    participant.MiddleNames.Should().NotBeNullOrEmpty();
                    participant.TelephoneNumber.Should().NotBeNullOrEmpty();
                    participant.Title.Should().NotBeNullOrEmpty();
                    participant.UserRoleName.Should().NotBeNullOrEmpty();
                }
                hearing.ScheduledDateTime.Should().BeAfter(DateTime.MinValue);
                hearing.ScheduledDuration.Should().BePositive();
                hearing.HearingRoomName.Should().NotBeNullOrEmpty();
                hearing.OtherInformation.Should().NotBeNullOrEmpty();
                hearing.CreatedBy.Should().NotBeNullOrEmpty();
                hearing.Endpoints.Should().NotBeNullOrEmpty();
                foreach (var endpoint in hearing.Endpoints)
                {
                    endpoint.Id.Should().NotBeEmpty();
                    endpoint.DisplayName.Should().NotBeNullOrEmpty();
                    endpoint.Sip.Should().NotBeNullOrEmpty();
                    endpoint.Pin.Should().NotBeNullOrEmpty();
                }

                hearing.GroupId.Should().Be(hearing.Id);
            }          
        }

        [Then(@"a list of hearing details should be retrieved for the case number")]
        public void ThenAListOfHearingDetailsShouldBeRetrievedForTheCaseNumber()
        {
            var model = RequestHelper.DeserialiseSnakeCaseJsonToResponse<List<AudioRecordedHearingsBySearchResponse>>(_context.Response.Content);
            model.Should().NotBeNull();
            foreach (var hearing in model)
            {
                hearing.ScheduledDateTime.Should().BeAfter(DateTime.MinValue);
                hearing.CaseName.Should().NotBeNullOrEmpty();
                hearing.CaseNumber.Should().NotBeNullOrEmpty();
                hearing.HearingVenueName.Should().NotBeNullOrEmpty();
                hearing.HearingRoomName.Should().NotBeNullOrEmpty();
                hearing.CourtroomAccount.Should().NotBeNullOrEmpty();
                hearing.CourtroomAccountName.Should().NotBeNullOrEmpty();
            }
        }

        [Then(@"an empty list of hearing details should be retrieved")]
        public void ThenAnEmptyListOfHearingDetailsShouldBeRetrieved()
        {
            var model = RequestHelper.DeserialiseSnakeCaseJsonToResponse<List<AudioRecordedHearingsBySearchResponse>>(_context.Response.Content);
            model.Should().NotBeNull();
            model.Count.Should().Be(0);
        }
        
        [Given(@"I have a valid search for recorded hearings by case number request")]
        public void GivenIHaveAValidSearchForHearingByCaseNumberRequest()
        {
            var caseResponse = _context.TestData.Hearing.Cases.First();
            var query =  new SearchForHearingsQuery
            {
                CaseNumber = caseResponse.Number
            };
            _context.Request = _context.Get(SearchForHearings(query));
        }

        [Given(@"I have an invalid search for recorded hearings by case number request")]
        public void GivenIHaveAnInvalidSearchForHearingByCaseNumberRequest()
        {
            var caseResponse = _context.TestData.Hearing.Cases.First();
            var query =  new SearchForHearingsQuery
            {
                CaseNumber = caseResponse.Number + "01"
            };
            _context.Request = _context.Get(SearchForHearings(query));
        }
    }
}