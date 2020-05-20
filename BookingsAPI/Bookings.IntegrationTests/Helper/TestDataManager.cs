using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bookings.DAL;
using Bookings.DAL.Commands;
using Bookings.DAL.Exceptions;
using Bookings.DAL.Queries;
using Bookings.Domain;
using Bookings.Domain.Enumerations;
using Bookings.Domain.RefData;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Testing.Common.Builders.Domain;

namespace Bookings.IntegrationTests.Helper
{
    public class TestDataManager
    {
        private readonly DbContextOptions<BookingsDbContext> _dbContextOptions;
        private readonly List<Guid> _seededHearings = new List<Guid>();
        private Guid _individualId;
        private List<Guid> _participantRepresentativeIds;
        private readonly string _defaultCaseName;

        public TestDataManager(DbContextOptions<BookingsDbContext> dbContextOptions, string defaultCaseName)
        {
            _dbContextOptions = dbContextOptions;
            _defaultCaseName = defaultCaseName;
        }

        public Task<VideoHearing> SeedVideoHearing(bool addSuitabilityAnswer = false, BookingStatus status = BookingStatus.Booked)
        {
            return SeedVideoHearing(null, addSuitabilityAnswer, status);
        }

        public async Task<VideoHearing> SeedVideoHearing(Action<SeedVideoHearingOptions> configureOptions,
            bool addSuitabilityAnswer = false, BookingStatus status = BookingStatus.Booked)
        {
            var options = new SeedVideoHearingOptions();
            configureOptions?.Invoke(options);
            var caseType = GetCaseTypeFromDb(options.CaseTypeName);

            var claimantCaseRole = caseType.CaseRoles.First(x => x.Name == options.ClaimantRole);
            var defendantCaseRole = caseType.CaseRoles.First(x => x.Name == options.DefendentRole);
            var judgeCaseRole = caseType.CaseRoles.First(x => x.Name == "Judge");

            var claimantLipHearingRole = claimantCaseRole.HearingRoles.First(x => x.Name == options.ClaimantHearingRole);
            var claimantRepresentativeHearingRole = claimantCaseRole.HearingRoles.First(x => x.Name == "Representative");
            var defendantRepresentativeHearingRole = defendantCaseRole.HearingRoles.First(x => x.Name == "Representative");
            var judgeHearingRole = judgeCaseRole.HearingRoles.First(x => x.Name == "Judge");

            var hearingType = caseType.HearingTypes.First(x => x.Name == options.HearingTypeName);

            var venues = new RefDataBuilder().HearingVenues;

            var person1 = new PersonBuilder(true).WithOrganisation().WithAddress().Build();
            var person2 = new PersonBuilder(true).Build();
            var person3 = new PersonBuilder(true).Build();
            var person4 = new PersonBuilder(true).Build();
            var scheduledDate = DateTime.Today.AddDays(1).AddHours(10).AddMinutes(30);
            const int duration = 45;
            const string hearingRoomName = "Room02";
            const string otherInformation = "OtherInformation02";
            const string createdBy = "test@integration.com";
            const bool questionnaireNotRequired = false;
            const bool audioRecordingRequired = true;
            var cancelReason = "Online abandonment (incomplete registration)";

            var videoHearing = new VideoHearing(caseType, hearingType, scheduledDate, duration,
                venues.First(), hearingRoomName, otherInformation, createdBy, questionnaireNotRequired, 
                audioRecordingRequired, cancelReason);

            videoHearing.AddIndividual(person1, claimantLipHearingRole, claimantCaseRole,
                $"{person1.FirstName} {person1.LastName}");

            videoHearing.AddRepresentative(person2, claimantRepresentativeHearingRole, claimantCaseRole,
                $"{person2.FirstName} {person2.LastName}", string.Empty, "Ms X");

            videoHearing.AddRepresentative(person3, defendantRepresentativeHearingRole, defendantCaseRole,
                $"{person3.FirstName} {person3.LastName}", string.Empty, "Ms Y");

            videoHearing.AddJudge(person4, judgeHearingRole, judgeCaseRole, $"{person4.FirstName} {person4.LastName}");

            videoHearing.AddCase($"{Faker.RandomNumber.Next(1000, 9999)}/{Faker.RandomNumber.Next(1000, 9999)}",
                $"{_defaultCaseName} {Faker.RandomNumber.Next(900000, 999999)}", true);
            videoHearing.AddCase($"{Faker.RandomNumber.Next(1000, 9999)}/{Faker.RandomNumber.Next(1000, 9999)}",
                $"{_defaultCaseName} {Faker.RandomNumber.Next(900000, 999999)}", false);
            if(status == BookingStatus.Created)
            {
                videoHearing.UpdateStatus(BookingStatus.Created, createdBy, null);
            }

            await using (var db = new BookingsDbContext(_dbContextOptions))
            {
                await db.VideoHearings.AddAsync(videoHearing);
                await db.SaveChangesAsync();
            }

            var hearing = await new GetHearingByIdQueryHandler(new BookingsDbContext(_dbContextOptions)).Handle(
                new GetHearingByIdQuery(videoHearing.Id));
            _individualId = hearing.Participants.First(x =>
                x.HearingRole.Name.ToLower().IndexOf("judge", StringComparison.Ordinal) < 0 &&
                x.HearingRole.Name.ToLower().IndexOf("representative", StringComparison.Ordinal) < 0).Id;
            _participantRepresentativeIds = hearing.Participants
                .Where(x => x.HearingRole.Name.ToLower().IndexOf("representative", StringComparison.Ordinal) >= 0).Select(x => x.Id).ToList();

            if (addSuitabilityAnswer)
            {
                await AddQuestionnaire();
            }

            hearing = await new GetHearingByIdQueryHandler(new BookingsDbContext(_dbContextOptions)).Handle(
                new GetHearingByIdQuery(videoHearing.Id));
            _seededHearings.Add(hearing.Id);
            return hearing;
        }

