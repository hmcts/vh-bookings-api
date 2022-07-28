using System.Threading.Tasks;
using BookingsApi.Controllers;
using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Commands.Core;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace BookingsApi.UnitTests.Controllers
{
    
    public class JobHistoryControllerTests
    {
        private JobHistoryController _controller;
        private Mock<ICommandHandler> _commandHandlerMock;
        [SetUp]
        public void Setup()
        {
            _commandHandlerMock = new Mock<ICommandHandler>();
            _controller = new JobHistoryController(_commandHandlerMock.Object);
        }
        
        [Test]
        public async Task Should_call_add_job_history_command_and_return_ok()
        {
            var result = await _controller.UpdateJobHistory(It.IsAny<string>(), It.IsAny<bool>());
            _commandHandlerMock.Verify(e => e.Handle(It.IsAny<AddJobHistoryCommand>()), Times.Exactly(1));
            result.Should().BeOfType<OkResult>();
        }
    }
}