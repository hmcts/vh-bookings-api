using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Dtos;
using BookingsApi.DAL.Exceptions;
using BookingsApi.DAL.Queries;
using BookingsApi.DAL.Services;
using BookingsApi.Domain.Enumerations;

namespace BookingsApi.IntegrationTests.Database.Commands
{
    public class UpdateEndPointOfHearingCommandTests : DatabaseTestsBase
    {
        private UpdateEndPointOfHearingCommandHandler _commandHandler;
        private GetHearingByIdQueryHandler _getHearingByIdQueryHandler;

        [SetUp]
        public void Setup()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            _commandHandler = new UpdateEndPointOfHearingCommandHandler(context, new HearingService(context));
            _getHearingByIdQueryHandler = new GetHearingByIdQueryHandler(context);
        }

        [Test]
        public void Should_throw_exception_when_hearing_does_not_exist()
        {
            var hearingId = Guid.NewGuid();

            Assert.ThrowsAsync<HearingNotFoundException>(() => _commandHandler.Handle(
                new UpdateEndPointOfHearingCommand(hearingId, Guid.NewGuid(), "DP", null, null, null, null,
                    Guid.NewGuid().ToString(), null)));
        }

        [Test]
        public async Task Should_throw_exception_when_endpoint_does_not_exist()
        {
            var seededHearing = await Hooks.SeedVideoHearingV2();
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");

            Assert.ThrowsAsync<EndPointNotFoundException>(() => _commandHandler.Handle(
                new UpdateEndPointOfHearingCommand(seededHearing.Id, Guid.NewGuid(), "DP", null, null, null, null,
                    Guid.NewGuid().ToString(), null)));
        }

        [Test]
        public async Task Should_throw_exception_when_display_name_is_null()
        {
            var seededHearing = await Hooks.SeedVideoHearingV2();
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");

            Assert.ThrowsAsync<ArgumentNullException>(() => _commandHandler.Handle(
                new UpdateEndPointOfHearingCommand(seededHearing.Id, seededHearing.GetEndpoints()[0].Id, string.Empty,
                    null, null, null, null, Guid.NewGuid().ToString(), null)));
        }

        [Test]
        public async Task Should_update_endpoint_display_name()
        {
            var seededHearing = await Hooks.SeedVideoHearingV2();
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");

            var endpoint = seededHearing.GetEndpoints()[0];
            var updatedDisplayName = "updatedDisplayName";
            var externalReferenceId = Guid.NewGuid().ToString();
            var measuresExternalId = Guid.NewGuid().ToString();
            var command = new UpdateEndPointOfHearingCommand(seededHearing.Id, endpoint.Id,
                updatedDisplayName, null, null, null, null, externalReferenceId, measuresExternalId);
            var originalUpdatedDate = endpoint.UpdatedDate;
            
            await _commandHandler.Handle(command);

            var returnedVideoHearing =
                await _getHearingByIdQueryHandler.Handle(new GetHearingByIdQuery(seededHearing.Id));
            var updatedEndPoint = returnedVideoHearing.GetEndpoints().First(ep => ep.Id == endpoint.Id);
            updatedEndPoint.DisplayName.Should().Be(updatedDisplayName);
            updatedEndPoint.UpdatedDate.Should().BeAfter(originalUpdatedDate);
            updatedEndPoint.ExternalReferenceId.Should().Be(externalReferenceId);
            updatedEndPoint.MeasuresExternalId.Should().Be(measuresExternalId);
            command.UpdatedEndpoint.Id.Should().Be(updatedEndPoint.Id);
        }
        
