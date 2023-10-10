using BookingsApi.Contract.V2.Requests;
using BookingsApi.Domain.Constants;
using BookingsApi.Validations.V2;
using FizzWare.NBuilder;

namespace BookingsApi.UnitTests.Validation.V2
{
    public class ParticipantRequestValidationV2Tests
    {
        private ParticipantRequestValidationV2 _validator;
        
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _validator = new ParticipantRequestValidationV2();
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
            result.Errors.Count.Should().Be(2);
            result.Errors.Exists(x => x.ErrorMessage == ParticipantRequestValidationV2.NoDisplayNameErrorMessage)
                .Should().BeTrue();
            result.Errors.Exists(x => x.ErrorMessage == ParticipantRequestValidationV2.InvalidDisplayNameErrorMessage)
                .Should().BeTrue();
        }

        [Test]
        public async Task Should_return_missing_hearing_role_code_error()
        {
            var request = BuildRequest();
            request.HearingRoleCode = string.Empty;

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Exists(x => x.ErrorMessage == ParticipantRequestValidationV2.NoHearingRoleCodeErrorMessage)
                .Should().BeTrue();
        }

        [Test]
        public async Task Should_return_missing_first_name_error()
        {
            var request = BuildRequest();
            request.FirstName = string.Empty;

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(2);
            result.Errors.Exists(x => x.ErrorMessage == ParticipantValidationV2.NoFirstNameErrorMessage)
                .Should().BeTrue();
        }

        [Test]
        public async Task Should_return_missing_last_name_error()
        {
            var request = BuildRequest();
            request.LastName = string.Empty;

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(2);
            result.Errors.Exists(x => x.ErrorMessage == ParticipantValidationV2.NoLastNameErrorMessage)
                .Should().BeTrue();
        }

        [Test]
        public async Task Should_return_missing_username_error()
        {
            var request = BuildRequest();
            request.HearingRoleCode = HearingRoleCodes.Judge;
            request.ContactEmail = "test@T.com";

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Exists(x => x.ErrorMessage == ParticipantRequestValidationV2.NoUsernameErrorMessage)
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
            result.Errors.Exists(x => x.ErrorMessage == ParticipantRequestValidationV2.NoContactEmailErrorMessage)
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
            result.Errors.Exists(x => x.ErrorMessage == ParticipantRequestValidationV2.InvalidContactEmailErrorMessage)
                .Should().BeTrue();
        }

        [Test]
        public async Task Should_return_invalid_judge_username_error()
        {
            var request = BuildRequest();
            request.HearingRoleCode = HearingRoleCodes.Judge;

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Exists(x => x.ErrorMessage == ParticipantRequestValidationV2.InvalidJudgeUsernameErrorMessage)
                .Should().BeTrue();
        }

        [Test]
        public async Task Should_return_valid_judge_username()
        {
            var request = BuildRequest();
            request.HearingRoleCode = HearingRoleCodes.Judge;

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
        public async Task Should_return_missing_telephone_number_error_for_non_judge()
        {
            var request = BuildRequest();
            request.TelephoneNumber = string.Empty;
            request.HearingRoleCode = "RPTT";

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Exists(x => x.ErrorMessage == ParticipantRequestValidationV2.NoTelephoneNumberErrorMessage)
                .Should().BeTrue();
        }

        [Test]
        public async Task Should_not_return_missing_telephone_number_error_for_judge()
        {
            var request = BuildRequest();
            request.TelephoneNumber = string.Empty;
            request.HearingRoleCode = HearingRoleCodes.Judge;
            
            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeTrue();
        }
        [TestCase("wil.li_am." , false)]
        [TestCase("Cr.aig_1234", true)]
        [TestCase("I.", false)]
        [TestCase(".william1234", false)]
        [TestCase("_a", true)]
        [TestCase("Willi..amCraig1234", false)]
        [TestCase(" qweqwe ", false)]
        [TestCase("w.w", true)]
        [TestCase("XY", true)]
        [TestCase("bill e boy", true)]
        [TestCase("Test-Judge 1", true)]
        [TestCase("Test-Judge  1", false)]
        public async Task Should_valid_first_last_names_against_regex(string testName, bool expectedResult)
        {
            //Arrange
            var request = BuildRequest();
            request.FirstName = testName;
            request.LastName = testName;
            //Act
            var result = await _validator.ValidateAsync(request);
            //Assert
            result.IsValid.Should().Be(expectedResult);
            if (!expectedResult)
            {
                result.Errors.Count.Should().Be(2);
                result.Errors.Exists(x => x.ErrorMessage == ParticipantValidationV2.FirstNameDoesntMatchRegex)
                    .Should().BeTrue();
                result.Errors.Exists(x => x.ErrorMessage == ParticipantValidationV2.LastNameDoesntMatchRegex)
                    .Should().BeTrue();
            }
        }

        private static ParticipantRequestV2 BuildRequest()
        {
            return Builder<ParticipantRequestV2>.CreateNew()
                 .With(x => x.HearingRoleCode = "RPTT")
                 .With(x => x.TelephoneNumber = "020 7946 0101")
                 .With(x => x.ContactEmail = "mm@mm.com")
                 .Build();
        }
    }
}