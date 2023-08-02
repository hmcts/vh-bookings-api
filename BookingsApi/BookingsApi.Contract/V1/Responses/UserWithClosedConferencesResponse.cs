using System.Collections.Generic;

namespace BookingsApi.Contract.V1.Responses
{
    public class UserWithClosedConferencesResponse
    {
        public IList<string> Usernames { get; set; }
    }
}

