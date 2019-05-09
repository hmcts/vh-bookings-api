using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Bookings.AcceptanceTests.Contexts;
using Bookings.AcceptanceTests.Models;
using Bookings.Api.Contract.Responses;
using FluentAssertions;
using TechTalk.SpecFlow;
using Testing.Common.Builders.Api;

namespace Bookings.AcceptanceTests.Steps
{
    [Binding]
    public sealed class HearingsSteps : BaseSteps
    {
        private readonly TestContext _acTestContext;
        private readonly HearingsEndpoints _endpoints = new ApiUriFactory().HearingsEndpoints;

        public HearingsSteps(TestContext acTestContext)
        {
            _acTestContext = acTestContext;
        }

        [Given(@"I have a get details for a given hearing request with a valid hearing id")]
        public void GivenIHaveAGetDetailsForAGivenHearingRequestWithAValidHearingId()
        {
            _acTestContext.Request = _acTestContext.Get(_endpoints.GetHearingDetailsById(_acTestContext.HearingId));
        }

        [Given(@"I have a valid book a new hearing request")]
        public void GivenIHaveAValidBookANewHearingRequest()
        {
            var bookNewHearingRequest = CreateHearingRequest.BuildRequest();         
            _acTestContext.Request = _acTestContext.Post(_endpoints.BookNewHearing(), bookNewHearingRequest);
        }

        [Given(@"I have a valid update hearing request")]
        public void GivenIHaveAValidUpdateHearingRequest()
        {
            var updateHearingRequest = UpdateHearingRequest.BuildRequest();
            _acTestContext.Request = _acTestContext.Put(_endpoints.UpdateHearingDetails(_acTestContext.HearingId), updateHearingRequest);
        }

        [Given(@"I have a valid get hearing by username request")]
        public void GivenIHaveAValidGetHearingByUsernameRequest()
        {
            _acTestContext.Request = _acTestContext.Get(_endpoints.GetHearingsByUsername(_acTestContext.Participants.First().Username));
        }

        [Given(@"I have a remove hearing request with a valid hearing id")]
        public void GivenIHaveARemoveHearingRequestWithAValidHearingId()
        {
            _acTestContext.Request = _acTestContext.Delete(_endpoints.RemoveHearing(_acTestContext.HearingId));
        }       

        [Then(@"hearing details should be retrieved")]
        public void ThenAHearingDetailsShouldBeRetrieved()
        {
            var model = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<HearingDetailsResponse>(_acTestContext.Json);
            model.Should().NotBeNull();
            _acTestContext.HearingId = model.Id;

            model.CaseTypeName.Should().NotBeNullOrEmpty();
            foreach (var theCase in model.Cases)
            {
                theCase.Name.Should().NotBeNullOrEmpty();
                theCase.Number.Should().NotBeNullOrEmpty();
            }
            model.HearingTypeName.Should().NotBeNullOrEmpty();
            model.HearingVenueName.Should().NotBeNullOrEmpty();
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

                if (participant.UserRoleName.Equals("Individual"))
                {
                    participant.HouseNumber.Should().NotBeNullOrEmpty();
                    participant.Street.Should().NotBeNullOrEmpty();
                    participant.City.Should().NotBeNullOrEmpty();
                    participant.County.Should().NotBeNullOrEmpty();
                    participant.Postcode.Should().NotBeNullOrEmpty();
                }
                if (participant.UserRoleName.Equals("Representative"))
                {
                    participant.HouseNumber.Should().BeNull();
                    participant.Street.Should().BeNull();
                    participant.City.Should().BeNull();
                    participant.County.Should().BeNull();
                    participant.Postcode.Should().BeNull();
                }
            }
            model.ScheduledDateTime.Should().BeAfter(DateTime.MinValue);
            model.ScheduledDuration.Should().BePositive();
            model.HearingRoomName.Should().NotBeNullOrEmpty();
            model.OtherInformation.Should().NotBeNullOrEmpty();
            model.CreatedBy.Should().NotBeNullOrEmpty();
        }

