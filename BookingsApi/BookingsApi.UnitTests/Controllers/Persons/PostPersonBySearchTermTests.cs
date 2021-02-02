using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using BookingsApi.Contract.Requests;
using BookingsApi.Contract.Responses;
using Bookings.Domain;
using BookingsApi.DAL.Queries;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace BookingsApi.UnitTests.Controllers.Persons
{
    public class PostPersonBySearchTermTests : PersonsControllerTest
    {
        [Test]
        public async Task Should_return_list_of_PersonResponse_successfully()
        {
            var searchTermRequest = new SearchTermRequest("test");
            var persons = new List<Person> { 
                                new Person("Mr", "Test", "Tester", "T Tester") { ContactEmail = "test@tester.com" },
                                new Person("Mr", "Tester", "Test", "T Test") { ContactEmail = "atest@tester.com" }};
            QueryHandlerMock
           .Setup(x => x.Handle<GetPersonBySearchTermQuery, List<Person>>(It.IsAny<GetPersonBySearchTermQuery>()))
           .ReturnsAsync(persons);

            var result = await Controller.PostPersonBySearchTerm(searchTermRequest);

            result.Should().NotBeNull();
            var objectResult = (ObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var personResponses = (List<PersonResponse>)objectResult.Value;
            personResponses.Count.Should().Be(2);
            personResponses[0].LastName.Should().Be("Test");
            QueryHandlerMock.Verify(x => x.Handle<GetPersonBySearchTermQuery, List<Person>>(It.IsAny<GetPersonBySearchTermQuery>()), Times.Once);
        }
    }
}
