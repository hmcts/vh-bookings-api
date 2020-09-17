﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Bookings.DAL;
using Bookings.DAL.Commands;
using Bookings.DAL.Exceptions;
using Bookings.DAL.Queries;
using FluentAssertions;
using NUnit.Framework;

namespace Bookings.IntegrationTests.Database.Commands
{
    public class UpdateEndPointOfHearingCommandTests : DatabaseTestsBase
    {
        private UpdateEndPointOfHearingCommandHandler _commandHandler;
        private GetHearingByIdQueryHandler _getHearingByIdQueryHandler;
        private Guid _newHearingId;

        [SetUp]
        public void Setup()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            _commandHandler = new UpdateEndPointOfHearingCommandHandler(context);
            _getHearingByIdQueryHandler = new GetHearingByIdQueryHandler(context);
            _newHearingId = Guid.Empty;
        }

        [TearDown]
        public new async Task TearDown()
        {
            if (_newHearingId != Guid.Empty)
            {
                TestContext.WriteLine($"Removing test hearing {_newHearingId}");
                await Hooks.RemoveVideoHearing(_newHearingId);
            }
        }

        [Test]
        public void Should_throw_exception_when_hearing_does_not_exist()
        {
            var hearingId = Guid.NewGuid();

            Assert.ThrowsAsync<HearingNotFoundException>(() => _commandHandler.Handle(
                new UpdateEndPointOfHearingCommand(hearingId, Guid.NewGuid(), "DP", null)));
        }

        [Test]
        public async Task Should_throw_exception_when_endpoint_does_not_exist()
        {
            var seededHearing = await Hooks.SeedVideoHearing();
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            _newHearingId = seededHearing.Id;

            Assert.ThrowsAsync<EndPointNotFoundException>(() => _commandHandler.Handle(
                new UpdateEndPointOfHearingCommand(seededHearing.Id, Guid.NewGuid(), "DP", null)));
        }

        [Test]
        public async Task Should_throw_exception_when_display_name_is_null()
        {
            var seededHearing = await Hooks.SeedVideoHearing();
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            _newHearingId = seededHearing.Id;

            Assert.ThrowsAsync<ArgumentNullException>(() => _commandHandler.Handle(
                new UpdateEndPointOfHearingCommand(seededHearing.Id, seededHearing.GetEndpoints().First().Id, string.Empty, null)));
        }

        [Test]
        public async Task Should_update_endpoint_display_name()
        {
            var seededHearing = await Hooks.SeedVideoHearing();
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            _newHearingId = seededHearing.Id;

            var endpoint = seededHearing.GetEndpoints().First();
            var updatedDisplayName = "updatedDisplayName";
            await _commandHandler.Handle(new UpdateEndPointOfHearingCommand(seededHearing.Id, endpoint.Id, updatedDisplayName, null));


            var returnedVideoHearing =
                await _getHearingByIdQueryHandler.Handle(new GetHearingByIdQuery(seededHearing.Id));
            var updatedEndPoint = returnedVideoHearing.GetEndpoints().First(ep => ep.Id == endpoint.Id);
            updatedEndPoint.DisplayName.Should().Be(updatedDisplayName);
        }
        
        [Test]
        public async Task Should_update_endpoint_with_defence_advocate()
        {
            var seededHearing = await Hooks.SeedVideoHearing();
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            _newHearingId = seededHearing.Id;

            var endpoint = seededHearing.GetEndpoints().First();
            var dA = seededHearing.Participants.First(x => x.HearingRole.UserRole.IsRepresentative);
            var updatedDisplayName = "updatedDisplayName";
            await _commandHandler.Handle(new UpdateEndPointOfHearingCommand(seededHearing.Id, endpoint.Id, updatedDisplayName, dA));


            var returnedVideoHearing =
                await _getHearingByIdQueryHandler.Handle(new GetHearingByIdQuery(seededHearing.Id));
            var updatedEndPoint = returnedVideoHearing.GetEndpoints().First(ep => ep.Id == endpoint.Id);
            updatedEndPoint.DisplayName.Should().Be(updatedDisplayName);
            updatedEndPoint.DefenceAdvocate.Should().NotBeNull();
            updatedEndPoint.DefenceAdvocate.Id.Should().Be(dA.Id);
        }

        [Test]
        public async Task Should_update_endpoint_with_defence_advocate_as_null_for_none()
        {
            var seededHearing = await Hooks.SeedVideoHearing();
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            _newHearingId = seededHearing.Id;

            var endpoint = seededHearing.GetEndpoints().FirstOrDefault(ep => ep.DefenceAdvocate != null);
            var updatedDisplayName = "updatedDisplayName";
            await _commandHandler.Handle(new UpdateEndPointOfHearingCommand(seededHearing.Id, endpoint.Id, updatedDisplayName, null));
            var returnedVideoHearing =
                await _getHearingByIdQueryHandler.Handle(new GetHearingByIdQuery(seededHearing.Id));
            var updatedEndPoint = returnedVideoHearing.GetEndpoints().First(ep => ep.Id == endpoint.Id);
            updatedEndPoint.DisplayName.Should().Be(updatedDisplayName);
            updatedEndPoint.DefenceAdvocate.Should().BeNull();
        }
    }
}
