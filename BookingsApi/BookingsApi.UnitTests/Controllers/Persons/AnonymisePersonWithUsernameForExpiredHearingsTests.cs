using System.Net;
using System.Threading.Tasks;
using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Exceptions;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace BookingsApi.UnitTests.Controllers.Persons
{
    public class AnonymisePersonWithUsernameForExpiredHearingsTests : PersonsControllerTest
    {
        [Test]
        public async Task AnonymisePersonWithUsernameForExpiredHearings_Returns_Ok_Status_Code()
        {
            var usernameToAnonymise = "john.doe@email.net";
            var response =
                await Controller.AnonymisePersonWithUsernameForExpiredHearings(usernameToAnonymise) as OkResult;

            response.StatusCode.Should().Be((int) HttpStatusCode.OK);
            CommandHandlerMock
                .Verify(c =>
                        c.Handle(It.Is<AnonymisePersonWithUsernameCommand>(command =>
                            command.Username == usernameToAnonymise)),
                    Times.Once);
        }

        [Test]
        public async Task AnonymisePersonWithUsernameForExpiredHearings_Returns_Not_Found_For_Invalid_Username()
        {
            CommandHandlerMock.Setup(c => c.Handle(It.IsAny<AnonymisePersonWithUsernameCommand>()))
                .ThrowsAsync(new PersonNotFoundException(string.Empty));

            var response =
                await Controller.AnonymisePersonWithUsernameForExpiredHearings("invalidUsername") as NotFoundResult;

            response.StatusCode.Should().Be((int) HttpStatusCode.NotFound);
        }
    }
}