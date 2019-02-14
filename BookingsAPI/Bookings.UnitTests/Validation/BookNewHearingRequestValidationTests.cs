using System;
using System.Linq;
using System.Threading.Tasks;
using Bookings.Api.Contract.Requests;
using Bookings.API.Validations;
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
        public async Task should_pass_validation()
        {
            var request = BuildRequest();

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeTrue();
        }
        
        [Test]
        public async Task should_return_missing_hearing_venue_name_error()
        {
            var request = BuildRequest();
            request.HearingVenueName = string.Empty;
           
            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == BookNewHearingRequestValidation.NoHearingVenueErrorMessage)
                .Should().BeTrue();
        }
        
        [Test]
        public async Task should_return_hearing_schedule_date_time_in_past_error()
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
        public async Task should_return_hearing_schedule_duration_error()
        {
            var request = BuildRequest();
            request.ScheduledDuration = 0;
            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == BookNewHearingRequestValidation.NoScheduleDurationErrorMessage)
                .Should().BeTrue();
        }
        
        [Test]
        public async Task should_return_missing_case_type_name_error()
        {
            var request = BuildRequest();
            request.CaseTypeName = string.Empty;
           
            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == BookNewHearingRequestValidation.NoCaseTypeNameErrorMessage)
                .Should().BeTrue();
        }
        
        [Test]
        public async Task should_return_missing_hearing_type_name_error()
        {
            var request = BuildRequest();
            request.HearingTypeName = string.Empty;
           
            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == BookNewHearingRequestValidation.NoHearingTypeErrorMessage)
                .Should().BeTrue();
        }
        
        [Test]
        public async Task should_return_missing_participants_error()
        {
            var request = BuildRequest();
            request.Participants = Enumerable.Empty<ParticipantRequest>().ToList();
           
            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(2);
            result.Errors.Any(x => x.ErrorMessage == BookNewHearingRequestValidation.NoParticipantsErrorMessage)
                .Should().BeTrue();
        }
        
        [Test]
        public async Task should_return_participants_error()
        {
            var request = BuildRequest();
            request.Participants[0].Title = string.Empty;
           
            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == ParticipantRequestValidation.NoTitleErrorMessage)
                .Should().BeTrue();
        }
        
        [Test]
        public async Task should_return_missing_cases_error()
        {
            var request = BuildRequest();
            request.Cases = Enumerable.Empty<CaseRequest>().ToList();
           
            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(2);
            result.Errors.Any(x => x.ErrorMessage == BookNewHearingRequestValidation.NoCasesErrorMessage)
                .Should().BeTrue();
        }
        
        [Test]
        public async Task should_return_cases_error()
        {
            var request = BuildRequest();
            request.Cases[0].Name = string.Empty;
           
            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == CaseRequestValidation.NoCaseNameMessage)
                .Should().BeTrue();
        }
        
        private BookNewHearingRequest BuildRequest()
        {
            var participants = Builder<ParticipantRequest>.CreateListOfSize(4).Build().ToList();
            var cases = Builder<CaseRequest>.CreateListOfSize(2).Build().ToList();
            
            return Builder<BookNewHearingRequest>.CreateNew()
                .With(x => x.CaseTypeName = "Civil Money Claims")
                .With(x => x.HearingTypeName = "Application to Set Judgment Aside")
                .With(x => x.HearingVenueName = "Birmingham Civil and Family Justice Centre")
                .With(x => x.Participants = participants)
                .With(x => x.Cases = cases)
                .Build();
        }
    }
}