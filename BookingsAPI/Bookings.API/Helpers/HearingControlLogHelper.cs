using Bookings.Api.Contract.Requests;
using Bookings.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using FluentValidation.Results;

namespace Bookings.API.Helpers
{
    public static class HearingControlLogHelper
    {
        public static Dictionary<string, string> ErrorMessages(BookNewHearingRequest request)
        {
            var obj = JsonConvert.SerializeObject(request);
            var tmp = GetPayloadValue(obj);
            return new Dictionary<string, string>
                {
                    {"payload", GetPayloadValue(JsonConvert.SerializeObject(request))},
                    {"ScheduledDateTime", request.ScheduledDateTime.ToString("s")},
                    {"ScheduledDuration", request.ScheduledDuration.ToString()},
                    {"CaseTypeName", request.CaseTypeName},
                    {"HearingTypeName", request.HearingTypeName}
                };
        }

        public static Dictionary<string, string> ErrorMessages(ValidationResult result,BookNewHearingRequest request)
        {
            var dictionary = result.Errors.ToDictionary(x => $"{x.PropertyName}-{Guid.NewGuid()}", x => x.ErrorMessage);
            dictionary.Add("payload", GetPayloadValue(JsonConvert.SerializeObject(request)));
            return dictionary;
        }

        private static string GetPayloadValue(string payload)
        {
            return !string.IsNullOrWhiteSpace(payload) ? payload : "Empty Payload";
        }

        public static Dictionary<string, string> LogInfo(VideoHearing queriedVideoHearing)
        {
            return new Dictionary<string, string>
                {
                    {"HearingId", queriedVideoHearing.Id.ToString()},
                    {"CaseType", queriedVideoHearing.CaseType?.Name},
                    {"Participants.Count", queriedVideoHearing.Participants.Count.ToString()},
                };
        }

        public static Dictionary<string, string> AddTrace(string key, string value)
        {
            return new Dictionary<string, string>
                    {
                        { key, value }
                    };
        }

        public static Dictionary<string, string> AddTrace(string key, IEnumerable<object> value)
        {
            return AddTrace(key, string.Join(", ", value));
        }

    }
}
