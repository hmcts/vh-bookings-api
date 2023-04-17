using System.Linq;
using BookingsApi.Contract.Responses;
using BookingsApi.Domain;

namespace BookingsApi.Mappings
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
                UserRoles = judiciaryPersonStagingRequest.JusticeUserRoles
                    .Select(jur => new UserRoleResponse(jur.UserRole.Id, jur.UserRole.Name))
                    .ToArray(),
                IsVhTeamLeader = judiciaryPersonStagingRequest.JusticeUserRoles.Any(jur => jur.UserRole.IsVhTeamLead),
                CreatedBy = judiciaryPersonStagingRequest.CreatedBy,
                Id = judiciaryPersonStagingRequest.Id,
                FullName = judiciaryPersonStagingRequest.FirstName + " " + judiciaryPersonStagingRequest.Lastname,
                Deleted = judiciaryPersonStagingRequest.Deleted
            };
        }
    }
}
