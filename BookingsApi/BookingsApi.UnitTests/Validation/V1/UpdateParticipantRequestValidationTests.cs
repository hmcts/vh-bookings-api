using BookingsApi.Contract.V1.Requests;
using BookingsApi.Validations.V1;
using FizzWare.NBuilder;

namespace BookingsApi.UnitTests.Validation.V1
{
    public class UpdateParticipantRequestValidationTests
    {
        private UpdateParticipantRequestValidation _validator;
        
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _validator = new UpdateParticipantRequestValidation();
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
        public async Task Should_not_validate_contact_email_when_empty()
        {
            var request = BuildRequest();
            request.ContactEmail = "";

            var result = await _validator.ValidateAsync(request);
            
            result.IsValid.Should().BeTrue();
        }
        
        [Test]
        public async Task Should_return_invalid_contact_email_error()
        {
            var request = BuildRequest();
            request.ContactEmail = "gsdgdsgfs";

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Exists(x => x.ErrorMessage == ParticipantRequestValidation.InvalidContactEmailErrorMessage)
                .Should().BeTrue();
        }

        private static UpdateParticipantRequest BuildRequest()
        {
            return Builder<UpdateParticipantRequest>.CreateNew()
                .With(x => x.ContactEmail = "")
                .Build();
        }
    }
}