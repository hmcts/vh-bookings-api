using System.Linq;
using System.Threading.Tasks;
using BookingsApi.Contract.Requests;
using BookingsApi.Validations;
using FizzWare.NBuilder;
using FluentAssertions;
using NUnit.Framework;

namespace BookingsApi.UnitTests.Validation
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
        
        private AddParticipantsToHearingRequest BuildRequest()
        {
            var participants = Builder<ParticipantRequest>.CreateListOfSize(4).Build().ToList();
            participants[0].ContactEmail = "me0@me.com";
            participants[0].TelephoneNumber = "(+44)123 456";
            participants[1].ContactEmail = "me1@me.com";
            participants[1].TelephoneNumber = "(+44)123 456";
            participants[2].ContactEmail = "me2@me.com";
            participants[2].TelephoneNumber = "(+44)123 456";
            participants[3].ContactEmail = "me3@me.com";
            participants[3].TelephoneNumber = "(+44)123 456";
            return new AddParticipantsToHearingRequest
            {
                Participants = participants
            };
        }
    }
}