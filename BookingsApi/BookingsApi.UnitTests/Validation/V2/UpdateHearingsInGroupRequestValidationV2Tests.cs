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

        [Test]
        public async Task Should_return_missing_hearing_venue_code_error()
        {
            var request = BuildRequest();
            request.Hearings[0].HearingVenueCode = string.Empty;
            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Exists(x => x.ErrorMessage == HearingRequestInputValidationV2.NoHearingVenueCodeErrorMessage)
                .Should().BeTrue();
        }

        [Test]
        public async Task Should_return_invalid_schedule_duration_error()
        {
            var request = BuildRequest();
            request.Hearings[0].ScheduledDuration = 0;
            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Exists(x => x.ErrorMessage == HearingRequestInputValidationV2.NoScheduleDurationErrorMessage)
                .Should().BeTrue();
        }

        [Test]
        public async Task Should_return_missing_case_number_error()
        {
            var request = BuildRequest();
            request.Hearings[0].CaseNumber = string.Empty;
            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Exists(x => x.ErrorMessage == HearingRequestInputValidationV2.CaseNumberErrorMessage)
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
                        HearingVenueCode = "VenueCode",
                        ScheduledDuration = 45,
                        CaseNumber = "CaseNumber",
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
