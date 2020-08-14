using System;
using Bookings.Common.Services;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Bookings.UnitTests.Services
{
    [TestFixture]
    public class RandomGeneratorTests
    {
        private Mock<IClock> _clock;

        private RandomGenerator _randomGenerator;

        public RandomGeneratorTests()
        {
            _clock = new Mock<IClock>();
            
            _randomGenerator = new RandomGenerator(_clock.Object);
        }

        [Test]
        public void Should_throw_exception_on_skip_greater_than_ticks_length()
        {
            _clock.Setup(x => x.UtcNow).Returns(DateTime.UtcNow);

            Assert.Throws<ArgumentOutOfRangeException>(() => _randomGenerator.GetRandomFromTicks(999, 999));
        }

        [Test]
        public void Should_throw_exception_on_taking_more_than_available()
        {
            _clock.Setup(x => x.UtcNow).Returns(DateTime.UtcNow);

            Assert.Throws<ArgumentOutOfRangeException>(() => _randomGenerator.GetRandomFromTicks(2, 999));
        }

        [TestCase("637330171125319309", 0u, 18u, "637330171125319309")]
        [TestCase("637330171125319309", 0u, 17u, "63733017112531930")]
        [TestCase("637330171125319309", 3u, 10u, "3301711253")]
        [TestCase("637330171125319309", 13u, 4u, "1930")]
        public void Should_return_correct_ticks(string source, uint skip, uint take, string expected)
        {
            _clock.Setup(x => x.UtcNow).Returns(new DateTime(long.Parse(source)));

            var result = _randomGenerator.GetRandomFromTicks(skip, take);

            result.Should().NotBeNullOrWhiteSpace();
            result.Should().Be(expected);
        }
    }
}