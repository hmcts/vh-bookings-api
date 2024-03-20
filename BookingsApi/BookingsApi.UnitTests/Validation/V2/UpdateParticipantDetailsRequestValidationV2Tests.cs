using BookingsApi.Contract.V2.Requests;
using BookingsApi.Validations.V2;
using FizzWare.NBuilder;

namespace BookingsApi.UnitTests.Validation.V2
{
    public class UpdateParticipantDetailsRequestValidationV2Tests
    {
        private UpdateParticipantRequestValidationV2 _validator;
        
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _validator = new UpdateParticipantRequestValidationV2();
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
            result.Errors.Exists(x => x.ErrorMessage == ParticipantRequestValidationV2.InvalidContactEmailErrorMessage)
                .Should().BeTrue();
        }
        
        private UpdateParticipantRequestV2 BuildRequest()
        {
            return Builder<UpdateParticipantRequestV2>.CreateNew()
                .With(x => x.ContactEmail = "")
                .Build();
        }
    }
}
