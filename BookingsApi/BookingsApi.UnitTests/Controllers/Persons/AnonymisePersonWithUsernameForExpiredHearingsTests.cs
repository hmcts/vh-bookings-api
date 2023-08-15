using System;
using System.Net;
using System.Threading.Tasks;
using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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
            var username = "user@email.com";
            var exception = new PersonNotFoundException(username);
            
            CommandHandlerMock.Setup(c => c.Handle(It.IsAny<AnonymisePersonWithUsernameCommand>()))
                .ThrowsAsync(exception);

            var response =
                await Controller.AnonymisePersonWithUsernameForExpiredHearings("invalidUsername") as NotFoundResult;

            response.StatusCode.Should().Be((int) HttpStatusCode.NotFound);

            LoggerMock.Verify(x => x.Log(
                It.Is<LogLevel>(log => log == LogLevel.Error),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.Is<Exception>(x => x == exception),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Once);
        }
    }
}