using BookingsApi.Contract;
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
    public class GetPersonBySearchTermAndAccountTypeTests : PersonsControllerTest
    {
        [Test]
        public async Task Should_return_list_of_PersonResponse_successfully()
        {
            var searchTermRequest = new SearchTermAndAccountTypeRequest("term") { AccountType = new List<string>()};
            var persons = new List<Person> {
                                new Person("Mr", "Test", "Tester", "T Tester") { ContactEmail = "test@hmcts.net" },
                                new Person("Mr", "Tester", "Test", "T Test") { ContactEmail = "atest@hmcts.net" }};
            QueryHandlerMock
           .Setup(x => x.Handle<GetPersonBySearchTermAndAccountTypeQuery, List<Person>>(It.Is<GetPersonBySearchTermAndAccountTypeQuery>(x => x.AccountType == searchTermRequest.AccountType && x.Term == searchTermRequest.Term)))
           .ReturnsAsync(persons);

            var result = await Controller.GetPersonBySearchTermAndAccountType(searchTermRequest);

            result.Should().NotBeNull();
            var objectResult = (ObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var personResponses = (List<PersonResponse>)objectResult.Value;
            personResponses.Count.Should().Be(2);
            personResponses[0].LastName.Should().Be("Test");
            QueryHandlerMock.Verify(x => x.Handle<GetPersonBySearchTermAndAccountTypeQuery, List<Person>>(It.Is<GetPersonBySearchTermAndAccountTypeQuery>(x => x.AccountType == searchTermRequest.AccountType && x.Term == searchTermRequest.Term)), Times.Once);
        }
    }
}
