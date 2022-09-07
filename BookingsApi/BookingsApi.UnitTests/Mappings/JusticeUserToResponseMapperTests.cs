using BookingsApi.Contract.Responses;
using BookingsApi.Domain;
using BookingsApi.Domain.RefData;
using BookingsApi.Mappings;
using BookingsApi.UnitTests.Constants;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookingsApi.UnitTests.Mappings
{
    public class JusticeUserToResponseMapperTests
    {
        [Test]
        public void Map_MapsJusticeUser_ToJusticeUserReponse()
        {
            // Arrange
            var justiceUser = new JusticeUser
            {
                FirstName = "FirstName",
                Lastname = "Lastname",
                Username = "email.test@email.com",
                ContactEmail = "email.test@email.com",
                UserRole = new UserRole((int)UserRoleId.vhTeamLead, "Video Hearings Team Lead"),
                CreatedBy = "created.by@email.com",
                Telephone = "0123456789",
                UserRoleId = (int)UserRoleId.vhTeamLead
            };

            var expectedJusticeUserResponse = new JusticeUserResponse
            {
                FirstName = "FirstName",
                Lastname = "Lastname",
                Username = "email.test@email.com",
                ContactEmail = "email.test@email.com",
                UserRoleName = "Video Hearings Team Lead",
                CreatedBy = "created.by@email.com",
                IsVhTeamLeader = true,
                Telephone = "0123456789",
                UserRoleId = (int)UserRoleId.vhTeamLead
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
