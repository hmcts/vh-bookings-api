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

    public virtual JusticeUser JusticeUser { get; set; }
    public virtual UserRole UserRole { get; set; }
}