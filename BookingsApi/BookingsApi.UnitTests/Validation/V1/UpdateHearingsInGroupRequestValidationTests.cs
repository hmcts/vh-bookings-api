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
        
        [Test]
        public async Task Should_return_missing_hearing_venue_name_error()
        {
            var request = BuildRequest();
            request.Hearings[0].HearingVenueName = string.Empty;
            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Exists(x => x.ErrorMessage == UpdateHearingRequestValidation.NoHearingVenueNameErrorMessage)
                .Should().BeTrue();
        }
        
        [Test]
        public async Task Should_return_invalid_scheduled_date_time_error()
        {
            var request = BuildRequest();
            request.Hearings[0].ScheduledDateTime = DateTime.Today.AddDays(-1).AddHours(10);
            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Exists(x => x.ErrorMessage == UpdateHearingRequestValidation.ScheduleDateTimeInPastErrorMessage)
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
            result.Errors.Exists(x => x.ErrorMessage == UpdateHearingRequestValidation.NoScheduleDurationErrorMessage)
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
            result.Errors.Exists(x => x.ErrorMessage == CaseRequestValidation.CaseNumberMessage)
                .Should().BeTrue();
        }
        
        private static UpdateHearingsInGroupRequest BuildRequest()
        {
            var existingParticipants = new Builder()
                .CreateListOfSize<UpdateParticipantRequest>(2)
                .Build()
                .ToList();

            for (var i = 0; i < existingParticipants.Count; i++)
            {
                existingParticipants[i].ContactEmail = $"contactEmail_{i}@email.com";
            }
            
            return new UpdateHearingsInGroupRequest
            {
                UpdatedBy = "updatedBy@email.com",
                Hearings = new List<HearingRequest>
                {
                    new()
                    {
                        HearingId = Guid.NewGuid(),
                        HearingVenueName = "VenueName",
                        ScheduledDateTime = DateTime.Today.AddDays(1).AddHours(10),
                        ScheduledDuration = 45,
                        CaseNumber = "CaseNumber",
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
