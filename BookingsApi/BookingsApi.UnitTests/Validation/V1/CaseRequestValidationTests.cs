using System.Linq;
using System.Threading.Tasks;
using BookingsApi.Contract.V1.Requests;
using BookingsApi.Validations.V1;
using FluentAssertions;
using NUnit.Framework;

namespace BookingsApi.UnitTests.Validation.V1
{
    public class CaseRequestValidationTests
    {
        private CaseRequestValidation _validator;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _validator = new CaseRequestValidation();
        }

        [Test]
        public async Task Should_pass_validation()
        {
            var request = BuildRequest();
            
            var result = await _validator.ValidateAsync(request);
            
            result.IsValid.Should().BeTrue();
        }
        
        [Test]
        public async Task Should_return_missing_case_name_error()
        {
            var request = BuildRequest();
            request.Name = string.Empty;
            
            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == CaseRequestValidation.CaseNameMessage)
                .Should().BeTrue();
        }
        
        [Test]
        public async Task Should_return_missing_case_number_error()
        {
            var request = BuildRequest();
            request.Number = string.Empty;
            
            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == CaseRequestValidation.CaseNumberMessage)
                .Should().BeTrue();
        }

        private CaseRequest BuildRequest()
        {
            return new CaseRequest
            {
                Name = "A vs B",
                Number = "1234567890",
                IsLeadCase = true
            };
        }
    }
}