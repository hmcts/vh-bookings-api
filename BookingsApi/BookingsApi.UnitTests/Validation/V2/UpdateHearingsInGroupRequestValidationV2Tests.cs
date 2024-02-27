using System.Collections.Generic;
using BookingsApi.Contract.V2.Requests;
using BookingsApi.Validations.V2;
using FizzWare.NBuilder;

namespace BookingsApi.UnitTests.Validation.V2
{
    public class UpdateHearingsInGroupRequestValidationV2Tests
    {
        private UpdateHearingsInGroupRequestInputValidationV2 _validator;
        
        [SetUp]
        public void SetUp()
        {
            _validator = new UpdateHearingsInGroupRequestInputValidationV2();
        }
        
        [Test]
        public async Task Should_return_missing_updated_by_error()
        {
            var request = BuildRequest();
            request.UpdatedBy = string.Empty;
            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Exists(x => x.ErrorMessage == UpdateHearingsInGroupRequestInputValidationV2.NoUpdatedByErrorMessage)
                .Should().BeTrue();
        }
        
        private static UpdateHearingsInGroupRequestV2 BuildRequest()
        {
            var existingParticipants = new Builder()
                .CreateListOfSize<UpdateParticipantRequestV2>(2)
                .Build()
                .ToList();
            
            return new UpdateHearingsInGroupRequestV2
            {
                UpdatedBy = "updatedBy@email.com",
                Hearings = new List<HearingRequestV2>
                {
                    new()
                    {
                        HearingId = Guid.NewGuid(),
                        Endpoints = new UpdateHearingEndpointsRequestV2(),
                        Participants = new UpdateHearingParticipantsRequestV2()
                        {
                            ExistingParticipants = existingParticipants
                        }
                    }
                }
            };
        }
    }
}
