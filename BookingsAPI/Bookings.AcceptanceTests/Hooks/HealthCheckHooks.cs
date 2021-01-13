﻿using AcceptanceTests.Common.Api;
using Bookings.AcceptanceTests.Contexts;
using System.Net;
using FluentAssertions;
using TechTalk.SpecFlow;
using static Testing.Common.Builders.Api.ApiUriFactory;

namespace Bookings.AcceptanceTests.Hooks
{
    [Binding]
    public static class HealthCheckHooks
    {
        [BeforeScenario(Order = (int)HooksSequence.HealthCheckHooks)]
        public static void CheckApiHealth(TestContext context)
        {
            context.Request = context.Get(HealthCheckEndpoints.HealthCheck);
            var response = context.Client().Execute(context.Request);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.IsSuccessful.Should().BeTrue();
        }
    }
}
