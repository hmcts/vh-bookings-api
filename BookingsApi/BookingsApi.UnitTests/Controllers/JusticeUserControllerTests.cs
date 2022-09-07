using BookingsApi.Controllers;
using BookingsApi.DAL.Queries;
using BookingsApi.DAL.Queries.Core;
using BookingsApi.Domain;
using Microsoft.AspNetCore.Mvc;
using Moq;
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

            // Act
            var response = await _controller.GetJusticeUserByUsername(_justiceUser.Username) as OkObjectResult;

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(response);
            Assert.AreEqual(_justiceUser, response.Value);
        }
    }
}
