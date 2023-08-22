using BookingsApi.Contract.V1.Requests;
using BookingsApi.Validations.V1;
using FizzWare.NBuilder;

namespace BookingsApi.UnitTests.Validation.V1
{
    public class UpdateHearingParticipantsRequestValidationTests
    {
        private UpdateHearingParticipantsRequestValidation _validator;
        
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _validator = new UpdateHearingParticipantsRequestValidation();
        }

        [Test]
        public async Task Should_pass_validation()
        {
            var request = BuildRequest();
            request.RemovedParticipantIds.Add(Guid.NewGuid());

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeTrue();
        }

        [Test]
        public async Task Should_return_no_participants_error()
        {
            var request = BuildRequest();

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == UpdateHearingParticipantsRequestValidation.NoParticipantsErrorMessage)
                .Should().BeTrue();
        }

        private UpdateHearingParticipantsRequest BuildRequest()
        {
            return Builder<UpdateHearingParticipantsRequest>.CreateNew()
                 .Build();
        }
    }
}