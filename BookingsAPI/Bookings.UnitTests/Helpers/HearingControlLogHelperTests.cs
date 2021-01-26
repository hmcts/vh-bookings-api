using Bookings.Api.Contract.Requests;
using Bookings.API.Helpers;
using Bookings.Domain;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Linq;
using FluentValidation.Results;
using System.Collections.Generic;
using Bookings.Domain;
using Testing.Common.Builders.Domain;
using Microsoft.EntityFrameworkCore.Internal;

namespace Bookings.UnitTests.Helpers
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
            var testValues = new string[] { "TestVal1", "TestVal2" };
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
            result["payload"].Should().NotBe("Empty Payload");
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
            result["payload"].Should().NotBe("Empty Payload"); 
            result.Keys.FirstOrDefault().StartsWith("TestProp-");
            result.Keys.FirstOrDefault().Length.Should().Be(45);
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
            result.Count().Should().Be(3);
            result["HearingId"].Should().Be(videoHearing.Id.ToString());
            result["CaseType"].Should().Be(videoHearing.CaseType?.Name);
            result["Participants.Count"].Should().Be(videoHearing.Participants.Count.ToString());
        }


    }
}
