using System.Linq;
using BookingsApi.Contract.V1.Responses;
using BookingsApi.Domain;
using JusticeUserRole = BookingsApi.Contract.V1.Requests.Enums.JusticeUserRole;

namespace BookingsApi.Mappings.V1
{
    public static class JusticeUserToResponseMapper
    {
        public static JusticeUserResponse Map(
            JusticeUser judiciaryPersonStagingRequest)
        {
            return new JusticeUserResponse
                {
                    FirstName = judiciaryPersonStagingRequest.FirstName,
                    Lastname = judiciaryPersonStagingRequest.Lastname,
                    ContactEmail = judiciaryPersonStagingRequest.ContactEmail,
                    Username = judiciaryPersonStagingRequest.Username,
                    Telephone = judiciaryPersonStagingRequest.Telephone,
                    UserRoles = judiciaryPersonStagingRequest.JusticeUserRoles.Select(x=> (JusticeUserRole) x.UserRole.Id).ToList(),
                    IsVhTeamLeader = judiciaryPersonStagingRequest.IsTeamLeader(),
                    CreatedBy = judiciaryPersonStagingRequest.CreatedBy,
                    Id = judiciaryPersonStagingRequest.Id,
                    FullName = judiciaryPersonStagingRequest.FirstName + " " + judiciaryPersonStagingRequest.Lastname,
                    Deleted = judiciaryPersonStagingRequest.Deleted
                };
        }
    }
}
