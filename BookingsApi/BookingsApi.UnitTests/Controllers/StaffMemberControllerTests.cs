using BookingsApi.Contract.Requests;
using BookingsApi.Contract.Responses;
using BookingsApi.Controllers;
using BookingsApi.DAL.Queries;
using BookingsApi.DAL.Queries.Core;
using BookingsApi.Domain;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace BookingsApi.UnitTests.Controllers
{
    public class StaffMemberControllerTests
    {
        protected StaffMemberController _controller;
        protected Mock<IQueryHandler> _queryHandlerMock;

        [Test]
        public async Task GetStaffMemberBySearchTerm_ShouldReturn_List_Of_StaffMembers()
        {   //Arrange
            _queryHandlerMock = new Mock<IQueryHandler>();
            _controller = new StaffMemberController(_queryHandlerMock.Object);

            var searchTermRequest = new SearchTermRequest("staf");
            var staffMembers = new List<Person> {
                                new Person("Mr", "staffffff", "Member", "T Tester") { ContactEmail = "staffff@hmcts.net" },
                                new Person("Mr", "staffer", "Person", "T Test") { ContactEmail = "staffer@hmcts.net" }};
            _queryHandlerMock
             .Setup(x => x.Handle<GetStaffMemberBySearchTermQuery, List<Person>>(It.IsAny<GetStaffMemberBySearchTermQuery>()))
             .ReturnsAsync(staffMembers);

            var result = await _controller.GetStaffMemberBySearchTerm(searchTermRequest);

            result.Should().NotBeNull();
            var objectResult = (ObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var personResponses = (List<PersonResponse>)objectResult.Value;
            personResponses.Count.Should().Be(2);
            personResponses[0].LastName.Should().Be("Person");
            _queryHandlerMock.Verify(x => x.Handle<GetStaffMemberBySearchTermQuery, List<Person>>(It.IsAny<GetStaffMemberBySearchTermQuery>()), Times.Once);
        }
    }
}
