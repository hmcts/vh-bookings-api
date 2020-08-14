using System;
using Bookings.Common.Services;
using FluentAssertions;
using NUnit.Framework;

namespace Bookings.UnitTests.Services
{
    [TestFixture]
    public class ClockTests
    {
        [Test]
        public void Should_return_date_time()
        {
            var clock = new Clock();

            clock.UtcNow.Should().NotBe(DateTime.MinValue);
            clock.UtcNow.Should().NotBe(DateTime.MaxValue);
        }    
    }
}