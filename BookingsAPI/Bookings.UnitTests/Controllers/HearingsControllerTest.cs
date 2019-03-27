using Bookings.API.Controllers;
using Bookings.DAL.Commands.Core;
using Bookings.DAL.Queries;
using Bookings.DAL.Queries.Core;
using Bookings.Domain;
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
        private Mock<IQueryHandler> _queryHandlerMock;
        private Mock<IQueryHandler> _queryHandlerCaseTypesMock;
        private Mock<ICommandHandler> _commandHandlerMock;

        [SetUp]
        public void Setup()
        {
            _queryHandlerMock = new Mock<IQueryHandler>();
            _queryHandlerCaseTypesMock = new Mock<IQueryHandler>();
            _commandHandlerMock = new Mock<ICommandHandler>();
            _controller = new HearingsController(_queryHandlerMock.Object, _commandHandlerMock.Object);
        }

        [Test]
        public async Task should_return_bad_request_if_invalid_case_types()
        {
            var caseTypes = new List<int> { 44, 78 };
            _queryHandlerCaseTypesMock
             .Setup(x => x.Handle<GetAllCaseTypesQuery, List<CaseType>>(It.IsAny<GetAllCaseTypesQuery>()))
             .ReturnsAsync(new List<CaseType>());

            var result = await _controller.GetHearingsByTypes(caseTypes, "0", 2);

            result.Should().NotBeNull();
            var objectResult = (ObjectResult) result.Result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task should_return_bookings()
        {
            var caseTypes = new List<int>();
            _queryHandlerCaseTypesMock
             .Setup(x => x.Handle<GetAllCaseTypesQuery, List<CaseType>>(It.IsAny<GetAllCaseTypesQuery>()))
             .ReturnsAsync(new List<CaseType>());

            _queryHandlerMock
                .Setup(x => x.Handle<GetBookingsByCaseTypesQuery, CursorPagedResult<VideoHearing, string>>(It.IsAny<GetBookingsByCaseTypesQuery>()))
                .ReturnsAsync(new CursorPagedResult<VideoHearing, string>(new List<VideoHearing>(), "0", "next cursor"));
            var result = await _controller.GetHearingsByTypes(caseTypes, "0", 2);

            result.Should().NotBeNull();
            var objectResult = (ObjectResult) result.Result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }
    }
}
