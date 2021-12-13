using System.Collections.Generic;

namespace BookingsApi.DAL.Helper
{
    public static class UserRoleForHearingRole
    {

        public static Dictionary<string, int> UserRoleId = new Dictionary<string, int>
        {
            { UserRoles.Judge, 4 },
            { UserRoles.Individual, 5},
            { UserRoles.Observer, 5},
            { UserRoles.Respondent, 5},
            { UserRoles.Representative, 6 },
            { UserRoles.PanelMember, 7 },
            { UserRoles.JudicialOfficeHolder, 7 },
            { UserRoles.StaffMember, 8}
        };
    }
}