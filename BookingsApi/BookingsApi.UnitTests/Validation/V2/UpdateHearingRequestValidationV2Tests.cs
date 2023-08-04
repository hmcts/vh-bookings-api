using System;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.Contract.V2.Requests;
using BookingsApi.Validations.V2;
using FizzWare.NBuilder;

namespace BookingsApi.UnitTests.Validation.V2
{
    public class UpdateHearingRequestValidationV2Tests
    {
        private UpdateHearingRequestValidationV2 _validator;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _validator = new UpdateHearingRequestValidationV2();
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
            // arrange
            // venue name and code are both empty
            _validator = new UpdateHearingRequestValidationV2();
            var request = BuildRequest();
            request.HearingVenueCode = string.Empty;

            // act
            var result = await _validator.ValidateAsync(request);

            // assert
            result.Errors.Any(x => x.ErrorMessage == UpdateHearingRequestValidationV2.NoHearingVenueCodeErrorMessage)
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
            result.Errors.Any(x => x.ErrorMessage == UpdateHearingRequestValidationV2.ScheduleDateTimeInPastErrorMessage)
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
            result.Errors.Any(x => x.ErrorMessage == UpdateHearingRequestValidationV2.NoScheduleDurationErrorMessage)
                .Should().BeTrue();
        }
        
        [Test]
        public async Task Should_return_missing_updated_by_error()
        {
            var request = BuildRequest();
            request.UpdatedBy = string.Empty;
            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == UpdateHearingRequestValidationV2.NoUpdatedByErrorMessage)
                .Should().BeTrue();
        }

        private UpdateHearingRequestV2 BuildRequest()
        {
            var cases = Builder<CaseRequestV2>.CreateListOfSize(1).Build().ToList();
            cases[0].IsLeadCase = false;
            cases[0].Name = $"auto test validation {Faker.RandomNumber.Next(0, 9999999)}";
            cases[0].Number = $"{Faker.RandomNumber.Next(0, 9999)}/{Faker.RandomNumber.Next(0, 9999)}";
            return new UpdateHearingRequestV2
            {
                ScheduledDuration = 60,
                ScheduledDateTime = DateTime.Today.AddDays(5).AddHours(10).AddMinutes(30),
                HearingRoomName = "RoomUpdate",
                OtherInformation = "OtherInformationUpdate",
                UpdatedBy = "test@hmcts.net",
                Cases = cases,
                HearingVenueCode = "231596",
                AudioRecordingRequired = false
            };
        }
    }
}