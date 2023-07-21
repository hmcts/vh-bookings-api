﻿using System.Net;
using System.Threading.Tasks;
using BookingsApi.Contract.Responses;
using BookingsApi.DAL.Queries;
using BookingsApi.Domain;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Testing.Common.Assertions;

namespace BookingsApi.UnitTests.Controllers.V1.Persons
{
    public class GetPersonByUsernameTests : PersonsControllerTest
    {
        [Test]
        public async Task Should_return_badrequest_for_invalid_username()
        {
            var username = string.Empty;

            var result = await Controller.GetPersonByUsername(username);

            result.Should().NotBeNull();
            var objectResult = (BadRequestObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            ((SerializableError)objectResult.Value).ContainsKeyAndErrorMessage(nameof(username), $"Please provide a valid {nameof(username)}");
        }

        [Test]
        public async Task Should_return_notfound_with_no_matching_person()
        {
            var username = "test@hmcts.net";
            QueryHandlerMock
           .Setup(x => x.Handle<GetPersonByUsernameQuery, Person>(It.IsAny<GetPersonByUsernameQuery>()))
           .ReturnsAsync((Person)null);

            var result = await Controller.GetPersonByUsername(username);

            result.Should().NotBeNull();
            var objectResult = (NotFoundResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
            QueryHandlerMock.Verify(x => x.Handle<GetPersonByUsernameQuery, Person>(It.IsAny<GetPersonByUsernameQuery>()), Times.Once);
        }

        [Test]
        public async Task Should_return_PersonResponse_successfully()
        {
            var username = "test@hmcts.net";
            QueryHandlerMock
           .Setup(x => x.Handle<GetPersonByUsernameQuery, Person>(It.IsAny<GetPersonByUsernameQuery>()))
           .ReturnsAsync(new Person("Mr", "Test", "Tester", "T Tester", "T Tester"));

            var result = await Controller.GetPersonByUsername(username);

            result.Should().NotBeNull();
            var objectResult = (ObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var personResponse = (PersonResponse)objectResult.Value;
            personResponse.Should().NotBeNull();
            personResponse.LastName.Should().Be("Tester");
            QueryHandlerMock.Verify(x => x.Handle<GetPersonByUsernameQuery, Person>(It.IsAny<GetPersonByUsernameQuery>()), Times.Once);
        }
    }
}
