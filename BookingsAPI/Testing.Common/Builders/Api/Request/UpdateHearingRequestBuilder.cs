using System;
using VhListings.Api.Contract.Requests;

namespace Testing.Common.Builders.Api.Request
{
    public static class UpdateHearingRequestBuilder
    {
        public static UpdateHearingRequest BuildRequest()
        {
            return new UpdateHearingRequest
            {
                CourtId = 1,
                ScheduledDateTime = DateTime.Today.AddHours(10),
                ScheduledDuration = 100
            };
        }
    }
}