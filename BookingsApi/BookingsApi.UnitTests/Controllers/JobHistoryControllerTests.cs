using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookingsApi.Controllers;
using BookingsApi.Controllers.V1;
using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Commands.Core;
using BookingsApi.DAL.Queries;
using BookingsApi.DAL.Queries.Core;
using BookingsApi.Domain;
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
        private Mock<IQueryHandler> _queryHandlerMock;
        [SetUp]
        public void Setup()
        {
            _commandHandlerMock = new Mock<ICommandHandler>();
            _queryHandlerMock = new Mock<IQueryHandler>();
            _controller = new JobHistoryController(_commandHandlerMock.Object, _queryHandlerMock.Object);
        }
        
        [Test]
        public async Task Should_call_add_job_history_command_and_return_no_content()
        {
            var result = await _controller.UpdateJobHistory(It.IsAny<string>(), It.IsAny<bool>());
            _commandHandlerMock.Verify(e => e.Handle(It.IsAny<AddJobHistoryCommand>()), Times.Exactly(1));
            result.Should().BeOfType<NoContentResult>();
        }
                
        [Test]
        public async Task Should_call_get_job_history_and_return_ok()
        {
            _queryHandlerMock.Setup(e =>
                    e.Handle<GetJobHistoryByJobNameQuery, List<JobHistory>>(It.IsAny<GetJobHistoryByJobNameQuery>()))
                     .ReturnsAsync(new List<JobHistory> {new UpdateJobHistory("mockJob", true)});
            var result = await _controller.GetJobHistory(It.IsAny<String>());
            _queryHandlerMock.Verify(e => e.Handle<GetJobHistoryByJobNameQuery, List<JobHistory>>(It.IsAny<GetJobHistoryByJobNameQuery>()), Times.Exactly(1));
            result.Should().BeOfType<OkObjectResult>();
        }
    }
}