using System;
using System.Collections.Generic;

namespace BookingsApi.DAL.Dtos
{
    public class UsernamesToAnonymiseDto
    {
        public List<string> Usernames { get; set; }
        public List<Guid> HearingIds { get; set; }
    }
}