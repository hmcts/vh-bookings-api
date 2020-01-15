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
            var hearingService = new HearingService(context);
            _commandHandler = new AddParticipantsToVideoHearingCommandHandler(context, hearingService);
            _getHearingByIdQueryHandler = new GetHearingByIdQueryHandler(context);
            _newHearingId = Guid.Empty;
        }

        [Test]
        public void should_throw_exception_when_hearing_does_not_exist()
        {
            var hearingId = Guid.NewGuid();

            Assert.ThrowsAsync<HearingNotFoundException>(() => _commandHandler.Handle(
                new AddParticipantsToVideoHearingCommand(hearingId, new List<NewParticipant>())));
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
            var newParticipant = new NewParticipant()
            {
                Person = newPerson,
                CaseRole = claimantCaseRole,
                HearingRole = claimantSolicitorHearingRole,
                DisplayName = $"{newPerson.FirstName} {newPerson.LastName}",
                SolicitorsReference = string.Empty,
                Representee = string.Empty
            };
            var participants = new List<NewParticipant>()
            {
                newParticipant
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
            var representative = (Representative) seededHearing.GetParticipants().First(x => x.GetType() == typeof(Representative));

            var newParticipant = new NewParticipant()
            {
                Person = representative.Person,
                CaseRole = representative.CaseRole,
                HearingRole = representative.HearingRole,
                DisplayName = representative.DisplayName,
                SolicitorsReference = representative.SolicitorsReference,
                Representee = representative.Representee
            };
            
            var participants = new List<NewParticipant>()
            {
                newParticipant
            };

            Assert.ThrowsAsync<DomainRuleException>(() => _commandHandler.Handle(
                new AddParticipantsToVideoHearingCommand(_newHearingId, participants)));
        }
        
        [Test]
        public async Task should_throw_exception_when_adding_unsupported_role_to_hearing()
        {
            var seededHearing = await Hooks.SeedVideoHearing();
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            _newHearingId = seededHearing.Id;
            var representative = (Representative) seededHearing.GetParticipants().First(x => x.GetType() == typeof(Representative));

            representative.HearingRole.UserRole.Name = "NonExistent";
            var newParticipant = new NewParticipant()
            {
                Person = representative.Person,
                CaseRole = representative.CaseRole,
                HearingRole = representative.HearingRole,
                DisplayName = representative.DisplayName,
                SolicitorsReference = representative.SolicitorsReference,
                Representee = representative.Representee
            };
            
            var participants = new List<NewParticipant>()
            {
                newParticipant
            };

            Assert.ThrowsAsync<DomainRuleException>(() => _commandHandler.Handle(
                new AddParticipantsToVideoHearingCommand(_newHearingId, participants)));
        }

        [Test]
        public async Task should_use_existing_representative_when_adding_to_video_hearing()
        {
            var seededRepresentative = await SeedPerson(true, false);
            var personListBefore = await AddExistingPersonToAHearing(seededRepresentative);
            var personsListAfter = await GetPersonsInDb();
            personsListAfter.Count.Should().Be(personListBefore.Count);

            var existingPersonInDb = personsListAfter.Single(p => p.ContactEmail.Equals(seededRepresentative.ContactEmail));
            existingPersonInDb.Organisation.Name.Should().Be(seededRepresentative.Organisation.Name);
            existingPersonInDb.Address.Should().BeNull();
            CheckPersonDetails(existingPersonInDb, seededRepresentative);
        }

        [Test]
        public async Task should_use_existing_individual_when_adding_to_video_hearing()
        {
            var seededRepresentative = await SeedPerson(true, true);
            var personListBefore = await AddExistingPersonToAHearing(seededRepresentative);
            var personsListAfter = await GetPersonsInDb();
            personsListAfter.Count.Should().Be(personListBefore.Count);

            var existingPersonInDb = personsListAfter.Single(p => p.ContactEmail.Equals(seededRepresentative.ContactEmail));
            existingPersonInDb.Organisation.Should().NotBeNull();
            existingPersonInDb.Organisation.Name.Should().Be(seededRepresentative.Organisation.Name);
            CheckPersonDetails(existingPersonInDb, seededRepresentative);
            existingPersonInDb.Address.Should().NotBeNull();
            CheckAddressDetails(existingPersonInDb.Address, seededRepresentative.Address);
        }

        [Test]
        public async Task should_add_judge_to_video_hearing()
        {
            var seededHearing = await Hooks.SeedVideoHearing();
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            _newHearingId = seededHearing.Id;

            var beforeCount = seededHearing.GetParticipants().Count;

            var caseTypeName = "Civil Money Claims";
            var caseType = GetCaseTypeFromDb(caseTypeName);

            var judgeCaseRole = caseType.CaseRoles.First(x => x.Name == "Judge");
            var judgeHearingRole = judgeCaseRole.HearingRoles.First(x => x.Name == "Judge");

            var newPerson = new PersonBuilder(true).Build();
            var newParticipant = new NewParticipant()
            {
                Person = newPerson,
                CaseRole = judgeCaseRole,
                HearingRole = judgeHearingRole,
                DisplayName = $"{newPerson.FirstName} {newPerson.LastName}",
                SolicitorsReference = string.Empty,
                Representee = string.Empty
            };
            var participants = new List<NewParticipant>()
            {
                newParticipant
            };

            await _commandHandler.Handle(new AddParticipantsToVideoHearingCommand(_newHearingId, participants));

            var returnedVideoHearing =
                await _getHearingByIdQueryHandler.Handle(new GetHearingByIdQuery(seededHearing.Id));
            var afterCount = returnedVideoHearing.GetParticipants().Count;

            afterCount.Should().BeGreaterThan(beforeCount);
        }

        private async Task<List<Person>> AddExistingPersonToAHearing(Person existingPerson)
        {
            var seededHearing = await Hooks.SeedVideoHearing();
            var personListBefore = await GetPersonsInDb();

            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            _newHearingId = seededHearing.Id;

            var caseTypeName = "Civil Money Claims";
            var caseType = GetCaseTypeFromDb(caseTypeName);

            var claimantCaseRole = caseType.CaseRoles.First(x => x.Name == "Claimant");
            var claimantSolicitorHearingRole = claimantCaseRole.HearingRoles.First(x => x.Name == "Solicitor");

            var newParticipant = new NewParticipant()
            {
                Person = existingPerson,
                CaseRole = claimantCaseRole,
                HearingRole = claimantSolicitorHearingRole,
                DisplayName = $"{existingPerson.FirstName} {existingPerson.LastName}",
                SolicitorsReference = string.Empty,
                Representee = string.Empty
            };

            var participants = new List<NewParticipant>()
            {
                newParticipant
            };

            await _commandHandler.Handle(new AddParticipantsToVideoHearingCommand(_newHearingId, participants));

            return personListBefore;
        }

        private void CheckPersonDetails(Person existingPersonInDb, Person seededPerson)
        {
            existingPersonInDb.Title.Should().Be(seededPerson.Title);
            existingPersonInDb.FirstName.Should().Be(seededPerson.FirstName);
            existingPersonInDb.MiddleNames.Should().Be(seededPerson.MiddleNames);
            existingPersonInDb.LastName.Should().Be(seededPerson.LastName);
            existingPersonInDb.ContactEmail.Should().Be(seededPerson.ContactEmail);
            existingPersonInDb.Username.Should().Be(seededPerson.Username);
            existingPersonInDb.TelephoneNumber.Should().Be(seededPerson.TelephoneNumber);
        }

        private void CheckAddressDetails(Address existingPersonAddressInDb, Address seededPersonAddress)
        {
            existingPersonAddressInDb.HouseNumber.Should().Be(seededPersonAddress.HouseNumber);
            existingPersonAddressInDb.Street.Should().Be(seededPersonAddress.Street);
            existingPersonAddressInDb.County.Should().Be(seededPersonAddress.County);
            existingPersonAddressInDb.City.Should().Be(seededPersonAddress.City);
            existingPersonAddressInDb.Postcode.Should().Be(seededPersonAddress.Postcode);
        }

        private async Task<List<Person>> GetPersonsInDb()
        {
            using (var db = new BookingsDbContext(BookingsDbContextOptions))
            {
                return await db.Persons
                    .Include(p => p.Organisation)
                    .Include(p => p.Address)
                    .ToListAsync();
            }
        }
        
        private async Task<Person> SeedPerson(bool withOrganisation = false, bool withAddress = false)
        {
            var builder = new PersonBuilder(true);
            
            if(withAddress)
            {
                builder.WithAddress();
            }
            if (withOrganisation)
            {
                builder.WithOrganisation();
            }

            var newPerson = builder.Build();
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
        public new async Task TearDown()
        {
            if (_newHearingId != Guid.Empty)
            {
                TestContext.WriteLine($"Removing test hearing {_newHearingId}");
                await Hooks.RemoveVideoHearing(_newHearingId);
            }
        }
    }
}