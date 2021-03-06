﻿using AcceptanceTests.Common.Api.Helpers;
using BookingsApi.AcceptanceTests.Contexts;
using BookingsApi.Contract.Responses;
using FluentAssertions;
using TechTalk.SpecFlow;
using static Testing.Common.Builders.Api.ApiUriFactory;

namespace BookingsApi.AcceptanceTests.Steps
{
    [Binding]
    public sealed class HealthCheckSteps
    {
        private readonly TestContext _context;

        public HealthCheckSteps(TestContext context)
        {
            _context = context;
        }

        [Given(@"I have a get health request")]
        public void GivenIHaveAGetHealthRequest()
        {
            _context.Request = _context.Get(HealthCheckEndpoints.HealthCheck);
        }

        [Then(@"the application version should be retrieved")]
        public void ThenTheApplicationVersionShouldBeRetrieved()
        {
            var model = RequestHelper.Deserialise<BookingsApiHealthResponse>(_context.Response.Content);
            model.Should().NotBeNull();
            model.AppVersion.Should().NotBeNull();
            model.AppVersion.FileVersion.Should().NotBeNull();
            model.AppVersion.InformationVersion.Should().NotBeNull();
        }
    }
}
