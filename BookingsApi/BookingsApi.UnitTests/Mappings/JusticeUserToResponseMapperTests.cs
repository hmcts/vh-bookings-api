﻿using BookingsApi.Contract.Responses;
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
        public void Map_MapsJusticeUser_ToJusticeUserReponse()
        {
            // Arrange
            var justiceUser = new JusticeUser
            {
                FirstName = "FirstName",
                Lastname = "Lastname",
                Username = "email.test@email.com",
                ContactEmail = "email.test@email.com",
                UserRole = new UserRole((int)UserRoleId.VhTeamLead, "Video Hearings Team Lead"),
                CreatedBy = "created.by@email.com",
                Telephone = "0123456789",
                UserRoleId = (int)UserRoleId.VhTeamLead
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
                UserRoleId = (int)UserRoleId.VhTeamLead
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
