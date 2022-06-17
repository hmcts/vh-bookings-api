using System.Net;
using BookingsApi.Controllers;
using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Commands.Core;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace BookingsApi.UnitTests.Controllers
{
    public class JudiciaryPersonStagingControllerTests
    {
        private JudiciaryPersonStagingController _controller;
        private Mock<ICommandHandler> _commandHandlerMock;
        
        [SetUp]
        public void Setup()
        {
            _commandHandlerMock = new Mock<ICommandHandler>();
   
            _controller = new JudiciaryPersonStagingController(_commandHandlerMock.Object);
        }

        [Test]
        public void RemoveAllJudiciaryPersonsStaging_Should_Clear_Existing_Entries()
        {
            var result =_controller.RemoveAllJudiciaryPersonsStaging();
            
            result.Should().NotBeNull();
            var objectResult = (OkResult)result.Result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            
            _commandHandlerMock.Verify(c => c.Handle(It.IsAny<RemoveAllJudiciaryPersonStagingCommand>()), Times.Exactly(1));
        }
    }
}