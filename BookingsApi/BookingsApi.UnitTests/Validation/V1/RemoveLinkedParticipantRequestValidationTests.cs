using System;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.Contract.V1.Requests;
using BookingsApi.Validations.V1;
using FluentAssertions;
using NUnit.Framework;

namespace BookingsApi.UnitTests.Validation.V1
{
    public class RemoveLinkedParticipantRequestValidationTests
    {
        private RemoveLinkedParticipantRequestValidation _validator;
        private Guid _id;

        [SetUp]
        public void SetUp()
        {
            _validator = new RemoveLinkedParticipantRequestValidation();
            _id = Guid.NewGuid();
        }

        [Test]
        public async Task Should_Pass_Validation()
        {
            var request = new RemoveLinkedParticipantRequest(_id);
            
            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeTrue();
        }
        
        [Test]
        public async Task Should_Fail_When_Guid_Is_Empty()
        {
            var request = new RemoveLinkedParticipantRequest(Guid.Empty);
            
            var result = await _validator.ValidateAsync(request);
            
            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == RemoveLinkedParticipantRequestValidation.NoId)
                .Should().BeTrue();
        }
    }
}