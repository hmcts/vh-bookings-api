using BookingsApi.Controllers;
using BookingsApi.DAL.Queries;
using BookingsApi.DAL.Queries.Core;
using BookingsApi.Domain;
using BookingsApi.Domain.RefData;
using BookingsApi.Mappings;
using BookingsApi.UnitTests.Constants;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Threading.Tasks;

namespace BookingsApi.UnitTests.Controllers
{
    public class JusticeUserControllerTests
    {

        private JusticeUserController _controller;
        private Mock<IQueryHandler> _queryHandlerMock;
        private JusticeUser _justiceUser;

        [SetUp]
        public void Setup()
        {
            _justiceUser = new JusticeUser
            {
                FirstName = "FirstName",
                Lastname = "Lastname",
                Username = "email.test@email.com",
                ContactEmail = "email.test@email.com",
                UserRole = new UserRole((int)UserRoleId.vhTeamLead, "Video Hearings Team Lead")
            };

            _queryHandlerMock = new Mock<IQueryHandler>();
            _queryHandlerMock.Setup(x => x.Handle<GetJusticeUserByUsernameQuery, JusticeUser>(It.Is<GetJusticeUserByUsernameQuery>(
                x => x.Username == _justiceUser.Username)))
                .ReturnsAsync(_justiceUser);

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
    }
}
