using System.Net;
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
    }
}