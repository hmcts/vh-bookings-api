using System.Collections.Generic;
using BookingsApi.Contract.V1.Requests;
using BookingsApi.Validations.V1;
using FizzWare.NBuilder;

namespace BookingsApi.UnitTests.Validation.V1
{
    public class UpdateHearingsInGroupRequestValidationTests
    {
        private UpdateHearingsInGroupRequestInputValidation _validator;
        
        [SetUp]
        public void SetUp()
        {
            _validator = new UpdateHearingsInGroupRequestInputValidation();
        }
        
        [Test]
        public async Task Should_return_missing_updated_by_error()
        {
            var request = BuildRequest();
            request.UpdatedBy = string.Empty;
            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Exists(x => x.ErrorMessage == UpdateHearingsInGroupRequestInputValidation.NoUpdatedByErrorMessage)
                .Should().BeTrue();
        }
        
        private static UpdateHearingsInGroupRequest BuildRequest()
        {
            var existingParticipants = new Builder()
                .CreateListOfSize<UpdateParticipantRequest>(2)
                .Build()
                .ToList();
            
            return new UpdateHearingsInGroupRequest
            {
                UpdatedBy = "updatedBy@email.com",
                Hearings = new List<HearingRequest>
                {
                    new()
                    {
                        HearingId = Guid.NewGuid(),
                        Endpoints = new UpdateHearingEndpointsRequest(),
                        Participants = new UpdateHearingParticipantsRequest()
                        {
                            ExistingParticipants = existingParticipants
                        }
                    }
                }
            };
        }
    }
}
