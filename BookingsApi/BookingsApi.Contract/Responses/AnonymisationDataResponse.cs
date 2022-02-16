using System;
using System.Collections.Generic;

namespace BookingsApi.Contract.Responses
{
    public class AnonymisationDataResponse
    {
        public List<string> Usernames { get; set; }
        public List<Guid> HearingIds { get; set; }
    }
}