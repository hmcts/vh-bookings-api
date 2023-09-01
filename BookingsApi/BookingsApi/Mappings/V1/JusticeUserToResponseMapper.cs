using BookingsApi.Contract.V1.Responses;
using JusticeUserRole = BookingsApi.Contract.V1.Requests.Enums.JusticeUserRole;

namespace BookingsApi.Mappings.V1
{
    public static class JusticeUserToResponseMapper
    {
        public static JusticeUserResponse Map(JusticeUser justiceUser)
        {
            return new JusticeUserResponse
                {
                    FirstName = justiceUser.FirstName,
                    Lastname = justiceUser.Lastname,
                    ContactEmail = justiceUser.ContactEmail,
                    Username = justiceUser.Username,
                    Telephone = justiceUser.Telephone,
                    UserRoles = justiceUser.JusticeUserRoles.Select(x=> (JusticeUserRole) x.UserRole.Id).ToList(),
                    IsVhTeamLeader = justiceUser.IsTeamLeader(),
                    CreatedBy = justiceUser.CreatedBy,
                    Id = justiceUser.Id,
                    FullName = justiceUser.FirstName + " " + justiceUser.Lastname,
                    Deleted = justiceUser.Deleted
                };
        }
    }
}
