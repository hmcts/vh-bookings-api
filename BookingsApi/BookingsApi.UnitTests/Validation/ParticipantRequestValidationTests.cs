using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.Contract.Requests;
using BookingsApi.Validations;
using FizzWare.NBuilder;
using FluentAssertions;
using NUnit.Framework;

namespace BookingsApi.UnitTests.Validation
{
    public class ParticipantRequestValidationTests
    {
        private ParticipantRequestValidation _validator;
        
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _validator = new ParticipantRequestValidation();
        }


        [Test]
        public async Task Should_pass_validation()
        {
            var request = BuildRequest();

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeTrue();
        }

        [Test]
        public async Task Should_return_missing_display_name_error()
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
        public async Task Should_return_missing_case_role_name_error()
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
        public async Task Should_return_missing_hearing_role_name_error()
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
        public async Task Should_return_missing_first_name_error()
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
        public async Task Should_return_missing_last_name_error()
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
        public async Task Should_return_missing_username_error()
        {
            var request = BuildRequest();
            request.HearingRoleName = "Judge";
            request.ContactEmail = "test@T.com";
            request.Username = string.Empty;

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == ParticipantRequestValidation.NoUsernameErrorMessage)
                .Should().BeTrue();
        }

        [Test]
        public async Task Should_return_missing_contact_email_error()
        {
            var request = BuildRequest();
            request.ContactEmail = string.Empty;

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(2);
            result.Errors.Any(x => x.ErrorMessage == ParticipantRequestValidation.NoContactEmailErrorMessage)
                .Should().BeTrue();
        }

        [Test]
        public async Task Should_return_invalid_contact_email_error()
        {
            var request = BuildRequest();
            request.ContactEmail = "gsdgdsgfs";

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == ParticipantRequestValidation.InvalidContactEmailErrorMessage)
                .Should().BeTrue();
        }

        [Test]
        public async Task Should_return_invalid_judge_username_error()
        {
            var request = BuildRequest();
            request.HearingRoleName = "Judge";
            request.Username = "gsdgdsgfs";

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == ParticipantRequestValidation.InvalidJudgeUsernameErrorMessage)
                .Should().BeTrue();
        }

        [Test]
        public async Task Should_return_valid_judge_username()
        {
            var request = BuildRequest();
            request.HearingRoleName = "Judge";
            request.Username = "judge.one@ejudiciary.net";

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeTrue();
        }

        [Test]
        public async Task Should_return_valid_contact_email_error()
        {
            var request = BuildRequest();
            request.ContactEmail = "mmm@mm.com";

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeTrue();
        }
        
        [Test]
        public async Task Should_return_missing_telephone_number_error()
        {
            var request = BuildRequest();
            request.TelephoneNumber = string.Empty;

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == ParticipantRequestValidation.NoTelephoneNumberErrorMessage)
                .Should().BeTrue();
        }
        
        [Test]
        public async Task Should_return_invalid_telephone_number_error()
        {
            var request = BuildRequest();
            request.TelephoneNumber = "a";

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == ParticipantRequestValidation.InvalidTelephoneNumberErrorMessage)
                .Should().BeTrue();
        }

        [Test]
        public async Task Should_return_valid_telephone_number()
        {
            var request = BuildRequest();
            request.TelephoneNumber = "(+44)123 456";

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeTrue();
        }

        private ParticipantRequest BuildRequest()
        {
            return Builder<ParticipantRequest>.CreateNew()
                 .With(x => x.CaseRoleName = "Applicant")
                 .With(x => x.HearingRoleName = "Representative")
                 .With(x => x.ContactEmail = "mm@mm.com")
                 .With(x => x.TelephoneNumber = "0123")
                 .Build();
        }
    }
}