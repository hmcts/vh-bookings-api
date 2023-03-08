using System;
using BookingsApi.Common.Services;
using FluentAssertions;
using NUnit.Framework;

namespace BookingsApi.UnitTests.Services
{
    [TestFixture]
    public class RandomGeneratorTests
    {
        private readonly RandomGenerator _randomGenerator;

        public RandomGeneratorTests()
        {
            _randomGenerator = new RandomGenerator();
        }
        
        [Test]
        public void Should_throw_exception_on_skip_greater_than_ticks_length()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => 
                _randomGenerator.GetWeakDeterministic(DateTime.UtcNow.Ticks, 999, 999));
        }

        [Test]
        public void Should_throw_exception_on_taking_more_than_available()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => 
                _randomGenerator.GetWeakDeterministic(DateTime.UtcNow.Ticks, 2, 999));
        }

        [TestCase("637330171125319309", 0u, 18u, "903913521171033736")]
        [TestCase("637330171125319309", 0u, 17u, "90391352117103373")]
        [TestCase("637330171125319309", 3u, 10u, "9135211710")]
        [TestCase("637030171125319309", 3u, 10u, "9135211710")]
        [TestCase("637330171125319309", 13u, 4u, "3373")]
        [TestCase("607330171125309309", 13u, 4u, "3370")]
        public void Should_return_correct_ticks(string source, uint skip, uint take, string expected)
        {
            var result = _randomGenerator.GetWeakDeterministic(long.Parse(source), skip, take);

            result.Should().NotBeNullOrWhiteSpace();
            result.Should().Be(expected);
        }
    }
}