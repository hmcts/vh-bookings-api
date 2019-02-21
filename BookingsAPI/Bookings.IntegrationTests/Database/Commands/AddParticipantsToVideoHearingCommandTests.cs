using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bookings.DAL;
using Bookings.DAL.Commands;
using Bookings.DAL.Exceptions;
using Bookings.DAL.Queries;
using Bookings.Domain;
using Bookings.Domain.Participants;
using Bookings.Domain.RefData;
using Bookings.Domain.Validations;
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Testing.Common.Builders.Domain;

namespace Bookings.IntegrationTests.Database.Commands
{
    public class AddParticipantsToVideoHearingCommandTests : DatabaseTestsBase
    {
        private AddParticipantsToVideoHearingCommandHandler _commandHandler;
        private GetHearingByIdQueryHandler _getHearingByIdQueryHandler;
        private Guid _newHearingId;

        [SetUp]
        public void Setup()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            _commandHandler = new AddParticipantsToVideoHearingCommandHandler(context);
            _getHearingByIdQueryHandler = new GetHearingByIdQueryHandler(context);
            _newHearingId = Guid.Empty;
        }

        [Test]
        public void should_throw_exception_when_hearing_does_not_exist()
        {
            var hearingId = Guid.NewGuid();

            Assert.ThrowsAsync<HearingNotFoundException>(() => _commandHandler.Handle(
                new AddParticipantsToVideoHearingCommand(hearingId, new List<Participant>())));
        }


        [Test]
        public async Task should_add_participants_to_video_hearing()
        {
            var seededHearing = await Hooks.SeedVideoHearing();
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            _newHearingId = seededHearing.Id;

            var beforeCount = seededHearing.GetParticipants().Count;

            var caseTypeName = "Civil Money Claims";
            var caseType = GetCaseTypeFromDb(caseTypeName);

            var claimantCaseRole = caseType.CaseRoles.First(x => x.Name == "Claimant");
            var claimantSolicitorHearingRole = claimantCaseRole.HearingRoles.First(x => x.Name == "Solicitor");

            var newPerson = new PersonBuilder(true).Build();
            var claimantSolicitorParticipant = Builder<Representative>.CreateNew().WithFactory(() =>
                new Representative(newPerson, claimantSolicitorHearingRole, claimantCaseRole)
            ).Build();

            var participants = new List<Participant>()
            {
                claimantSolicitorParticipant
            };

            await _commandHandler.Handle(new AddParticipantsToVideoHearingCommand(_newHearingId, participants));

            var returnedVideoHearing =
                await _getHearingByIdQueryHandler.Handle(new GetHearingByIdQuery(seededHearing.Id));
            var afterCount = returnedVideoHearing.GetParticipants().Count;

            afterCount.Should().BeGreaterThan(beforeCount);
        }

        [Test]
        public async Task should_not_add_existing_participant_to_the_same_video_hearing()
        {
            var seededHearing = await Hooks.SeedVideoHearing();
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            _newHearingId = seededHearing.Id;

            var participants = new List<Participant>()
            {
                seededHearing.GetParticipants().First()
            };

            Assert.ThrowsAsync<DomainRuleException>(() => _commandHandler.Handle(
                new AddParticipantsToVideoHearingCommand(_newHearingId, participants)));
        }

        [Test]
        public async Task should_use_existing_person_when_adding_to_video_hearing()
        {
            var seededHearing = await Hooks.SeedVideoHearing();
            var seededPerson = await SeedPerson();
            
            var personCountBefore = await GetNumberOfPersonsInDb();
            
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            _newHearingId = seededHearing.Id;
            
            var caseTypeName = "Civil Money Claims";
            var caseType = GetCaseTypeFromDb(caseTypeName);

            var claimantCaseRole = caseType.CaseRoles.First(x => x.Name == "Claimant");
            var claimantSolicitorHearingRole = claimantCaseRole.HearingRoles.First(x => x.Name == "Solicitor");

            var claimantSolicitorParticipant = Builder<Representative>.CreateNew().WithFactory(() =>
                new Representative(seededPerson, claimantSolicitorHearingRole, claimantCaseRole)
            ).Build();

            var participants = new List<Participant>()
            {
                claimantSolicitorParticipant
            };

            await _commandHandler.Handle(new AddParticipantsToVideoHearingCommand(_newHearingId, participants));

            var personCountAfter = await GetNumberOfPersonsInDb();

            personCountAfter.Should().Be(personCountBefore);

        }

        private async Task<int> GetNumberOfPersonsInDb()
        {
            using (var db = new BookingsDbContext(BookingsDbContextOptions))
            {
                return await db.Persons.CountAsync();
            }
        }

        private async Task<Person> SeedPerson()
        {
            var newPerson = new PersonBuilder(true).Build();
            using (var db = new BookingsDbContext(BookingsDbContextOptions))
            {
                await db.Persons.AddAsync(newPerson);
                await db.SaveChangesAsync();
            }

            return newPerson;
        }

        private CaseType GetCaseTypeFromDb(string caseTypeName)
        {
            CaseType caseType;
            using (var db = new BookingsDbContext(BookingsDbContextOptions))
            {
                caseType = db.CaseTypes
                    .Include(x => x.CaseRoles)
                    .ThenInclude(x => x.HearingRoles)
                    .ThenInclude(x => x.UserRole)
                    .Include(x => x.HearingTypes)
                    .First(x => x.Name == caseTypeName);
            }

            return caseType;
        }

        [TearDown]
        public async Task TearDown()
        {
            if (_newHearingId != Guid.Empty)
            {
                TestContext.WriteLine($"Removing test hearing {_newHearingId}");
                await Hooks.RemoveVideoHearing(_newHearingId);
            }
        }
    }
}