using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bookings.Api.Contract.Requests;
using Bookings.API.Validations;
using Bookings.UnitTests.Controllers.HearingsController.Helpers;
using FizzWare.NBuilder;
using FluentAssertions;
using NUnit.Framework;

namespace Bookings.UnitTests.Validation
{
    public class BookNewHearingRequestValidationTests
    {
        private BookNewHearingRequestValidation _validator;
        
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _validator = new BookNewHearingRequestValidation();
        }
        
        [Test]
        public async Task Should_pass_validation()
        {
            var request = BuildRequest();

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeTrue();
        }
        
        [Test]
        public async Task Should_return_missing_hearing_venue_name_error()
        {
            var request = BuildRequest();
            request.HearingVenueName = string.Empty;
           
            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == BookNewHearingRequestValidation.HearingVenueErrorMessage)
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
            result.Errors.Any(x => x.ErrorMessage == BookNewHearingRequestValidation.ScheduleDateTimeInPastErrorMessage)
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
            result.Errors.Any(x => x.ErrorMessage == BookNewHearingRequestValidation.ScheduleDurationErrorMessage)
                .Should().BeTrue();
        }
        
        [Test]
        public async Task Should_return_missing_case_type_name_error()
        {
            var request = BuildRequest();
            request.CaseTypeName = string.Empty;
           
            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == BookNewHearingRequestValidation.CaseTypeNameErrorMessage)
                .Should().BeTrue();
        }
        
        [Test]
        public async Task Should_return_missing_hearing_type_name_error()
        {
            var request = BuildRequest();
            request.HearingTypeName = string.Empty;
           
            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == BookNewHearingRequestValidation.HearingTypeErrorMessage)
                .Should().BeTrue();
        }
        
        [Test]
        public async Task Should_return_missing_participants_error()
        {
            var request = BuildRequest();
            request.Participants = Enumerable.Empty<ParticipantRequest>().ToList();
           
            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(3);
            result.Errors.Any(x => x.ErrorMessage == BookNewHearingRequestValidation.ParticipantsErrorMessage)
                .Should().BeTrue();
        }

        [Test]
        public async Task Should_return_missing_cases_error()
        {
            var request = BuildRequest();
            request.Cases = Enumerable.Empty<CaseRequest>().ToList();
           
            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(2);
            result.Errors.Any(x => x.ErrorMessage == BookNewHearingRequestValidation.CasesErrorMessage)
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
            result.Errors.Any(x => x.ErrorMessage == CaseRequestValidation.CaseNameMessage)
                .Should().BeTrue();
        }
        
        [Test]
        public async Task Should_return_case_duplication_error()
        {
            var request = BuildRequest();
            request.Cases = new List<CaseRequest>
            {
                new CaseRequest{Name = "one", Number = "one" },
                new CaseRequest{Name = "one", Number = "one" }
            };
           
            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == BookNewHearingRequestValidation.CaseDuplicationErrorMessage)
                .Should().BeTrue();
        }

        [Test]
        public async Task Should_return_judge_absence_error()
        {
            var participants = Builder<ParticipantRequest>.CreateListOfSize(4).Build().ToList();
            var cases = Builder<CaseRequest>.CreateListOfSize(2).Build().ToList();

            var judgelessRequest = Builder<BookNewHearingRequest>.CreateNew()
                .With(x => x.CaseTypeName = "Civil Money Claims")
                .With(x => x.HearingTypeName = "Application to Set Judgment Aside")
                .With(x => x.HearingVenueName = "Birmingham Civil and Family Justice Centre")
                .With(x => x.Participants = participants.Where(participant => participant.HearingRoleName != "Judge").ToList())
                .With(x => x.Cases = cases)
                .Build();

            var result = await _validator.ValidateAsync(judgelessRequest);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == BookNewHearingRequestValidation.JudgeAbsenceErrorMessage)
                .Should().BeTrue();
        }
        
        [Test]
        public async Task Should_Pass_When_Linked_Participant_Is_Empty()
        {
            var request = BuildRequest();
            request.LinkedParticipants = new List<LinkedParticipantRequest>();

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeTrue();
        }
        
        private BookNewHearingRequest BuildRequest()
        {
            return RequestBuilder.Build();
        }
    }
}