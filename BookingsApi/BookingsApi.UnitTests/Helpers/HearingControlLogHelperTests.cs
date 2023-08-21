﻿using BookingsApi.Helpers;
using BookingsApi.Contract.V1.Requests;
using FluentValidation.Results;
using Newtonsoft.Json;

namespace BookingsApi.UnitTests.Helpers
{
    public class HearingControlLogHelperTests
    {
        [TestCase]
        public void Should_return_dictionary_with_given_key_value()
        {
            var result = HearingControlLogHelper.AddTrace("TestKey","TestVal");

            result.Should().NotBeNull();
            result.Keys.Contains("TestKey").Should().BeTrue();
            result["TestKey"].Should().Be("TestVal");
        }

        [TestCase]
        public void Should_return_dictionary_with_given_key_objects()
        {
            var testValues = new[] { "TestVal1", "TestVal2" };
            var result = HearingControlLogHelper.AddTrace("TestKey", testValues);

            result.Should().NotBeNull();
            result.Keys.Contains("TestKey").Should().BeTrue();
            result["TestKey"].Should().Be("TestVal1, TestVal2");
        }

        [TestCase]
        public void Should_return_error_messages_with_booking_request()
        {
            var request = new BookNewHearingRequest
            {
                ScheduledDateTime = DateTime.Now,
                ScheduledDuration = 30,
                CaseTypeName = "TestCaseTypeName",
                HearingTypeName = "TestHearingTypeName"
            };
            var result = HearingControlLogHelper.ErrorMessages(request);

            result.Should().NotBeNull();
            result.Count.Should().Be(5);
            result["payload"].Should().Be(JsonConvert.SerializeObject(request));
            result["ScheduledDateTime"].Should().Be(request.ScheduledDateTime.ToString("s"));
            result["ScheduledDuration"].Should().Be(request.ScheduledDuration.ToString());
            result["CaseTypeName"].Should().Be(request.CaseTypeName);
            result["HearingTypeName"].Should().Be(request.HearingTypeName);
        }

        [TestCase]
        public void Should_return_error_messages_with_validation_result_and_booking_request()
        {
            var validationResult = new ValidationResult();
            validationResult.Errors.Add(new ValidationFailure("TestProp","Test Error Message"));

            var request = new BookNewHearingRequest();
            var result = HearingControlLogHelper.ErrorMessages(validationResult,request);

            result.Should().NotBeNull();
            result.Count.Should().Be(2);
            result["payload"].Should().Be(JsonConvert.SerializeObject(request)); 
            result.Keys.First().Should().StartWith("TestProp-");
            result.Keys.First().Length.Should().Be(45);
            result.ContainsValue("Test Error Message").Should().BeTrue();
           
        }

        [TestCase]
        public void Should_return_error_messages_with_empty_validation_result_and_booking_request()
        {
            var validationResult = new ValidationResult();

            var request = new BookNewHearingRequest();
            var result = HearingControlLogHelper.ErrorMessages(validationResult, request);

            result.Should().NotBeNull();
            result.Count.Should().Be(1);
            result["payload"].Should().NotBe("Empty Payload");
        }

        [TestCase]
        public void Should_return_log_info_with_videohearing()
        {
            var videoHearing = new VideoHearingBuilder().Build();
            var result = HearingControlLogHelper.LogInfo(videoHearing);

            result.Should().NotBeNull();
            result.Count.Should().Be(3);
            result["HearingId"].Should().Be(videoHearing.Id.ToString());
            result["CaseType"].Should().Be(videoHearing.CaseType?.Name);
            result["Participants.Count"].Should().Be(videoHearing.Participants.Count.ToString());
        }


    }
}
