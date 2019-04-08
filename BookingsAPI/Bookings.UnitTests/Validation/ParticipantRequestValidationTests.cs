using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bookings.Api.Contract.Requests;
using Bookings.API.Validations;
using FizzWare.NBuilder;
using FluentAssertions;
using NUnit.Framework;

namespace Bookings.UnitTests.Validation
{
    public class ParticipantRequestValidationTests
    {
        private ParticipantRequestValidation _validator;
        private AddressValidation _addressValidator;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _validator = new ParticipantRequestValidation();
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
        public async Task should_return_missing_case_role_name_error()
        {
            var request = BuildRequest();
            request.CaseRoleName = string.Empty;

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == ParticipantRequestValidation.NoCaseRoleNameErrorMessage)
                .Should().BeTrue();
        }

        [Test]
        public async Task should_return_missing_hearing_role_name_error()
        {
            var request = BuildRequest();
            request.HearingRoleName = string.Empty;

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == ParticipantRequestValidation.NoHearingRoleNameErrorMessage)
                .Should().BeTrue();
        }

        [Test]
        public async Task should_return_missing_title_error()
        {
            var request = BuildRequest();
            request.Title = string.Empty;

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == ParticipantRequestValidation.NoTitleErrorMessage)
                .Should().BeTrue();
        }

        [Test]
        public async Task should_return_missing_first_name_error()
        {
            var request = BuildRequest();
            request.FirstName = string.Empty;

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == ParticipantRequestValidation.NoFirstNameErrorMessage)
                .Should().BeTrue();
        }

        [Test]
        public async Task should_return_missing_last_name_error()
        {
            var request = BuildRequest();
            request.LastName = string.Empty;

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == ParticipantRequestValidation.NoLastNameErrorMessage)
                .Should().BeTrue();
        }

        [Test]
        public async Task should_return_missing_username_error()
        {
            var request = BuildRequest();
            request.Username = string.Empty;

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == ParticipantRequestValidation.NoUsernameErrorMessage)
                .Should().BeTrue();
        }

        [Test]
        public async Task should_return_missing_contact_email_error()
        {
            var request = BuildRequest();
            request.ContactEmail = string.Empty;

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == ParticipantRequestValidation.NoContactEmailErrorMessage)
                .Should().BeTrue();
        }
        [Test]
        public async Task should_return_missing_housenumber_error()
        {
            var request = BuildRequest();
            request.HearingRoleName = "Claimant LIP";
            request.HouseNumber = string.Empty;

            var result = await _addressValidator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == AddressValidation.NoHouseNumberErrorMessage)
                .Should().BeTrue();
        }
        [Test]
        public async Task should_return_missing_street_error()
        {
            var request = BuildRequest();
            request.HearingRoleName = "Defendant LIP";
            request.Street = string.Empty;

            var result = await _addressValidator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == AddressValidation.NoStreetErrorMessage)
                .Should().BeTrue();
        }
        [Test]
        public async Task should_return_missing_city_error()
        {
            var request = BuildRequest();
            request.HearingRoleName = "Applicant LIP";
            request.City = string.Empty;

            var result = await _addressValidator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == AddressValidation.NoCityErrorMessage)
                .Should().BeTrue();
        }
        [Test]
        public async Task should_return_missing_county_error()
        {
            var request = BuildRequest();
            request.HearingRoleName = "Respondent LIP";
            request.County = string.Empty;

            var result = await _addressValidator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == AddressValidation.NoCountyErrorMessage)
                .Should().BeTrue();
        }
        [Test]
        public async Task should_return_missing_postcode_error()
        {
            var request = BuildRequest();
            request.HearingRoleName = "Claimant LIP";
            request.Postcode = string.Empty;

            var result = await _addressValidator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == AddressValidation.NoPostcodeErrorMessage)
                .Should().BeTrue();
        }

        [Test]
        public async Task should_pass_validation_for_representative()
        {
            var request = BuildRequest();
            request.HearingRoleName = "Solicitor";
            request.HouseNumber = string.Empty;
            request.Street = string.Empty;
            request.City = string.Empty;
            request.County = string.Empty;
            request.Postcode = string.Empty;

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeTrue();
            result.Errors.Count.Should().Be(0);
        }

        private ParticipantRequest BuildRequest()
        {
            return Builder<ParticipantRequest>.CreateNew()
                 .With(x => x.CaseRoleName = "Claimant")
                 .With(x => x.HearingRoleName = "Solicitor")
                 .Build();
        }
    }
}