        private async Task AddQuestionnaire()
        {
            await using var db = new BookingsDbContext(_dbContextOptions);
            AddIndividualQuestionnaire(db);
            AddRepresentativeQuestionnaire(db);

            await db.SaveChangesAsync();
        }

        private void AddIndividualQuestionnaire(BookingsDbContext db)
        {
            var participant = db.Participants
                .Include(x => x.Questionnaire)
                .FirstOrDefault(x => x.Id == _individualId);
            participant.Questionnaire = new Questionnaire {Participant = participant, ParticipantId = _individualId};
            participant.Questionnaire.AddSuitabilityAnswer("INTERPRETER", "No", "");
            participant.Questionnaire.AddSuitabilityAnswer("ROOM", "Yes", "");
            participant.UpdatedDate = DateTime.UtcNow;
            db.Participants.Update(participant);
        }

        private void AddRepresentativeQuestionnaire(BookingsDbContext db)
        {
            foreach (var item in _participantRepresentativeIds)
            {
                var participantRepresentative = db.Participants
                    .Include(x => x.Questionnaire)
                    .FirstOrDefault(x => x.Id == item);
                participantRepresentative.Questionnaire = new Questionnaire
                    {Participant = participantRepresentative, ParticipantId = item};

                participantRepresentative.Questionnaire.AddSuitabilityAnswer("ABOUT_YOUR_CLIENT", "No", "");
                participantRepresentative.Questionnaire.AddSuitabilityAnswer("ROOM", "No", "Comments");
                participantRepresentative.UpdatedDate = DateTime.UtcNow;
                db.Participants.Update(participantRepresentative);
            }
        }

        private CaseType GetCaseTypeFromDb(string caseTypeName)
        {
            CaseType caseType;
            using (var db = new BookingsDbContext(_dbContextOptions))
            {
                caseType = db.CaseTypes
                    .Include(x => x.CaseRoles)
                    .ThenInclude(x => x.HearingRoles)
                    .ThenInclude(x => x.UserRole)
                    .Include(x => x.HearingTypes)
                    .FirstOrDefault(x => x.Name == caseTypeName);

                if (caseType == null)
                {
                    throw new InvalidOperationException("Unknown case type: " + caseTypeName);
                }
            }

            return caseType;
        }

        public async Task ClearSeededHearings()
        {
            foreach (var hearingId in _seededHearings)
            {
                await RemoveVideoHearing(hearingId);
            }
        }

        public async Task RemoveVideoHearing(Guid hearingId)
        {
            try
            {
                await using var db = new BookingsDbContext(_dbContextOptions);
                await new RemoveHearingCommandHandler(db).Handle(new RemoveHearingCommand(hearingId));
            }
            catch (HearingNotFoundException)
            {
                TestContext.WriteLine(@$"Ignoring cleanup for: ${hearingId}. Does not exist.");
            }
        }
        
        public async Task ClearUnattachedPersons(IEnumerable<string> removedPersons)
        {
            foreach (var personEmail in removedPersons)
            {
                await using var db = new BookingsDbContext(_dbContextOptions);
                var person = await db.Persons
                    .Include(x => x.Address)
                    .Include(x => x.Organisation)
                    .SingleOrDefaultAsync(x => x.ContactEmail == personEmail);

                if (person == null) return;
                if(person.Address != null)db.Remove(person.Address);
                if(person.Organisation != null)db.Remove(person.Organisation);
                db.Remove(person);

                await db.SaveChangesAsync();
            }

        }

        public string[] GetIndividualHearingRoles =>
            new[] {"Claimant LIP", "Defendant LIP", "Applicant LIP", "Respondent LIP"};
    }
}