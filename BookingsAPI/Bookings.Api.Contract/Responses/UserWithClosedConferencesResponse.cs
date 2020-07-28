using System.Collections.Generic;

namespace Bookings.Api.Contract.Responses
{
    public class UserWithClosedConferencesResponse
    {
        public IList<string> Usernames { get; set; }
    }
}

