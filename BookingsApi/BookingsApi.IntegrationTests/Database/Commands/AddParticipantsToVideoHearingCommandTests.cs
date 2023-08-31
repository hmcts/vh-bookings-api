using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Dtos;
using BookingsApi.DAL.Exceptions;
using BookingsApi.DAL.Queries;
using BookingsApi.DAL.Services;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.Participants;
using BookingsApi.Domain.RefData;
using BookingsApi.Domain.Validations;
using Testing.Common.Builders.Domain;

namespace BookingsApi.IntegrationTests.Database.Commands
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
        public void Should_throw_exception_when_hearing_does_not_exist()
        {
            var hearingId = Guid.NewGuid();

            Assert.ThrowsAsync<HearingNotFoundException>(() => _commandHandler.Handle(
                new AddParticipantsToVideoHearingCommand(hearingId, new List<NewParticipant>(), null)));
        }

        [Test]
        public async Task Should_add_participants_to_video_hearing()
        {
            var seededHearing = await Hooks.SeedVideoHearing();
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            _newHearingId = seededHearing.Id;

            var beforeCount = seededHearing.GetParticipants().Count;

            const string caseTypeName = "Generic";
            var caseType = GetCaseTypeFromDb(caseTypeName);

            var applicantCaseRole = caseType.CaseRoles.First(x => x.Name == "Applicant");
            var applicantRepresentativeHearingRole =
                applicantCaseRole.HearingRoles.First(x => x.Name == "Representative");

            var newPerson = new PersonBuilder(true).Build();
            var newParticipant = new NewParticipant()
            {
                Person = newPerson,
                CaseRole = applicantCaseRole,
                HearingRole = applicantRepresentativeHearingRole,
                DisplayName = $"{newPerson.FirstName} {newPerson.LastName}",
                Representee = string.Empty
            };
            var participants = new List<NewParticipant>()
            {
                newParticipant
            };

            await _commandHandler.Handle(new AddParticipantsToVideoHearingCommand(_newHearingId, participants, null));

            var returnedVideoHearing =
                await _getHearingByIdQueryHandler.Handle(new GetHearingByIdQuery(seededHearing.Id));
            var afterCount = returnedVideoHearing.GetParticipants().Count;

            afterCount.Should().BeGreaterThan(beforeCount);
        }

        [Test]
        public async Task Should_not_add_existing_participant_to_the_same_video_hearing()
        {
            var seededHearing = await Hooks.SeedVideoHearing();
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            _newHearingId = seededHearing.Id;
            var representative =
                (Representative) seededHearing.GetParticipants().First(x => x.GetType() == typeof(Representative));

            var newParticipant = new NewParticipant()
            {
                Person = representative.Person,
                CaseRole = representative.CaseRole,
                HearingRole = representative.HearingRole,
                DisplayName = representative.DisplayName,
                Representee = representative.Representee
            };

            var participants = new List<NewParticipant>()
            {
                newParticipant
            };

            Assert.ThrowsAsync<DomainRuleException>(() => _commandHandler.Handle(
                new AddParticipantsToVideoHearingCommand(_newHearingId, participants, null)));
        }

        [Test]
        public async Task Should_throw_exception_when_adding_unsupported_role_to_hearing()
        {
            var seededHearing = await Hooks.SeedVideoHearing();
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            _newHearingId = seededHearing.Id;
            var representative =
                (Representative) seededHearing.GetParticipants().First(x => x.GetType() == typeof(Representative));

            representative.HearingRole.UserRole.Name = "NonExistent";
            var newParticipant = new NewParticipant()
            {
                Person = representative.Person,
                CaseRole = representative.CaseRole,
                HearingRole = representative.HearingRole,
                DisplayName = representative.DisplayName,
                Representee = representative.Representee
            };

            var participants = new List<NewParticipant>()
            {
                newParticipant
            };

            Assert.ThrowsAsync<DomainRuleException>(() => _commandHandler.Handle(
                new AddParticipantsToVideoHearingCommand(_newHearingId, participants, null)));
        }

        [Test]
        public async Task Should_use_existing_representative_when_adding_to_video_hearing()
        {
            var seededRepresentative = await SeedPerson(true);
            var personListBefore = await AddExistingPersonToAHearing(seededRepresentative);
            var personsListAfter = await GetPersonsInDb();
            personsListAfter.Count.Should().Be(personListBefore.Count);

            var existingPersonInDb =
                personsListAfter.Single(p => p.ContactEmail.Equals(seededRepresentative.ContactEmail));
            existingPersonInDb.Organisation.Name.Should().Be(seededRepresentative.Organisation.Name);
            CheckPersonDetails(existingPersonInDb, seededRepresentative);
        }

        [Test]
        public async Task Should_use_existing_individual_when_adding_to_video_hearing()
        {
            var seededRepresentative = await SeedPerson(true);
            var personListBefore = await AddExistingPersonToAHearing(seededRepresentative);
            var personsListAfter = await GetPersonsInDb();
            personsListAfter.Count.Should().Be(personListBefore.Count);

            var existingPersonInDb =
                personsListAfter.Single(p => p.ContactEmail.Equals(seededRepresentative.ContactEmail));
            existingPersonInDb.Organisation.Should().NotBeNull();
            existingPersonInDb.Organisation.Name.Should().Be(seededRepresentative.Organisation.Name);
            CheckPersonDetails(existingPersonInDb, seededRepresentative);
        }

        [Test]
        public async Task Should_Add_Participant_With_A_Link()
        {
            var seededHearing = await Hooks.SeedVideoHearing();
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            _newHearingId = seededHearing.Id;

            const string caseTypeName = "Generic";
            var caseType = GetCaseTypeFromDb(caseTypeName);

            var applicantCaseRole = caseType.CaseRoles.First(x => x.Name == "Applicant");
            var applicantRepresentativeHearingRole =
                applicantCaseRole.HearingRoles.First(x => x.Name == "Representative");

            var respondentCaseRole = caseType.CaseRoles.First(x => x.Name == "Respondent");
            var respondentRepresentativeHearingRole =
                respondentCaseRole.HearingRoles.First(x => x.Name == "Representative");

            var newPerson = new PersonBuilder(true).Build();
            var newPerson1 = new PersonBuilder(true).Build();
            var interpretee = new NewParticipant()
            {
                Person = newPerson,
                CaseRole = applicantCaseRole,
                HearingRole = applicantRepresentativeHearingRole,
                DisplayName = $"{newPerson.FirstName} {newPerson.LastName}",
                Representee = string.Empty
            };
            var interpreter = new NewParticipant()
            {
                Person = newPerson1,
                CaseRole = respondentCaseRole,
                HearingRole = respondentRepresentativeHearingRole,
                DisplayName = $"{newPerson.FirstName} {newPerson.LastName}",
                Representee = string.Empty
            };
            var participants = new List<NewParticipant>()
            {
                interpretee,
                interpreter
            };
            var links = new List<LinkedParticipantDto>();
            var link = new LinkedParticipantDto(
                interpreter.Person.ContactEmail, 
                interpretee.Person.ContactEmail,
                LinkedParticipantType.Interpreter
                );
            
            links.Add(link);

            await _commandHandler.Handle(new AddParticipantsToVideoHearingCommand(_newHearingId, participants, links));

            var returnedVideoHearing =
                await _getHearingByIdQueryHandler.Handle(new GetHearingByIdQuery(seededHearing.Id));

            returnedVideoHearing.Participants.Where(x => x.LinkedParticipants.Any()).Should().NotBeNull();
        }

        [Test]
        public async Task Should_Add_New_Participant_Linked_To_Existing_Participant()
        {
            var seededHearing = await Hooks.SeedVideoHearing();
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            _newHearingId = seededHearing.Id;

            const string caseTypeName = "Generic";
            var caseType = GetCaseTypeFromDb(caseTypeName);

            var respondentCaseRole = caseType.CaseRoles.First(x => x.Name == "Respondent");
            var respondentRepresentativeHearingRole =
                respondentCaseRole.HearingRoles.First(x => x.Name == "Representative");

            var newPerson = new PersonBuilder(true).Build();

            var interpreter = new NewParticipant()
            {
                Person = newPerson,
                CaseRole = respondentCaseRole,
                HearingRole = respondentRepresentativeHearingRole,
                DisplayName = $"{newPerson.FirstName} {newPerson.LastName}",
                Representee = string.Empty
            };
            var participants = new List<NewParticipant>()
            {
                interpreter
            };
            var links = new List<LinkedParticipantDto>();
            var link = new LinkedParticipantDto(
                interpreter.Person.ContactEmail,
                seededHearing.Participants[0].Person.ContactEmail,
                LinkedParticipantType.Interpreter
                );

            links.Add(link);

            await _commandHandler.Handle(new AddParticipantsToVideoHearingCommand(_newHearingId, participants, links));

            var returnedVideoHearing =
                await _getHearingByIdQueryHandler.Handle(new GetHearingByIdQuery(seededHearing.Id));

            returnedVideoHearing.Participants.Where(x => x.LinkedParticipants.Any()).Should().NotBeNull();
        }

        [TestCase("Judge", "Judge")]
        [TestCase("Staff Member", "Staff Member")]
        public async Task Should_add_participant_to_video_hearing(string caseRole, string hearingRole)
        {
            var seededHearing = await Hooks.SeedVideoHearing(configureOptions: options =>
            {
                options.AddJudge = false;
            });
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            _newHearingId = seededHearing.Id;

            var beforeCount = seededHearing.GetParticipants().Count;

            var caseTypeName = "Generic";
            var caseType = GetCaseTypeFromDb(caseTypeName);

            var judgeCaseRole = caseType.CaseRoles.First(x => x.Name == caseRole);
            var judgeHearingRole = judgeCaseRole.HearingRoles.First(x => x.Name == hearingRole);

            var newPerson = new PersonBuilder(true).Build();
            var newParticipant = new NewParticipant()
            {
                Person = newPerson,
                CaseRole = judgeCaseRole,
                HearingRole = judgeHearingRole,
                DisplayName = $"{newPerson.FirstName} {newPerson.LastName}",
                Representee = string.Empty
            };
            var participants = new List<NewParticipant>()
            {
                newParticipant
            };

            await _commandHandler.Handle(new AddParticipantsToVideoHearingCommand(_newHearingId, participants, null));

            var returnedVideoHearing =
                await _getHearingByIdQueryHandler.Handle(new GetHearingByIdQuery(seededHearing.Id));
            var afterCount = returnedVideoHearing.GetParticipants().Count;

            afterCount.Should().BeGreaterThan(beforeCount);
        }

        [Test]
        public async Task Should_add_a_participant_with_same_contactemail_and_username_of_other_participants_to_video_hearing()
        {
            var seededHearing1 = await Hooks.SeedVideoHearing();
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing1.Id}");
            _newHearingId = seededHearing1.Id;

            var participantsFromHearing1 = seededHearing1.GetParticipants();
            var beforeCount = seededHearing1.GetParticipants().Count;

            const string caseTypeName = "Generic";
            var caseType = GetCaseTypeFromDb(caseTypeName);

            var applicantCaseRole = caseType.CaseRoles.First(x => x.Name == "Applicant");
            var applicantRepresentativeHearingRole =
                applicantCaseRole.HearingRoles.First(x => x.Name == "Representative");

            var seededHearing2 = await Hooks.SeedVideoHearing();
            var newPerson = new PersonBuilder(participantsFromHearing1[1].Person.Username, participantsFromHearing1[0].Person.ContactEmail).Build();
            var newParticipant = new NewParticipant()
            {
                Person = newPerson,
                CaseRole = applicantCaseRole,
                HearingRole = applicantRepresentativeHearingRole,
                DisplayName = $"{newPerson.FirstName} {newPerson.LastName}",
                Representee = string.Empty
            };
            var participants = new List<NewParticipant>()
            {
                newParticipant
            };

            await _commandHandler.Handle(new AddParticipantsToVideoHearingCommand(seededHearing2.Id, participants, null));

            var returnedVideoHearing =
                await _getHearingByIdQueryHandler.Handle(new GetHearingByIdQuery(seededHearing2.Id));
            var afterCount = returnedVideoHearing.GetParticipants().Count;

            afterCount.Should().BeGreaterThan(beforeCount);
        }


        private async Task<List<Person>> AddExistingPersonToAHearing(Person existingPerson)
        {
            var seededHearing = await Hooks.SeedVideoHearing();
            var personListBefore = await GetPersonsInDb();

            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            _newHearingId = seededHearing.Id;

            var caseTypeName = "Generic";
            var caseType = GetCaseTypeFromDb(caseTypeName);

            var applicantCaseRole = caseType.CaseRoles.First(x => x.Name == "Applicant");
            var applicantRepresentativeHearingRole =
                applicantCaseRole.HearingRoles.First(x => x.Name == "Representative");

            var newParticipant = new NewParticipant()
            {
                Person = existingPerson,
                CaseRole = applicantCaseRole,
                HearingRole = applicantRepresentativeHearingRole,
                DisplayName = $"{existingPerson.FirstName} {existingPerson.LastName}",
                Representee = string.Empty
            };

            var participants = new List<NewParticipant>()
            {
                newParticipant
            };

            await _commandHandler.Handle(new AddParticipantsToVideoHearingCommand(_newHearingId, participants, null));

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

        private async Task<List<Person>> GetPersonsInDb()
        {
            await using var db = new BookingsDbContext(BookingsDbContextOptions);
            return await db.Persons
                .Include(p => p.Organisation)
                .ToListAsync();
        }

        private async Task<Person> SeedPerson(bool withOrganisation = false)
        {
            var builder = new PersonBuilder(true);

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
    }
}