using System.Linq;
using System.Threading.Tasks;
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

        private UpdateParticipantRequest BuildRequest()
        {
            return Builder<UpdateParticipantRequest>.CreateNew()
                 .Build();
        }
    }
}