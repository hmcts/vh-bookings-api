using System.Threading.Tasks;
using BookingsApi.DAL.Commands.V1;
using BookingsApi.DAL.Exceptions;
using BookingsApi.DAL.Queries.V1;
using BookingsApi.Domain;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Testing.Common.Builders.Domain;

namespace BookingsApi.UnitTests.Controllers.V1.Persons
{
    public class UpdatePersonUsernameTests : PersonsControllerTest
    {
        [Test]
        public async Task should_return_bad_request_when_contactEmail_invalid()
        {
            var contactEmail = string.Empty;
            var username = "me@me.com";
            var result = await Controller.UpdatePersonUsername(contactEmail, username);
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Test]
        public async Task should_return_bad_request_when_username_invalid()
        {
            var contactEmail = "me@me.com";
            var username = "me";
            var result = await Controller.UpdatePersonUsername(contactEmail, username);
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Test]
        public async Task should_return_not_found_when_command_throws_person_not_found_exception()
        {
            var contactEmail = "me@me.com";
            var username = "me@me.com";

            CommandHandlerMock.Setup(x => x.Handle(It.IsAny<UpdatePersonUsernameCommand>()))
                .ThrowsAsync(new PersonNotFoundException(contactEmail));
            var result = await Controller.UpdatePersonUsername(contactEmail, username);
            result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        public async Task should_return_nocontent_when_username_updated()
        {
            var contactEmail = "me@me.com";
            var username = "me@me1.com";

            var hearing1 = new VideoHearingBuilder().WithCase().Build();
            hearing1.Participants[0].Person.ContactEmail = contactEmail;

            QueryHandlerMock
                .Setup(x =>
                    x.Handle<GetPersonByContactEmailQuery, Person>(It.IsAny<GetPersonByContactEmailQuery>()))
                .ReturnsAsync(hearing1.Participants[0].Person);
            
            var result = await Controller.UpdatePersonUsername(contactEmail, username);

            result.Should().BeOfType<NoContentResult>();
        }
    }
}