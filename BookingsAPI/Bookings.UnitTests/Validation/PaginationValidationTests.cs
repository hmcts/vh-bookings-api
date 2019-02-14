using FluentAssertions;
using Bookings.Api.Contract.Requests;
using Bookings.API.Validations;
using NUnit.Framework;

namespace Bookings.UnitTests.Validation
{
    public class PaginationValidationTests
    {
        private PaginationValidator _validator;

        [SetUp]
        public void Setup()
        {
            _validator = new PaginationValidator(500);
        }
        
        [Test]
        public void should_return_successful_validation()
        {
            var result = _validator.Validate(new PaginatedRequest(1, 5));
            result.IsValid.Should().BeTrue();
        }
        
        [Test]
        public void should_fail_if_page_is_below_one()
        {
            var result = _validator.Validate(new PaginatedRequest(0, 5));
            result.IsValid.Should().BeFalse();
        }
        
        [Test]
        public void should_fail_if_page_size_is_below_one()
        {
            var result = _validator.Validate(new PaginatedRequest(1, 0));
            result.IsValid.Should().BeFalse();
        }

        [Test]
        public void should_fail_if_page_size_is_above_max()
        {
            var result = _validator.Validate(new PaginatedRequest(1, 600));
            result.IsValid.Should().BeFalse();
        }
    }
}