using BookingsApi.Contract.Requests.Enums;
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
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == ParticipantRequestValidation.NoContactEmailErrorMessage)
                .Should().BeTrue();
        }
        private ParticipantRequest BuildRequest()
        {
            return Builder<ParticipantRequest>.CreateNew()
                 .With(x => x.CaseRoleName = "Applicant")
                 .With(x => x.HearingRoleName = HearingRoleName.Representative)
                 .Build();
        }

        [Test]
        public async Task Should_return_missing_telephone_error()
        {
            var request = BuildStaffMemberRequest();
            request.TelephoneNumber = string.Empty;

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == ParticipantRequestValidation.NoTelephoneErrorMessage)
                .Should().BeTrue();
        }

        private ParticipantRequest BuildStaffMemberRequest()
        {
            return Builder<ParticipantRequest>.CreateNew()
                 .With(x => x.CaseRoleName = "Staff Member")
                 .With(x => x.HearingRoleName = HearingRoleName.StaffMember)
                 .Build();
        }
    }
}