using System;
using System.Collections.Generic;
using System.Net;
using Bookings.AcceptanceTests.Contexts;
using Bookings.AcceptanceTests.Models;
using Bookings.Api.Contract.Responses;
using Bookings.Domain.Participants;
using FluentAssertions;
using TechTalk.SpecFlow;
using Testing.Common.Builders.Api;

namespace Bookings.AcceptanceTests.Steps
{
    [Binding]
    public sealed class CommonSteps : StepsBase
    {
        private readonly ScenarioContext context;
        private readonly AcTestContext _acTestContext;
        private readonly HearingsEndpoints _endpoints = new ApiUriFactory().HearingsEndpoints;

        public CommonSteps(ScenarioContext injectedContext, AcTestContext acTestContext)
        {
            context = injectedContext;
            _acTestContext = acTestContext;
        }

        [Given(@"I have a hearing")]
        public void GivenIHaveAHearing()
        {
            var bookNewHearingRequest = CreateHearingRequest.BuildRequest();
            _acTestContext.Request = _acTestContext.Post(_endpoints.BookNewHearing(), bookNewHearingRequest);
            _acTestContext.Response = _acTestContext.Client().Execute(_acTestContext.Request);
            _acTestContext.Json = _acTestContext.Response.Content;
            _acTestContext.Response.StatusCode.Should().Be(HttpStatusCode.Created);
            var model = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<HearingDetailsResponse>(_acTestContext.Json);
            model.Should().NotBeNull();
            _acTestContext.HearingId = model.Id;
            _acTestContext.Participants = new List<ParticipantResponse>(model.Participants);           
            model.ScheduledDuration.Should().NotBe(100);
            model.ScheduledDateTime.Should().NotBe(DateTime.Today.AddDays(3).AddHours(11).AddMinutes(45));
            model.HearingVenueName.Should().NotBe("Manchester Civil and Family Justice Centre");
        }

        [When(@"I send the request to the endpoint")]
        public void WhenISendTheRequestToTheEndpoint()
        {
            _acTestContext.Response = _acTestContext.Client().Execute(_acTestContext.Request);
            if (_acTestContext.Response.Content != null)
                _acTestContext.Json = _acTestContext.Response.Content;
        }

        [Then(@"the response should have the status (.*) and success status (.*)")]
        public void ThenTheResponseShouldHaveTheStatusAndSuccessStatus(HttpStatusCode httpStatusCode, bool isSuccess)
        {
            _acTestContext.Response.StatusCode.Should().Be(httpStatusCode);
            _acTestContext.Response.IsSuccessful.Should().Be(isSuccess);
        }
    }
}
