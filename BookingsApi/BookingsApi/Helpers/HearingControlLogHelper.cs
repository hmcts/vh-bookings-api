using BookingsApi.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using BookingsApi.Contract.V1.Requests;
using Newtonsoft.Json;
using FluentValidation.Results;

namespace BookingsApi.Helpers
{
    public static class HearingControlLogHelper
    {
        public static Dictionary<string, string> ErrorMessages(BookNewHearingRequest request)
        {
            return new Dictionary<string, string>
                {
                    {"payload", JsonConvert.SerializeObject(request)},
                    {"ScheduledDateTime", request.ScheduledDateTime.ToString("s")},
                    {"ScheduledDuration", request.ScheduledDuration.ToString()},
                    {"CaseTypeName", request.CaseTypeName},
                    {"HearingTypeName", request.HearingTypeName}
                };
        }

        public static Dictionary<string, string> ErrorMessages(ValidationResult result,BookNewHearingRequest request)
        {
            var dictionary = result.Errors.ToDictionary(x => $"{x.PropertyName}-{Guid.NewGuid()}", x => x.ErrorMessage);
            dictionary.Add("payload", JsonConvert.SerializeObject(request));
            return dictionary;
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
