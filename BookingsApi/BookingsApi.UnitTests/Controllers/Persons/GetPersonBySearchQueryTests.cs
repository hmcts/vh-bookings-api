using BookingsApi.Contract.Requests;
using BookingsApi.Contract.Responses;
using BookingsApi.DAL.Queries;
using BookingsApi.Domain;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BookingsApi.UnitTests.Controllers.Persons
{
    public class GetPersonBySearchQueryTests : PersonsControllerTest
    {
        [Test]
        public async Task Returns_List_Of_PersonResponse_Without_Judiciary_Persons()
        {
            var searchQueryRequest = new SearchQueryRequest { Term = "search_term" };
            var persons = new List<Person> {
                                new Person("Mr", "Test", "Tester", "T Tester") { ContactEmail = "test@hmcts.net" },
                                new Person("Mr", "Tester", "Test", "T Test") { ContactEmail = "atest@hmcts.net" }};
            QueryHandlerMock
                .Setup(x => 
                x.Handle<GetPersonBySearchTermExcludingJudiciaryPersonsQuery, List<Person>>(It.Is<GetPersonBySearchTermExcludingJudiciaryPersonsQuery>
                    (x => x.Term == searchQueryRequest.Term && 
                    x.JudiciaryUsersFromAD == searchQueryRequest.JudiciaryUsernamesFromAd))).ReturnsAsync(persons);

            var result = await Controller.GetPersonBySearchQuery(searchQueryRequest);

            result.Should().NotBeNull();
            var objectResult = (ObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var personResponses = (List<PersonResponse>)objectResult.Value;
            personResponses.Count.Should().Be(2);
            personResponses[0].LastName.Should().Be("Test");

        }
    }
}