        [Then(@"hearing details should be updated")]
        public void ThenHearingDetailsShouldBeUpdated()
        {
            var model = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<HearingDetailsResponse>(_acTestContext.Json);
            model.Should().NotBeNull();
            model.ScheduledDuration.Should().Be(100);
            model.ScheduledDateTime.Should().Be(DateTime.Today.AddDays(3).AddHours(11).AddMinutes(45).ToUniversalTime());
            model.HearingVenueName.Should().Be("Manchester Civil and Family Justice Centre");
            model.OtherInformation.Should().Be("OtherInformation12345");
            model.HearingRoomName.Should().Be("HearingRoomName12345");
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

                if (participant.UserRoleName.Equals("Individual"))
                {
                    participant.HouseNumber.Should().NotBeNullOrEmpty();
                    participant.Street.Should().NotBeNullOrEmpty();
                    participant.City.Should().NotBeNullOrEmpty();
                    participant.County.Should().NotBeNullOrEmpty();
                    participant.Postcode.Should().NotBeNullOrEmpty();
                }
                if (participant.UserRoleName.Equals("Representative"))
                {
                    participant.HouseNumber.Should().BeNull();
                    participant.Street.Should().BeNull();
                    participant.City.Should().BeNull();
                    participant.County.Should().BeNull();
                    participant.Postcode.Should().BeNull();
                }
            }
        }

        [Then(@"the hearing no longer exists")]
        public void ThenTheHearingNoLongerExists()
        {
            _acTestContext.Request = _acTestContext.Get(_endpoints.GetHearingDetailsById(_acTestContext.HearingId));
            _acTestContext.Response = _acTestContext.Client().Execute(_acTestContext.Request);
            _acTestContext.Response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Given(@"I have a valid book a new hearing for a case type")]
        public void GivenIHaveAValidBookANewHearingForAcaseType()
        {
            var bookNewHearingRequest = CreateHearingRequest.BuildRequest();
            bookNewHearingRequest.ScheduledDateTime = DateTime.Now.AddDays(2);
            _acTestContext.Request = _acTestContext.Post(_endpoints.BookNewHearing(), bookNewHearingRequest);
            _acTestContext.Response = _acTestContext.Client().Execute(_acTestContext.Request);
            _acTestContext.Json = _acTestContext.Response.Content;
            _acTestContext.Response.StatusCode.Should().Be(HttpStatusCode.Created);
            var model = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<HearingDetailsResponse>(_acTestContext.Json);
            model.Should().NotBeNull();
            _acTestContext.HearingId = model.Id;
        }

        [Given(@"I have a get details for a given hearing request with a valid case type")]
        public void GivenIHaveAGetDetailsForAGivenHearingRequestWithAValidCaseType()
        {
            _acTestContext.Request = _acTestContext.Get(_endpoints.GetHearingsByAnyCaseType());
        }

        [Then(@"hearing details should be retrieved for the case type")]
        public void ThenHearingDetailsShouldBeRetrievedForTheCaseType()
        {
            var model = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<BookingsResponse>(_acTestContext.Json);
            model.PrevPageUrl.Should().Contain(model.Limit.ToString());
            var hearing = model.Hearings.Where(u => u.ScheduledDate.Date == DateTime.UtcNow.AddDays(2).Date);
            var response = hearing.Hearings.Single(u => u.HearingId == _acTestContext.HearingId);
            response.CaseTypeName.Should().NotBeNullOrEmpty();
            response.HearingTypeName.Should().NotBeNullOrEmpty();
            response.ScheduledDateTime.Should().BeAfter(DateTime.UtcNow);
            response.ScheduledDuration.Should().NotBe(0);
            response.JudgeName.Should().NotBeNullOrEmpty();
            response.CourtAddress.Should().NotBeNullOrEmpty();
            response.HearingName.Should().NotBeNullOrEmpty();
            response.HearingNumber.Should().NotBeNullOrEmpty();
        }

        [Given(@"I have a cancel hearing request with a valid hearing id")]
        public void GivenIHaveACancelHearingRequestWithAValidHearingId()
        {
            var updateHearingStatusRequest = UpdateBookingStatusRequest.BuildRequest();
            _acTestContext.Request = _acTestContext.Patch(_endpoints.UpdateHearingDetails(_acTestContext.HearingId), updateHearingStatusRequest);
        }

        [Then(@"hearing should be cancelled")]
        public void ThenHearingShouldBeCancelled()
        {
            _acTestContext.Request = _acTestContext.Get(_endpoints.GetHearingDetailsById(_acTestContext.HearingId));
            _acTestContext.Response = _acTestContext.Client().Execute(_acTestContext.Request);
            var model = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<HearingDetailsResponse>(_acTestContext.Response.Content);
            model.UpdatedBy.Should().NotBeNullOrEmpty();
            model.Status.Should().Be(Domain.Enumerations.BookingStatus.Cancelled);
        }

        [Then(@"a list of hearing details should be retrieved")]
        public void ThenAListOfHearingDetailsShouldBeRetrieved()
        {
            var model = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<List<HearingDetailsResponse>>(_acTestContext.Json);
            model.Should().NotBeNull();
            _acTestContext.HearingId = model.First().Id;

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

                    if (participant.UserRoleName.Equals("Individual"))
                    {
                        participant.HouseNumber.Should().NotBeNullOrEmpty();
                        participant.Street.Should().NotBeNullOrEmpty();
                        participant.City.Should().NotBeNullOrEmpty();
                        participant.County.Should().NotBeNullOrEmpty();
                        participant.Postcode.Should().NotBeNullOrEmpty();
                    }

                    if (!participant.UserRoleName.Equals("Representative")) continue;
                    participant.HouseNumber.Should().BeNull();
                    participant.Street.Should().BeNull();
                    participant.City.Should().BeNull();
                    participant.County.Should().BeNull();
                    participant.Postcode.Should().BeNull();
                }
                hearing.ScheduledDateTime.Should().BeAfter(DateTime.MinValue);
                hearing.ScheduledDuration.Should().BePositive();
                hearing.HearingRoomName.Should().NotBeNullOrEmpty();
                hearing.OtherInformation.Should().NotBeNullOrEmpty();
                hearing.CreatedBy.Should().NotBeNullOrEmpty();
            }          
        }
    }
}