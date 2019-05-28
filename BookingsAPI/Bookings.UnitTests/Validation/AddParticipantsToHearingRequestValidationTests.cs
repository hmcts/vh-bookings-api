using System.Linq;
using System.Threading.Tasks;
using Bookings.Api.Contract.Requests;
using Bookings.API.Validations;
using FizzWare.NBuilder;
using FluentAssertions;
using NUnit.Framework;

namespace Bookings.UnitTests.Validation
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
        public async Task should_pass_validation()
        {
            var request = BuildRequest();

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeTrue();
        }
        
        [Test]
        public async Task should_return_missing_participants_error()
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

            return new AddParticipantsToHearingRequest
            {
                Participants = participants
            };
        }
    }
}