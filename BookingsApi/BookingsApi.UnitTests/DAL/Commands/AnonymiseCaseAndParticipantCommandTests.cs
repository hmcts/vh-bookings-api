using System.Collections.Generic;
using Bogus;
using BookingsApi.DAL;
using BookingsApi.DAL.Commands;
using BookingsApi.Domain;
using BookingsApi.Domain.Participants;
using BookingsApi.Domain.RefData;
using Microsoft.EntityFrameworkCore;
using Person = BookingsApi.Domain.Person;

namespace BookingsApi.UnitTests.DAL.Commands
{
    public class AnonymiseCaseAndParticipantCommandTests
    {
        private AnonymiseCaseAndParticipantCommandHandler _command;
        private BookingsDbContext _context;
        private Person _individualPerson, _representativePerson, _anonymisedParticipantPerson;
        private JudiciaryPerson _judgePerson;
        private VideoHearing _hearing1, _hearing2, _hearing3;
        private CaseType _caseType1;
        private UserRole _personUserRole;
        private HearingRole personHearingRole;
        private HearingVenue _hearingVenue1;
        private static readonly Faker Faker = new();
        
        [SetUp]
        public async Task SetUp()
        {
            var contextOptions = new DbContextOptionsBuilder<BookingsDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            
            _context = new BookingsDbContext(contextOptions);
            
            // seed roles
            await SeedRefData();
            await SeedHearingVenues();
            await SeedHearings();

            _command = new AnonymiseCaseAndParticipantCommandHandler(_context);
        }

        [TearDown]
        public async Task TearDown()
        {
            _context.VideoHearings.RemoveRange(_context.VideoHearings);
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
            _hearing1.AddRepresentative(Guid.NewGuid().ToString(), _representativePerson, personHearingRole, "Rep 123",
                repName);
            _hearing2.AddRepresentative(Guid.NewGuid().ToString(), _representativePerson, personHearingRole, "Rep 456",
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
            _hearing1.AddIndividual(Guid.NewGuid().ToString(), _anonymisedParticipantPerson, personHearingRole,
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
            var caseName1 = Faker.Name.FirstName();
            var caseName2 = Faker.Name.FirstName();
            var caseName3 = Faker.Name.FirstName();

            _hearing1.AddCase(Faker.Random.Number(0, 9999999).ToString(), caseName1, false);
            _hearing2.AddCase(Faker.Random.Number(0, 9999999).ToString(), caseName2, false);
            _hearing3.AddCase(Faker.Random.Number(0, 9999999).ToString(), caseName3, false);

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
            var caseName1 = $"{Faker.Name.FirstName()}{AnonymiseCaseAndParticipantCommandHandler.AnonymisedNameSuffix}";

            _hearing1.AddCase(Faker.Random.Number().ToString(), caseName1, false);

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
        
           private async Task SeedRefData()
        {
            _personUserRole = new UserRole(2, "user role 2");

            personHearingRole = new HearingRole(2, "hearing role 2") {UserRole = _personUserRole};
            
            _caseType1 = new CaseType(12, "case 1");
            
            await _context.CaseTypes.AddAsync(_caseType1);
            await _context.SaveChangesAsync();
        }
        
        private async Task SeedHearings()
        {
            _judgePerson = new JudiciaryPersonBuilder("1234Judge").Build();
            _individualPerson = new PersonBuilder().Build();
            _representativePerson = new PersonBuilder().Build();
            _anonymisedParticipantPerson = new PersonBuilder().Build();

            // TODO: investigate impact to switching to VideoHearingBuilder
            _hearing1 = new VideoHearing(_caseType1, DateTime.Today, 40, _hearingVenue1, 
                Faker.Name.FirstName(), Faker.Name.FirstName(), null, false);
            
            _hearing2 = new VideoHearing(_caseType1, DateTime.Today, 40, _hearingVenue1, 
                Faker.Name.FirstName(), Faker.Name.FirstName(), null, false);
            
            _hearing3 = new VideoHearing(_caseType1, DateTime.Today, 40, _hearingVenue1, 
                Faker.Name.FirstName(), Faker.Name.FirstName(), null, false);
            
            _hearing1.AddJudiciaryJudge(_judgePerson, "Judge 123");
            _hearing1.AddIndividual(Guid.NewGuid().ToString(), _individualPerson, personHearingRole, "Individual 123");

            _hearing2.AddJudiciaryJudge(_judgePerson, "Judge 123");
            _hearing2.AddIndividual(Guid.NewGuid().ToString(), _individualPerson, personHearingRole, "Individual 123");

            _hearing3.AddJudiciaryJudge(_judgePerson, "Judge 123");
            _hearing3.AddIndividual(Guid.NewGuid().ToString(), _individualPerson, personHearingRole, "Individual 123");
            
            await _context.VideoHearings.AddRangeAsync(_hearing1, _hearing2, _hearing3);
            await _context.SaveChangesAsync();
        }

        private async Task SeedHearingVenues()
        {
            _hearingVenue1 = new HearingVenue(1, "venue 1");

            await _context.Venues.AddAsync(_hearingVenue1);
            await _context.SaveChangesAsync();
        }
    }
}