        [Test]
        public async Task Should_update_endpoint_with_defence_advocate()
        {
            var seededHearing = await Hooks.SeedVideoHearingV2();
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            
            var endpoint = seededHearing.GetEndpoints()[0];
            var dA = seededHearing.Participants.First(x => x.HearingRole.UserRole.IsRepresentative);
            var updatedDisplayName = "updatedDisplayName";
            await _commandHandler.Handle(new UpdateEndPointOfHearingCommand(seededHearing.Id, endpoint.Id, updatedDisplayName, [dA], null, null, null, Guid.NewGuid().ToString(), null));


            var returnedVideoHearing = await _getHearingByIdQueryHandler.Handle(new GetHearingByIdQuery(seededHearing.Id));
            var updatedEndPoint = returnedVideoHearing.GetEndpoints().First(ep => ep.Id == endpoint.Id);
            updatedEndPoint.DisplayName.Should().Be(updatedDisplayName);
            updatedEndPoint.ParticipantsLinked.Should().NotBeEmpty();
            updatedEndPoint.ParticipantsLinked.Should().Contain(e => e.Id == dA.Id);
            endpoint.CreatedDate.Should().Be(updatedEndPoint.CreatedDate);
            updatedEndPoint.UpdatedDate.Should().BeAfter(updatedEndPoint.CreatedDate);
        }

        [Test]
        public async Task Should_update_endpoint_with_defence_advocate_as_null_for_none()
        {
            var seededHearing = await Hooks.SeedVideoHearingV2();
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");

            var endpoint = seededHearing.GetEndpoints()[0];
            var dA = seededHearing.Participants.First(x => x.HearingRole.UserRole.IsRepresentative);
            var updatedDisplayName = "updatedDisplayName";
            await _commandHandler.Handle(new UpdateEndPointOfHearingCommand(seededHearing.Id,
                endpoint.Id,
                updatedDisplayName,
                [dA], 
                null, 
                null, 
                null, 
                Guid.NewGuid().ToString(), 
                null));

            var returnedVideoHearing =
                await _getHearingByIdQueryHandler.Handle(new GetHearingByIdQuery(seededHearing.Id));
            var updatedEndPoint = returnedVideoHearing.GetEndpoints().First(ep => ep.Id == endpoint.Id);
            updatedEndPoint.DisplayName.Should().Be(updatedDisplayName);
            updatedEndPoint.ParticipantsLinked.Should().NotBeEmpty();
            updatedEndPoint.ParticipantsLinked.Should().Contain(e => e.Id == dA.Id);

            await _commandHandler.Handle(new UpdateEndPointOfHearingCommand(seededHearing.Id,
                endpoint.Id,
                updatedDisplayName, 
                null, 
                null, 
                null, 
                null, 
                Guid.NewGuid().ToString(), 
                null));
            
            returnedVideoHearing = await _getHearingByIdQueryHandler.Handle(new GetHearingByIdQuery(seededHearing.Id));
            updatedEndPoint = returnedVideoHearing.GetEndpoints().First(ep => ep.Id == endpoint.Id);
            updatedEndPoint.DisplayName.Should().Be(updatedDisplayName);
            updatedEndPoint.ParticipantsLinked.Should().BeEmpty();
        }

        [Test]
        public async Task Should_not_update_endpoint_when_it_has_not_changed()
        {
            // Arrange
            var seededHearing = await Hooks.SeedVideoHearingV2(options =>
            {
                options.AddScreening = true;
                options.AddInterpreterLanguages = true;
            }, BookingStatus.Created);
            var endpoint = seededHearing.GetEndpoints()[0];
            var screeningDto = new ScreeningDto
            {
                ScreeningType = endpoint.Screening.Type,
                ProtectedFrom = endpoint.Screening.ScreeningEntities.Select(e => e.Participant.ExternalReferenceId).ToList()
            };
            var originalUpdatedDate = endpoint.UpdatedDate;

            var command = new UpdateEndPointOfHearingCommand(
                seededHearing.Id,
                endpoint.Id,
                endpoint.DisplayName,
                endpoint.ParticipantsLinked,
                endpoint.InterpreterLanguage.Code,
                endpoint.OtherLanguage,
                screeningDto,
                endpoint.ExternalReferenceId,
                endpoint.MeasuresExternalId);
            
            // Act
            await _commandHandler.Handle(command);
            
            // Assert
            var updatedEndpoint = await _getHearingByIdQueryHandler.Handle(new GetHearingByIdQuery(seededHearing.Id));
            updatedEndpoint.UpdatedDate.Should().BeCloseTo(originalUpdatedDate, TimeSpan.FromMilliseconds(50)); // The two dates should be the same, but there is a small discrepancy with EF
            command.UpdatedEndpoint.Should().BeNull();
        }
    }
}
