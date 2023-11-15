﻿using System.Collections.Generic;
using System.Net;
using BookingsApi.Contract.V1.Requests;
using BookingsApi.Controllers;
using BookingsApi.Controllers.V1;
using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Commands.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BookingsApi.UnitTests.Controllers
{
    public class JudiciaryPersonStagingControllerTests
    {
        private JudiciaryPersonStagingController _controller;
        private Mock<ICommandHandler> _commandHandlerMock;
        private Mock<ILogger<JudiciaryPersonStagingController>> _loggerMock;

        [SetUp]
        public void Setup()
        {
            _commandHandlerMock = new Mock<ICommandHandler>();
            _loggerMock = new Mock<ILogger<JudiciaryPersonStagingController>>();

            _controller = new JudiciaryPersonStagingController(_commandHandlerMock.Object, _loggerMock.Object);
        }

        [Test]
        public void RemoveAllJudiciaryPersonsStaging_Should_Clear_Existing_Entries()
        {
            var result = _controller.RemoveAllJudiciaryPersonsStagingAsync();

            result.Should().NotBeNull();
            var objectResult = (OkResult) result.Result;
            objectResult.StatusCode.Should().Be((int) HttpStatusCode.OK);

            _commandHandlerMock.Verify(c => c.Handle(It.IsAny<RemoveAllJudiciaryPersonStagingCommand>()),
                Times.Exactly(1));
        }

        [Test]
        public async Task BulkJudiciaryPersonsStagingAsync_Returns_Ok_Result_With_Empty_Response_For_Zero_Bulk_Inserts()
        {
            var request = new List<JudiciaryPersonStagingRequest>();

            var result = await _controller.BulkJudiciaryPersonsStagingAsync(request);

            result.Should().NotBeNull();
            var objectResult = result as OkResult;
            objectResult.StatusCode.Should().Be((int) HttpStatusCode.OK);
        }

        [Test]
        public async Task BulkJudiciaryPersonsStagingAsync_Adds_Bulk_User_For_Bulk_Inserts()
        {
            var item1 = new JudiciaryPersonStagingRequest
            {
                Id = Guid.NewGuid().ToString(), Email = "some@email.com", Fullname = "a", Surname = "b", Title = "c",
                KnownAs = "d", PersonalCode = "123", PostNominals = "nom1", Leaver = "leaver", LeftOn = "leftOn", WorkPhone = "01234567890"
            };
            var request = new List<JudiciaryPersonStagingRequest> {item1, new JudiciaryPersonStagingRequest()};

            var result = await _controller.BulkJudiciaryPersonsStagingAsync(request);

            request.Should().NotBeNull();
            var objectResult = result as OkResult;
            objectResult.Should().NotBeNull();
            objectResult.StatusCode.Should().Be((int) HttpStatusCode.OK);
            
            _commandHandlerMock.Verify(c => c.Handle(It.IsAny<AddJudiciaryPersonStagingCommand>()), Times.Exactly(2));
            
            _commandHandlerMock.Verify(c => c.Handle(It.Is<AddJudiciaryPersonStagingCommand>
            (
                c => c.Email == item1.Email && c.Fullname == item1.Fullname && c.Surname == item1.Surname &&
                     c.Title == item1.Title && c.KnownAs == item1.KnownAs && c.PersonalCode == item1.PersonalCode &&
                     c.PostNominals == item1.PostNominals && c.ExternalRefId == item1.Id && c.Leaver == item1.Leaver && c.LeftOn == item1.LeftOn
                     && c.WorkPhone == item1.WorkPhone
            )));
        }

        [Test]
        public async Task BulkJudiciaryPersonsStagingAsync_Continues_To_Next_Item_When_Exception_Is_Thrown()
        {
            var item1 = new JudiciaryPersonStagingRequest
            {
                Id = Guid.NewGuid().ToString(), Email = "some@email.com", Fullname = "a", Surname = "b", Title = "c",
                KnownAs = "d", PersonalCode = "123", PostNominals = "nom1", Leaver = "leaver", LeftOn = "leftOn", WorkPhone = "01234567890"
            };
            _commandHandlerMock.Setup(x => x.Handle(It.IsAny<AddJudiciaryPersonStagingCommand>()))
                .ThrowsAsync(new Exception());
            
            var request = new List<JudiciaryPersonStagingRequest> {item1, new JudiciaryPersonStagingRequest()};
            
            var result = await _controller.BulkJudiciaryPersonsStagingAsync(request);
            
            request.Should().NotBeNull();
            var objectResult = result as OkResult;
            objectResult.Should().NotBeNull();
            objectResult.StatusCode.Should().Be((int) HttpStatusCode.OK);
            _commandHandlerMock.Verify(c => c.Handle(It.IsAny<AddJudiciaryPersonStagingCommand>()), Times.Exactly(2));
        }
    }
}