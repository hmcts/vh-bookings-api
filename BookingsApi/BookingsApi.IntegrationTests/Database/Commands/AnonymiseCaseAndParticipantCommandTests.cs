using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.DAL;
using BookingsApi.DAL.Commands;
using BookingsApi.Domain;
using BookingsApi.Domain.Participants;
using BookingsApi.Domain.RefData;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace BookingsApi.IntegrationTests.Database.Commands
{
    public class AnonymiseCaseAndParticipantCommandTests
    {
        private AnonymiseCaseAndParticipantCommandHandler _command;
        private BookingsDbContext _context;
        private Person _person1, _person2, _representativePerson, _anonymisedParticipantPerson;
        private VideoHearing _hearing1, _hearing2, _hearing3;
        private CaseType _caseType1;
        private UserRole _userRole1, _userRole2;
        private HearingRole _hearingRole1, _hearingRole2;

        [OneTimeSetUp]
        public async Task InitialSetup()
        {
            var contextOptions = new DbContextOptionsBuilder<BookingsDbContext>()
                .UseInMemoryDatabase("VhBookingsInMemory").Options;
            
            _context = new BookingsDbContext(contextOptions);
        }

        [OneTimeTearDown]
        public void FinalCleanUp()
        {
            _context.Database.EnsureDeleted();
        }

        [SetUp]
        public async Task SetUp()
        {
            _userRole1 = new UserRole(1, "user role 1");
            _userRole2 = new UserRole(2, "user role 2");

            _hearingRole1 = new HearingRole(1, "hearing role 1") {UserRole = _userRole1};
            _hearingRole2 = new HearingRole(2, "hearing role 2") {UserRole = _userRole2};

            _person1 = new Person(Faker.Name.Suffix(), Faker.Name.First(), Faker.Name.Last(), Faker.Internet.Email())
                {ContactEmail = Faker.Internet.Email()};
            _person2 = new Person(Faker.Name.Suffix(), Faker.Name.First(), Faker.Name.Last(), Faker.Internet.Email())
                {ContactEmail = Faker.Internet.Email()};
            _representativePerson = new Person(Faker.Name.Suffix(), Faker.Name.First(), Faker.Name.Last(),
                    Faker.Internet.Email())
                {ContactEmail = Faker.Internet.Email()};
            _anonymisedParticipantPerson = new Person(Faker.Name.Suffix(), Faker.Name.First(), Faker.Name.Last(),
                    Faker.Internet.Email())
                {ContactEmail = Faker.Internet.Email()};

            _caseType1 = new CaseType(12, "case 1")
                {HearingTypes = new List<HearingType> {new HearingType("Hearing type 1")}};

            await _context.CaseTypes.AddAsync(_caseType1);
            await _context.SaveChangesAsync();

            var hearingType = _caseType1.HearingTypes.First();

            _hearing1 = new VideoHearing(_caseType1,
                hearingType, DateTime.Today, 40,
                new HearingVenue(1, "venue 1"),
                Faker.Name.First(), Faker.Name.First(), null, true, false, null);

            _hearing2 = new VideoHearing(_caseType1,
                hearingType, DateTime.Today, 40,
                new HearingVenue(1, "venue 1"),
                Faker.Name.First(), Faker.Name.First(), null, true, false, null);

            _hearing3 = new VideoHearing(_caseType1,
                hearingType, DateTime.Today, 40,
                new HearingVenue(1, "venue 1"),
                Faker.Name.First(), Faker.Name.First(), null, true, false, null);

            _hearing1.AddJudge(_person1, _hearingRole1, new CaseRole(1, "judge"), "Judge 123");
            _hearing1.AddIndividual(_person2, _hearingRole2, new CaseRole(2, "individual"), "Individual 123");

            _hearing2.AddJudge(_person1, _hearingRole1, new CaseRole(1, "judge"), "Judge 123");
            _hearing2.AddIndividual(_person2, _hearingRole2, new CaseRole(2, "individual"), "Individual 123");

            _hearing3.AddJudge(_person1, _hearingRole1, new CaseRole(1, "judge"), "Judge 123");
            _hearing3.AddIndividual(_person2, _hearingRole2, new CaseRole(2, "individual"), "Individual 123");

            await _context.VideoHearings.AddRangeAsync(_hearing1, _hearing2, _hearing3);
            await _context.SaveChangesAsync();

            _command = new AnonymiseCaseAndParticipantCommandHandler(_context);
        }

        [TearDown]
        public async Task TearDown()
        {
            _context.CaseTypes.Remove(_caseType1);
            _context.VideoHearings.RemoveRange(_hearing1, _hearing2, _hearing3);

            var jobHistory = await _context.JobHistory.FirstOrDefaultAsync();

            if (jobHistory != null)
            {
                _context.JobHistory.Remove(jobHistory);
            }
            
            await _context.SaveChangesAsync();
        }

        [Test]
        public async Task AnonymiseCaseAndParticipantCommand_Anonymises_All_Participants_With_Matching_Hearing_Ids()
        {
            var command = new AnonymiseCaseAndParticipantCommand
            {
                HearingIds = new List<Guid> {_hearing1.Id, _hearing2.Id}
            };

            await _command.Handle(command);

            var participantsThatShouldBeAnonymised = _context.Participants
                .Where(p => p.HearingId == _hearing1.Id || p.HearingId == _hearing2.Id).ToList();

            var participantsThatShouldNotBeAnonymised = _context.Participants
                .Where(p => p.HearingId == _hearing3.Id).ToList();

            foreach (var participant in participantsThatShouldBeAnonymised)
            {
                participant.DisplayName.Should()
                    .Contain(AnonymiseCaseAndParticipantCommandHandler.AnonymisedNameSuffix);
            }

            foreach (var participant in participantsThatShouldNotBeAnonymised)
            {
                participant.DisplayName.Should()
                    .NotContain(AnonymiseCaseAndParticipantCommandHandler.AnonymisedNameSuffix);
            }
        }

        [Test]
        public async Task AnonymiseCaseAndParticipantCommand_Anonymises_Representee()
        {
            var repName = Faker.Name.FullName();
            _hearing1.AddRepresentative(_representativePerson, _hearingRole1, new CaseRole(1, "rep"), "Rep 123",
                repName);
            _hearing2.AddRepresentative(_representativePerson, _hearingRole1, new CaseRole(1, "rep"), "Rep 123",
                string.Empty);

            await _context.SaveChangesAsync();

            var command = new AnonymiseCaseAndParticipantCommand
            {
                HearingIds = new List<Guid> {_hearing1.Id, _hearing2.Id}
            };

            await _command.Handle(command);

            var participantsThatShouldBeAnonymised = _context.Participants
                .Where(p => p.HearingId == _hearing1.Id || p.HearingId == _hearing2.Id).ToList();

            foreach (var participant in participantsThatShouldBeAnonymised)
            {
                var castingParticipantAsRepresentative = participant as Representative;

                if (castingParticipantAsRepresentative != null)
                {
                    if (string.IsNullOrEmpty(castingParticipantAsRepresentative.Representee))
                    {
                        castingParticipantAsRepresentative.Representee.Should().BeNullOrEmpty();
                    }
                    else
                    {
                        castingParticipantAsRepresentative.Representee.Should().NotBe(repName);
                    }
                }
            }
        }

        [Test]
        public async Task AnonymiseCaseAndParticipantCommand_Skips_Already_Anonymised_Participant_Entry()
        {
            var anonymisedParticipantDisplayName =
                Faker.Name.Suffix() + AnonymiseCaseAndParticipantCommandHandler.AnonymisedNameSuffix;
            _hearing1.AddIndividual(_anonymisedParticipantPerson, _hearingRole2, new CaseRole(2, "individual"),
                "Individual 123");
            _hearing1.Participants.FirstOrDefault(p => p.PersonId == _anonymisedParticipantPerson.Id).DisplayName =
                anonymisedParticipantDisplayName;

            await _context.SaveChangesAsync();

            var command = new AnonymiseCaseAndParticipantCommand
            {
                HearingIds = new List<Guid> {_hearing1.Id, _hearing2.Id}
            };

            await _command.Handle(command);

            _context.Participants.FirstOrDefault(p => p.PersonId == _anonymisedParticipantPerson.Id).DisplayName
                .Should()
                .Be(anonymisedParticipantDisplayName);
        }

        [Test]
        public async Task AnonymiseCaseAndParticipantCommand_Anonymises_Case_Names_With_Matching_Hearing_Ids()
        {
            var caseName1 = Faker.Name.First();
            var caseName2 = Faker.Name.First();
            var caseName3 = Faker.Name.First();

            _hearing1.AddCase(Faker.RandomNumber.Next().ToString(), caseName1, false);
            _hearing2.AddCase(Faker.RandomNumber.Next().ToString(), caseName2, false);
            _hearing3.AddCase(Faker.RandomNumber.Next().ToString(), caseName3, false);

            await _context.SaveChangesAsync();
            
            var command = new AnonymiseCaseAndParticipantCommand
            {
                HearingIds = new List<Guid> {_hearing1.Id, _hearing2.Id}
            };

            await _command.Handle(command);

            var updatedHearing1 = _context.VideoHearings.FirstOrDefault(hearing => hearing.Id == _hearing1.Id);
            var updatedHearing2 = _context.VideoHearings.FirstOrDefault(hearing => hearing.Id == _hearing2.Id);
            var updatedHearing3 = _context.VideoHearings.FirstOrDefault(hearing => hearing.Id == _hearing3.Id);

            updatedHearing1.HearingCases.FirstOrDefault(hearing => hearing.HearingId == _hearing1.Id).Case.Name.Should()
                .NotBe(caseName1).And.Contain(AnonymiseCaseAndParticipantCommandHandler.AnonymisedNameSuffix);
            updatedHearing2.HearingCases.FirstOrDefault(hearing => hearing.HearingId == _hearing2.Id).Case.Name.Should()
                .NotBe(caseName2).And.Contain(AnonymiseCaseAndParticipantCommandHandler.AnonymisedNameSuffix);
            updatedHearing3.HearingCases.FirstOrDefault(hearing => hearing.HearingId == _hearing3.Id).Case.Name.Should()
                .Be(caseName3);
        }
        
        [Test]
        public async Task AnonymiseCaseAndParticipantCommand_Skips_Anonymised_Case()
        {
            var caseName1 = $"{Faker.Name.First()}{AnonymiseCaseAndParticipantCommandHandler.AnonymisedNameSuffix}";

            _hearing1.AddCase(Faker.RandomNumber.Next().ToString(), caseName1, false);

            await _context.SaveChangesAsync();
            
            var command = new AnonymiseCaseAndParticipantCommand
            {
                HearingIds = new List<Guid> {_hearing1.Id, _hearing2.Id}
            };

            await _command.Handle(command);

            var updatedHearing1 = _context.VideoHearings.FirstOrDefault(hearing => hearing.Id == _hearing1.Id);

            updatedHearing1.HearingCases.FirstOrDefault(hearing => hearing.HearingId == _hearing1.Id).Case.Name.Should()
                .Be(caseName1);
        }
        
        [Test]
        public async Task AnonymiseCaseAndParticipantCommand_Updates_JobHistory()
        {
            await _command.Handle(new AnonymiseCaseAndParticipantCommand {HearingIds = new List<Guid> {_hearing1.Id, _hearing2.Id}});

            var jobHistoryEntry = await _context.JobHistory.FirstOrDefaultAsync();

            jobHistoryEntry.Should().NotBeNull();
            jobHistoryEntry.LastRunDate.Value.Date.Should().Be(DateTime.UtcNow.Date);
        }
        
        [Test]
        public async Task AnonymiseCaseAndParticipantCommand_Updates_JobHistory_With_New_Date()
        {
            var jobHistory = new UpdateJobHistory();
            var lastRunDateTimeBeforeRunningCommand = jobHistory.LastRunDate;
            await _context.JobHistory.AddAsync(jobHistory);
            await _context.SaveChangesAsync();
            
            await _command.Handle(new AnonymiseCaseAndParticipantCommand {HearingIds = new List<Guid> {_hearing1.Id, _hearing2.Id}});

            var jobHistoryEntry = await _context.JobHistory.FirstOrDefaultAsync();

            jobHistoryEntry.Should().NotBeNull();
            _context.JobHistory.Count().Should().Be(1);
            jobHistoryEntry.LastRunDate.Value.Should().NotBe(lastRunDateTimeBeforeRunningCommand.Value);
        }     
    }
}