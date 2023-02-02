using System;
using System.Collections.Generic;
using System.Linq;
using BookingsApi.Controllers;
using BookingsApi.DAL.Queries;
using BookingsApi.DAL.Queries.Core;
using BookingsApi.Domain;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.RefData;
using BookingsApi.Mappings;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Threading.Tasks;
using FluentAssertions;

namespace BookingsApi.UnitTests.Controllers
{
    public class JusticeUserControllerTests
    {

        private JusticeUserController _controller;
        private Mock<IQueryHandler> _queryHandlerMock;
        private JusticeUser _justiceUser;
        private List<JusticeUser> _justiceUserList;

        [SetUp]
        public void Setup()
        {
            _justiceUserList = new List<JusticeUser>();
            _justiceUser = new JusticeUser
            {
                FirstName = "FirstName",
                Lastname = "Lastname",
                Username = "email.test@email.com",
                ContactEmail = "email.test@email.com",
                UserRole = new UserRole((int)UserRoleId.VhTeamLead, "Video Hearings Team Lead")
            };
            
            _justiceUserList.Add(_justiceUser);
            
            _justiceUser = new JusticeUser
            {
                FirstName = "FirstName2",
                Lastname = "Lastname2",
                Username = "email2.test@email.com",
                ContactEmail = "email2.test@email.com",
                UserRole = new UserRole((int)UserRoleId.VhTeamLead, "Video Hearings Team Lead")
            };
            _justiceUserList.Add(_justiceUser);

            _queryHandlerMock = new Mock<IQueryHandler>();
            _queryHandlerMock.Setup(x => x.Handle<GetJusticeUserByUsernameQuery, JusticeUser>(It.Is<GetJusticeUserByUsernameQuery>(
                    x => x.Username == _justiceUser.Username)))
                .ReturnsAsync(_justiceUser);
            
            _queryHandlerMock.Setup(x => 
                    x.Handle<GetJusticeUserListQuery, List<JusticeUser>>(It.IsAny<GetJusticeUserListQuery>()))
                .ReturnsAsync(_justiceUserList);

            _controller = new JusticeUserController(_queryHandlerMock.Object);
        }

        [Test]
        public async Task GetJusticeUserByUsername_ReturnsNull_WhenUserIsNotFound()
        {
            // Arrange

            // Act
            var response = await _controller.GetJusticeUserByUsername("dont.exist@email.com");

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(response);
        }

        [Test]
        public async Task GetJusticeUserByUsername_ReturnsUser_WhenUserIsFound()
        {
            // Arrange
            var expectedJusticeUserReponse = JusticeUserToResponseMapper.Map(_justiceUser);
            var expectedJusticeUserReponseJson = JsonConvert.SerializeObject(expectedJusticeUserReponse);

            // Act
            var response = await _controller.GetJusticeUserByUsername(_justiceUser.Username) as OkObjectResult;
            var actualJusticeUserReponseJson = JsonConvert.SerializeObject(response.Value);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(response);
            Assert.AreEqual(expectedJusticeUserReponseJson, actualJusticeUserReponseJson);
        }
        
        [Test]
        public async Task GetJusticeUserList_ReturnsList()
        {
            // Arrange
            var expectedJusticeUserReponse = _justiceUserList.Select(user => JusticeUserToResponseMapper.Map(user));
            var expectedJusticeUserReponseJson = JsonConvert.SerializeObject(expectedJusticeUserReponse);

            // Act
            var response = await _controller.GetJusticeUserList(null) as OkObjectResult;
            var actualJusticeUserReponseJson = JsonConvert.SerializeObject(response.Value);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(response);
            Assert.AreEqual(expectedJusticeUserReponseJson, actualJusticeUserReponseJson);
        }
        [Test]
        public async Task GetJusticeUserList_ReturnsListEmpty()
        {
            // Arrange
            _queryHandlerMock.Setup(x => 
                    x.Handle<GetJusticeUserListQuery, List<JusticeUser>>(It.IsAny<GetJusticeUserListQuery>()))
                .ReturnsAsync(new List<JusticeUser>());
            var expectedJusticeUserReponseJson = JsonConvert.SerializeObject(new List<JusticeUser>());

            // Act
            var response = await _controller.GetJusticeUserList(null) as OkObjectResult;
            var actualJusticeUserReponseJson = JsonConvert.SerializeObject(response.Value);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(response);
            Assert.AreEqual(expectedJusticeUserReponseJson, actualJusticeUserReponseJson);

        }
    }
}
