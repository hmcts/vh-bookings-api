using System.Linq;
using System.Threading.Tasks;
using BookingsApi.Contract.V1.Enums;
using BookingsApi.Contract.V1.Requests;
using BookingsApi.Validations;
using BookingsApi.Validations.V1;
using FizzWare.NBuilder;
using FluentAssertions;
using NUnit.Framework;

namespace BookingsApi.UnitTests.Validation
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
        
        [Test]
        public async Task Should_Return_Invalid_LinkedParticipant_Type_Error()
        {
            _request.Type = (LinkedParticipantType)456789;

            var result = await _validator.ValidateAsync(_request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == LinkedParticipantRequestValidation.InvalidType)
                .Should().BeTrue();
        }
        
        private LinkedParticipantRequest BuildRequest()
        {
            return Builder<LinkedParticipantRequest>.CreateNew()
                .With(x => x.ParticipantContactEmail = "interpretee@hmcts.net")
                .With(x => x.LinkedParticipantContactEmail = "interpreter@hmcts.net")
                .With(x => x.Type = LinkedParticipantType.Interpreter)
                .Build();
        }
    }
}