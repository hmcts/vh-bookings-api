using BookingsApi.Contract.V1.Requests;
using BookingsApi.Validations.V1;
using FizzWare.NBuilder;

namespace BookingsApi.UnitTests.Validation.V1
{
    public class AddParticipantsToHearingRequestValidationTests
    {
        private AddParticipantsToHearingRequestValidation _validator;
        
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _validator = new AddParticipantsToHearingRequestValidation();
        }
        
        [Test]
        public async Task Should_pass_validation()
        {
            var request = BuildRequest();

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeTrue();
        }
        
        [Test]
        public async Task Should_return_missing_participants_error()
        {
            var request = BuildRequest();
            request.Participants = Enumerable.Empty<ParticipantRequest>().ToList();
           
            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors
                .Any(x => x.ErrorMessage == AddParticipantsToHearingRequestValidation.NoParticipantsErrorMessage)
                .Should().BeTrue();
        }
        
        private static AddParticipantsToHearingRequest BuildRequest()
        {
            var participants = Builder<ParticipantRequest>.CreateListOfSize(4)
                .All()
                .With(x=> x.TelephoneNumber, "020 7946 0101")
                .Build().ToList();
            participants[0].ContactEmail = "me0@me.com";
            participants[1].ContactEmail = "me1@me.com";
            participants[2].ContactEmail = "me2@me.com";
            participants[3].ContactEmail = "me3@me.com";
            return new AddParticipantsToHearingRequest
            {
                Participants = participants
            };
        }
    }
}