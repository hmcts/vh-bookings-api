using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bookings.Api.Contract.Requests;
using Bookings.API.Validations;
using FizzWare.NBuilder;
using FluentAssertions;
using FluentValidation;
using NUnit.Framework;

namespace Bookings.UnitTests.Validation
{
    public class UpdateParticipantRequestValidationTests
    {
        private UpdateParticipantRequestValidation _validator;
        private AddressValidation _addressValidator;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _validator = new UpdateParticipantRequestValidation();
            _addressValidator = new AddressValidation();
        }
        
        [Test]
        public async Task should_pass_validation()
        {
            var request = BuildRequest();

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeTrue();
        }
        
        [Test]
        public async Task should_return_missing_display_name_error()
        {
            var request = BuildRequest();
            request.DisplayName = string.Empty;

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == ParticipantRequestValidation.NoDisplayNameErrorMessage)
                .Should().BeTrue();
        }

        [Test]
        public async Task should_return_missing_address_fields_error()
        {
            var addressErrorMessages = new List<string>() { AddressValidation.NoHouseNumberErrorMessage, AddressValidation.NoStreetErrorMessage, AddressValidation.NoCityErrorMessage, AddressValidation.NoCountyErrorMessage, AddressValidation.NoPostcodeErrorMessage };


            var request = BuildRequest();
            request.HearingRoleName = "Claimant LIP";
            request.HouseNumber = string.Empty;
            request.Street = string.Empty;
            request.City = string.Empty;
            request.County = string.Empty;
            request.Postcode = string.Empty;

            var result = await _addressValidator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(5);
            result.Errors.All(x => addressErrorMessages.Contains(x.ErrorMessage))
                .Should().BeTrue();
        }


        private UpdateParticipantRequest BuildRequest()
        {
           return Builder<UpdateParticipantRequest>.CreateNew()
                .Build();
        }
    }
}