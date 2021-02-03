using BookingsApi.Domain;
using FluentAssertions;
using NUnit.Framework;
using System;

namespace BookingsApi.UnitTests.Domain.EndPoints
{
    public class UpdateDisplayNameTests
    {
        private Endpoint _endpoint;

        [SetUp]
        public void Initialise()
        {
            _endpoint = new Endpoint("Original DisplayName", "sip@videohearings.net", "1234", null);
        }

        [Test]
        public void Should_update_display_name()
        {
            var newDisplayName = "New DisplayName";
            _endpoint.UpdateDisplayName(newDisplayName);
            _endpoint.DisplayName.Should().Be(newDisplayName);
        }

        [Test]
        public void Should_throw_argument_null_exception_when_display_name_field_is_null()
        {
            Action action = () => _endpoint.UpdateDisplayName(string.Empty);
            action.Should().Throw<ArgumentNullException>();
            _endpoint.DisplayName.Should().Be("Original DisplayName");
        }
    }
}
