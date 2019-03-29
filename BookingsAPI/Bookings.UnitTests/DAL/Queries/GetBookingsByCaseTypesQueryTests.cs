using System;
using Bookings.DAL.Queries;
using Bookings.UnitTests.Utilities;
using FluentAssertions;
using NUnit.Framework;
// ReSharper disable ObjectCreationAsStatement

namespace Bookings.UnitTests.DAL.Queries
{
    public class GetBookingsByCaseTypesQueryTests : TestBase
    {
        [Test]
        public void should_throw_exception_if_setting_an_invalid_limit()
        {
            When(() => new GetBookingsByCaseTypesQuery {Limit = 0})
                .Should().Throw<ArgumentException>();
            
            When(() => new GetBookingsByCaseTypesQuery {Limit = -1})
                .Should().Throw<ArgumentException>();
        }
    }
}