using System;
using System.Net;
using Bookings.AcceptanceTests.Contexts;
using Bookings.AcceptanceTests.Helpers;
using Bookings.API;
using Bookings.Common.Configuration;
using Bookings.Common.Security;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using TechTalk.SpecFlow;
using Testing.Common.Builders.Api;
using Testing.Common.Configuration;

namespace Bookings.AcceptanceTests.Steps
{
    [Binding]
    public abstract class BaseSteps
    {
        protected BaseSteps()
        {
        }
    }
}