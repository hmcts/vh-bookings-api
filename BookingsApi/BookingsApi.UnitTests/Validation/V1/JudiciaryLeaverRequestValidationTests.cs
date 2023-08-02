using System;
using BookingsApi.Contract.V1.Requests;
using BookingsApi.Validations.V1;
using FluentAssertions;
using NUnit.Framework;

namespace BookingsApi.UnitTests.Validation.V1
{
    public class JudiciaryLeaverRequestValidationTests
    {
        private JudiciaryLeaverRequestValidation _validator;

        [OneTimeSetUp]
        public void SetUp()
        {
            _validator = new JudiciaryLeaverRequestValidation();
        }

        [Test]
        public void Id_should_not_be_null_or_empty()
        {
            var req = new JudiciaryLeaverRequest
            {
                Leaver = true,
                LeftOn = DateTime.Now.AddDays(-100).ToLongDateString()
            };

            var result = _validator.Validate(req);

            result.IsValid.Should().BeFalse();
        }

        [Test]
        public void Leaver_should_not_be_null_or_empty()
        {
            var req = new JudiciaryLeaverRequest
            {
                Id = Guid.NewGuid().ToString(),
                LeftOn = DateTime.Now.AddDays(-100).ToLongDateString()
            };

            var result = _validator.Validate(req);

            result.IsValid.Should().BeFalse();
        }
    }
}
