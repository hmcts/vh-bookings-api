using BookingsApi.Contract.V2.Enums;
using BookingsApi.Contract.V2.Requests;
using BookingsApi.Validations.V2;
using FizzWare.NBuilder;

namespace BookingsApi.UnitTests.Validation.V2
{
    public class LinkedParticipantValidationV2Tests
    {
        private LinkedParticipantRequestValidationV2 _validator;
        private LinkedParticipantRequestV2 _requestV2;

        [SetUp]
        public void SetUp()
        {
            _validator = new LinkedParticipantRequestValidationV2();
            _requestV2 = BuildRequest();
        }
        
        [Test]
        public async Task Should_Pass_Validation()
        {
            var result = await _validator.ValidateAsync(_requestV2);

            result.IsValid.Should().BeTrue();
        }

        [Test]
        public async Task Should_Return_Missing_Participant_Email_Error()
        {
            _requestV2.ParticipantContactEmail = string.Empty;

            var result = await _validator.ValidateAsync(_requestV2);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == LinkedParticipantRequestValidationV2.NoParticipantEmail)
                .Should().BeTrue();
        }
        
        [Test]
        public async Task Should_Return_Missing_LinkedParticipant_Email_Error()
        {
            _requestV2.LinkedParticipantContactEmail = string.Empty;

            var result = await _validator.ValidateAsync(_requestV2);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == LinkedParticipantRequestValidationV2.NoLinkedParticipantEmail)
                .Should().BeTrue();
        }
        
        [Test]
        public async Task Should_Return_Invalid_LinkedParticipant_Type_Error()
        {
            _requestV2.TypeV2 = (LinkedParticipantTypeV2)456789;

            var result = await _validator.ValidateAsync(_requestV2);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == LinkedParticipantRequestValidationV2.InvalidType)
                .Should().BeTrue();
        }
        
        private LinkedParticipantRequestV2 BuildRequest()
        {
            return Builder<LinkedParticipantRequestV2>.CreateNew()
                .With(x => x.ParticipantContactEmail = "interpretee@hmcts.net")
                .With(x => x.LinkedParticipantContactEmail = "interpreter@hmcts.net")
                .With(x => x.TypeV2 = LinkedParticipantTypeV2.Interpreter)
                .Build();
        }
    }
}