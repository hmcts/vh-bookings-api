using System.Collections.Generic;

namespace BookingsApi.Contract.Responses
{
    public class UserWithClosedConferencesResponse
    {
        public IList<string> Usernames { get; set; }
    }
}

