using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.Contract.V2.Requests;
using BookingsApi.Validations.V2;
using FluentAssertions;
using NUnit.Framework;
using Testing.Common.Builders.Api.V2;

namespace BookingsApi.UnitTests.Validation.V2
{
    public class BookNewHearingRequestValidationV2Tests
    {
        private BookNewHearingRequestValidationV2 _validator;
        
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _validator = new BookNewHearingRequestValidationV2();
        }
        
        [Test]
        public async Task Should_pass_validation()
        {
            var request = BuildRequest();

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeTrue();
        }
        
        [Test]
        public async Task Should_return_missing_hearing_venue_code_error()
        {
            var request = BuildRequest();
            request.HearingVenueCode = string.Empty;
           
            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == BookNewHearingRequestValidationV2.HearingVenueCodeErrorMessage)
                .Should().BeTrue();
        }
        
        [Test]
        public async Task Should_return_missing_hearing_type_code_error()
        {
            var request = BuildRequest();
            request.HearingTypeCode = string.Empty;
           
            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == BookNewHearingRequestValidationV2.HearingTypeCodeErrorMessage)
                .Should().BeTrue();
        }
        
        [Test]
        public async Task Should_return_hearing_schedule_date_time_in_past_error()
        {
            var request = BuildRequest();
            request.ScheduledDateTime = DateTime.MinValue;
            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == BookNewHearingRequestValidationV2.ScheduleDateTimeInPastErrorMessage)
                .Should().BeTrue();
        }
        
        [Test]
        public async Task Should_return_hearing_schedule_duration_error()
        {
            var request = BuildRequest();
            request.ScheduledDuration = 0;
            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == BookNewHearingRequestValidationV2.ScheduleDurationErrorMessage)
                .Should().BeTrue();
        }
        
        [Test]
        public async Task Should_return_missing_case_type_service_id_error()
        {
            var request = BuildRequest();
            request.ServiceId = string.Empty;
           
            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == BookNewHearingRequestValidationV2.CaseTypeServiceIdErrorMessage)
                .Should().BeTrue();
        }

        [Test]
        public async Task Should_return_missing_participants_error()
        {
            var request = BuildRequest();
            request.Participants = Enumerable.Empty<ParticipantRequestV2>().ToList();
           
            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(2);
            result.Errors.Any(x => x.ErrorMessage == BookNewHearingRequestValidationV2.ParticipantsErrorMessage)
                .Should().BeTrue();
        }

        [Test]
        public async Task Should_return_missing_cases_error()
        {
            var request = BuildRequest();
            request.Cases = Enumerable.Empty<CaseRequestV2>().ToList();
           
            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(2);
            result.Errors.Any(x => x.ErrorMessage == BookNewHearingRequestValidationV2.CasesErrorMessage)
                .Should().BeTrue();
        }
        
        [Test]
        public async Task Should_return_cases_error()
        {
            var request = BuildRequest();
            request.Cases[0].Name = string.Empty;
           
            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == CaseRequestValidationV2.CaseNameMessage)
                .Should().BeTrue();
        }
        
        [Test]
        public async Task Should_return_case_duplication_error()
        {
            var request = BuildRequest();
            request.Cases = new List<CaseRequestV2>
            {
                new() {Name = "one", Number = "one" },
                new() {Name = "one", Number = "one" }
            };
           
            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == BookNewHearingRequestValidationV2.CaseDuplicationErrorMessage)
                .Should().BeTrue();
        }

        [Test]
        public async Task Should_Pass_When_Linked_Participant_Is_Empty()
        {
            var request = BuildRequest();
            request.LinkedParticipants = new List<LinkedParticipantRequestV2>();

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeTrue();
        }
        
        private BookNewHearingRequestV2 BuildRequest()
        {
            var date = DateTime.UtcNow.AddMinutes(5);
            var caseName = $"Auto Test {Guid.NewGuid():N}";
            return new SimpleBookNewHearingRequestV2(caseName, date).Build();
        }
    }
}