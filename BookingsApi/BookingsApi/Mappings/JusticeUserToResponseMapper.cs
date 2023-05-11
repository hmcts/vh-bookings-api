using System;
using System.Linq;
using BookingsApi.Contract.Responses;
using BookingsApi.Domain;
using JusticeUserRole = BookingsApi.Contract.Requests.Enums.JusticeUserRole;

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
