using System.Collections.Generic;

namespace BookingsApi.DAL.Helper
{
    public static class UserRoleForHearingRole
    {
        public static Dictionary<string, int> UserRoleId = new Dictionary<string, int>
        {
            { "Judge", 4 },
            { "Individual", 5},
            { "Observer", 5},
            { "Respondent", 5},
            { "Panel Member", 7 },
            { "Staff Member", 8}
        };
    }
}