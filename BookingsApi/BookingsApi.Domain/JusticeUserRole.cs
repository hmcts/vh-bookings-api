using System;
using BookingsApi.Domain.RefData;

namespace BookingsApi.Domain;

public class JusticeUserRole : TrackableEntity<int>
{
    public JusticeUserRole(){}
    public JusticeUserRole(JusticeUser justiceUser, UserRole userRole)
    {
        JusticeUser = justiceUser;
        UserRole = userRole;
    }

    public Guid JusticeUserId { get; set; }
    public virtual JusticeUser JusticeUser { get; set; }
    public int UserRoleId { get; set; }
    public virtual UserRole UserRole { get; set; }
}