using Bookings.API.Controllers;
using Bookings.DAL.Commands.Core;
using Bookings.DAL.Queries;
using Bookings.DAL.Queries.Core;
using Bookings.Domain.RefData;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;


namespace Bookings.UnitTests.Controllers
{
    public class HearingsControllerTest
    {
        private HearingsController _controller;
        private Mock<IQueryHandler> _quryHandlerMock;
        private Mock<IQueryHandler> _quryHandlerCaseTypesMock;
        private Mock<ICommandHandler> _commandHandlerMock;

        [SetUp]
        public void Setup()
        {
            _quryHandlerMock = new Mock<IQueryHandler>();
            _quryHandlerCaseTypesMock = new Mock<IQueryHandler>();
            _commandHandlerMock = new Mock<ICommandHandler>();
            _controller = new HearingsController(_quryHandlerMock.Object, _commandHandlerMock.Object);
        }

        [Test]
        public async Task should_return_bad_request_if_invalid_case_types()
        {
            var caseTypes = new List<int> { 44, 78 };
            _quryHandlerCaseTypesMock
             .Setup(x => x.Handle<GetAllCaseTypesQuery, List<CaseType>>(It.IsAny<GetAllCaseTypesQuery>()))
             .ReturnsAsync(new List<CaseType>());

            var result = await _controller.GetHearingsByTypes(caseTypes, "0", 2);

            result.Should().NotBeNull();
            var objectResult = result.Result as ObjectResult;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        }
    }
}
