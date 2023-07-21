using System.Collections.Generic;
using BookingsApi.Contract.V1.Responses;
using BookingsApi.Domain;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.RefData;
using BookingsApi.Mappings;
using Newtonsoft.Json;
using NUnit.Framework;

namespace BookingsApi.UnitTests.Mappings
{
    public class JusticeUserToResponseMapperTests
    {
        [Test]
        public void Map_MapsJusticeUser_ToJusticeUserResponse()
        {
            // Arrange
            var justiceUser = new JusticeUser
            {
                FirstName = "FirstName",
                Lastname = "Lastname",
                Username = "email.test@email.com",
                ContactEmail = "email.test@email.com", 
                CreatedBy = "created.by@email.com",
                Telephone = "0123456789",
            };
            
            var userRoles = new UserRole[]
            {
                new UserRole((int)UserRoleId.VhTeamLead, "Video Hearings Team Lead")
            };
            justiceUser.AddRoles(userRoles);

            List<Contract.V1.Requests.Enums.JusticeUserRole> roles = new List<Contract.V1.Requests.Enums.JusticeUserRole>();
            roles.Add(Contract.V1.Requests.Enums.JusticeUserRole.VhTeamLead);
            var expectedJusticeUserResponse = new JusticeUserResponse
            {
                FirstName = "FirstName",
                Lastname = "Lastname",
                Username = "email.test@email.com",
                ContactEmail = "email.test@email.com",
                CreatedBy = "created.by@email.com",
                IsVhTeamLeader = true,
                Telephone = "0123456789",
                UserRoles = roles,
                FullName = "FirstName Lastname"
            };

            var expectedJusticeUserResponseJson = JsonConvert.SerializeObject(expectedJusticeUserResponse);

            // Act
            var result = JusticeUserToResponseMapper.Map(justiceUser);
            var actualJusticeUserResponseJson = JsonConvert.SerializeObject(result);

            // Assert
            Assert.AreEqual(expectedJusticeUserResponseJson, actualJusticeUserResponseJson);
        }
    }
}
