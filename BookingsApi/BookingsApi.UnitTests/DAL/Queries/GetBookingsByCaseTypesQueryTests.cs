using System;
using BookingsApi.DAL.Queries;
using BookingsApi.UnitTests.Utilities;

namespace BookingsApi.UnitTests.DAL.Queries
{
    public class GetBookingsByCaseTypesQueryTests : TestBase
    {   
        [Test]
        public void Should_throw_exception_if_setting_an_invalid_limit()
        {
            When(() => new GetBookingsByCaseTypesQuery {Limit = 0})
                .Should().Throw<ArgumentException>();
            
            When(() => new GetBookingsByCaseTypesQuery {Limit = -1})
                .Should().Throw<ArgumentException>();
        }
    }
}