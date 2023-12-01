using BookingsApi.Controllers.V1;
using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Commands.Core;
using BookingsApi.DAL.Queries.Core;
using BookingsApi.Domain.Validations;
using BookingsApi.Infrastructure.Services.IntegrationEvents;

namespace BookingsApi.UnitTests.Controllers;


public class JudiciaryParticipantsControllerTests
{
    private readonly Mock<ICommandHandler> _mockCommandHandler;
    private readonly Mock<IQueryHandler> _mockQueryHandler;
    private readonly Mock<IEventPublisher> _mockEventPublisher;
    private readonly JudiciaryParticipantsController _controller;

    public JudiciaryParticipantsControllerTests()
    {
        _mockQueryHandler = new Mock<IQueryHandler>();
        _mockCommandHandler = new Mock<ICommandHandler>();
        _mockEventPublisher = new Mock<IEventPublisher>();
        _controller = new JudiciaryParticipantsController(_mockQueryHandler.Object, _mockCommandHandler.Object, _mockEventPublisher.Object);
    }
    
    [Test]
    public void Should_throw_domain_rule_exception_when_calling_RemoveJudiciaryParticipantFromHearingCommand_handler()
    {
        _mockCommandHandler
            .Setup(x => x.Handle(It.IsAny<RemoveJudiciaryParticipantFromHearingCommand>()))
            .ThrowsAsync(new DomainRuleException("Hearing",DomainRuleErrorMessages.CannotEditACancelledHearing));

        Assert.ThrowsAsync<DomainRuleException>(
            () => _controller.RemoveJudiciaryParticipantFromHearing(Guid.NewGuid(), "personalCode"), DomainRuleErrorMessages.CannotEditACancelledHearing);    
    }
}