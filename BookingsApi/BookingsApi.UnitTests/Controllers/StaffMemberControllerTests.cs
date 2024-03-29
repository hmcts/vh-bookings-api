﻿using BookingsApi.DAL.Queries;
using BookingsApi.DAL.Queries.Core;
using BookingsApi.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using BookingsApi.Contract.V1.Responses;
using BookingsApi.Controllers.V1;
using Testing.Common.Assertions;

namespace BookingsApi.UnitTests.Controllers
{
    public class StaffMemberControllerTests
    {
        protected StaffMemberController _controller;
        protected Mock<IQueryHandler> _queryHandlerMock;

        [SetUp]
        public void Setup()
        {
            _queryHandlerMock = new Mock<IQueryHandler>();
            _controller = new StaffMemberController(_queryHandlerMock.Object);
        }

        [Test]
        public async Task GetStaffMemberBySearchTerm_ShouldReturn_List_Of_StaffMembers()
        {   
            //Arrange
            var searchTermRequest = "staf";
            var staffMembers = new List<Person> {
                                new Person("Mr", "staffffff", "Member", "staffff@hmcts.net", "T Tester"),
                                new Person("Mr", "staffer", "Person","staffer@hmcts.net", "T Test")};
            _queryHandlerMock
             .Setup(x => x.Handle<GetStaffMemberBySearchTermQuery, List<Person>>(It.IsAny<GetStaffMemberBySearchTermQuery>()))
             .ReturnsAsync(staffMembers);

            var result = await _controller.GetStaffMemberBySearchTerm(searchTermRequest);

            result.Should().NotBeNull();
            var objectResult = (ObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            result.Should().BeOfType<OkObjectResult>();
            var personResponses = (List<PersonResponse>)objectResult.Value;
            personResponses.Count.Should().Be(2);
            personResponses[0].LastName.Should().Be("Person");
            _queryHandlerMock.Verify(x => x.Handle<GetStaffMemberBySearchTermQuery, List<Person>>(It.IsAny<GetStaffMemberBySearchTermQuery>()), Times.Once);
        }

        [Test]
        public async Task GetStaffMemberBySearchTerm_Return_BadRequest_When_SearchTerm_LessThan_3_Char()
        {
            var result = await _controller.GetStaffMemberBySearchTerm("hh");
            var objectResult = (ObjectResult)result;
            objectResult.Should().NotBeNull();
            ((ValidationProblemDetails)objectResult.Value).ContainsKeyAndErrorMessage("term", "Search term must be at least 3 characters.");
        }

        [Test]
        public async Task GetStaffMemberBySearchTerm_Return_NotFound_When_NoStaffMember_Has_SearchTerm()
        {
            _queryHandlerMock
             .Setup(x => x.Handle<GetStaffMemberBySearchTermQuery, List<Person>>(It.IsAny<GetStaffMemberBySearchTermQuery>()))
             .ReturnsAsync(new List<Person>());
            var result = await _controller.GetStaffMemberBySearchTerm("hhfff");
            result.Should().BeOfType<NotFoundResult>();
        }
    }
}
