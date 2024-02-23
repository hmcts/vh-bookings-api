using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace BookingsApi.UnitTests.Controllers.Persons;

public class DeleteTestPersonsControllerTest : PersonsControllerTest
{

    [Test]
    public async Task should_return_not_found_when_command_throws_person_not_found_exception()
    {
        var username = "person";
        CommandHandlerMock.Setup(x => x.Handle(It.IsAny<DeleteTestPersonCommand>()))
            .ThrowsAsync(new PersonNotFoundException(username));
        var result = await Controller.DeleteTestPersonByUsername(username);
        result.Should().BeOfType<NotFoundResult>();
    }

    [Test]
    public async Task should_return_forbidden_when_command_throws_argument_exception()
    {
        var username = "person";
        CommandHandlerMock.Setup(x => x.Handle(It.IsAny<DeleteTestPersonCommand>()))
            .ThrowsAsync(new ArgumentException(username));
        var result = await Controller.DeleteTestPersonByUsername(username);
        result.Should().BeOfType<ForbidResult>();
    }
    
    [Test]
    public async Task should_return_no_content_when_command_is_successful()
    {
        var username = "person";
        var result = await Controller.DeleteTestPersonByUsername(username);
        result.Should().BeOfType<NoContentResult>();
    }
}