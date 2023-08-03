using System.Linq;
using System.Threading.Tasks;
using BookingsApi.Contract.V2.Requests;
using BookingsApi.Validations.V2;
using FluentAssertions;
using NUnit.Framework;

namespace BookingsApi.UnitTests.Validation.V2
{
    public class CaseRequestValidationV2Tests
    {
        private CaseRequestValidationV2 _validator;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _validator = new CaseRequestValidationV2();
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
            result.Errors.Any(x => x.ErrorMessage == CaseRequestValidationV2.CaseNameMessage)
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
            result.Errors.Any(x => x.ErrorMessage == CaseRequestValidationV2.CaseNumberMessage)
                .Should().BeTrue();
        }

        private CaseRequestV2 BuildRequest()
        {
            return new CaseRequestV2
            {
                Name = "A vs B",
                Number = "1234567890",
                IsLeadCase = true
            };
        }
    }
}