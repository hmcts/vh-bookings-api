using System;
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
                    UserRoleId = judiciaryPersonStagingRequest.JusticeUserRoles.Select(x=>x.UserRole.Id).ToArray(), 
                    UserRoleName = judiciaryPersonStagingRequest.JusticeUserRoles.Select(x=>x.UserRole.Name).ToArray(), 
                    IsVhTeamLeader = judiciaryPersonStagingRequest.IsTeamLeader(),
                    CreatedBy = judiciaryPersonStagingRequest.CreatedBy,
                    Id = judiciaryPersonStagingRequest.Id,
                    FullName = judiciaryPersonStagingRequest.FirstName + " " + judiciaryPersonStagingRequest.Lastname,
                    Deleted = judiciaryPersonStagingRequest.Deleted
                };
        }
    }
}
