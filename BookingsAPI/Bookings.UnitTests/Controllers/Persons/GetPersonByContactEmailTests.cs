using Bookings.Api.Contract.Responses;
using Bookings.DAL.Queries;
using Bookings.Domain;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System.Net;
using System.Threading.Tasks;
using Testing.Common.Assertions;

namespace Bookings.UnitTests.Controllers
{
    public class GetPersonByContactEmailTests : PersonsControllerTest
    {
        [Test]
        public async Task Should_return_badrequest_for_given_invalid_contact_email()
        {
            var contactEmail = string.Empty;

            var result = await _controller.GetPersonByContactEmail(contactEmail);

            result.Should().NotBeNull();
            var objectResult = (BadRequestObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            ((SerializableError)objectResult.Value).ContainsKeyAndErrorMessage(nameof(contactEmail), $"Please provide a valid {nameof(contactEmail)}");
        }

        [Test]
        public async Task Should_return_notfound_with_no_matching_person()
        {
            var contactEmail = "test@user.com";
            _queryHandlerMock
           .Setup(x => x.Handle<GetPersonByContactEmailQuery, Person>(It.IsAny<GetPersonByContactEmailQuery>()))
           .ReturnsAsync((Person) null);

            var result = await _controller.GetPersonByContactEmail(contactEmail);

            result.Should().NotBeNull();
            var objectResult = (NotFoundResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
            _queryHandlerMock.Verify(x => x.Handle<GetPersonByContactEmailQuery, Person>(It.IsAny<GetPersonByContactEmailQuery>()), Times.Once);
        }

        [Test]
        public async Task Should_return_PersonResponse_successfully()
        {
            var contactEmail = "test@user.com";
            _queryHandlerMock
           .Setup(x => x.Handle<GetPersonByContactEmailQuery, Person>(It.IsAny<GetPersonByContactEmailQuery>()))
           .ReturnsAsync(new Person("Mr","Test","Tester","T Tester"));

            var result = await _controller.GetPersonByContactEmail(contactEmail);

            result.Should().NotBeNull();
            var objectResult = (ObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var personResponse = (PersonResponse)objectResult.Value;
            personResponse.Should().NotBeNull();
            personResponse.LastName.Should().Be("Tester");
            _queryHandlerMock.Verify(x => x.Handle<GetPersonByContactEmailQuery, Person>(It.IsAny<GetPersonByContactEmailQuery>()), Times.Once);
        }
    }
}
