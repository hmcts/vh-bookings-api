using System.Linq;
using System.Threading.Tasks;
using Bookings.Api.Contract.Requests;
using Bookings.API.Validations;
using FizzWare.NBuilder;
using FluentAssertions;
using NUnit.Framework;

namespace Bookings.UnitTests.Validation
{
    public class LinkedParticipantValidationTests
    {
        private LinkedParticipantRequestValidation _validator;
        private LinkedParticipantRequest _request;

        [SetUp]
        public void SetUp()
        {
            _validator = new LinkedParticipantRequestValidation();
            _request = BuildRequest();
        }
        
        [Test]
        public async Task Should_Pass_Validation()
        {
            var result = await _validator.ValidateAsync(_request);

            result.IsValid.Should().BeTrue();
        }

        [Test]
        public async Task Should_Return_Missing_Participant_Email_Error()
        {
            _request.ParticipantContactEmail = string.Empty;

            var result = await _validator.ValidateAsync(_request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == LinkedParticipantRequestValidation.NoParticipantEmail)
                .Should().BeTrue();
        }
        
        [Test]
        public async Task Should_Return_Missing_LinkedParticipant_Email_Error()
        {
            _request.LinkedParticipantContactEmail = string.Empty;

            var result = await _validator.ValidateAsync(_request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == LinkedParticipantRequestValidation.NoLinkedParticipantEmail)
                .Should().BeTrue();
        }
        
        private LinkedParticipantRequest BuildRequest()
        {
            return Builder<LinkedParticipantRequest>.CreateNew()
                .With(x => x.ParticipantContactEmail = "interpretee@test.com")
                .With(x => x.LinkedParticipantContactEmail = "interpreter@test.com")
                .Build();
        }
    }
}