using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bookings.DAL;
using Bookings.DAL.Queries;
using Bookings.Domain;
using Bookings.Domain.RefData;
using FizzWare.NBuilder;
using Microsoft.EntityFrameworkCore;
using Testing.Common.Builders.Domain;

namespace Bookings.IntegrationTests.Helper
{
    public class TestDataManager
    {
        private readonly DbContextOptions<BookingsDbContext> _dbContextOptions;
        private readonly List<Guid> _seededHearings = new List<Guid>();
        private BuilderSettings BuilderSettings { get; }
        private Guid _individualId;
        private List<Guid> _participantSolicitorIds;

        public TestDataManager(DbContextOptions<BookingsDbContext> dbContextOptions)
        {
            _dbContextOptions = dbContextOptions;

            BuilderSettings = new BuilderSettings();
        }

        public Task<VideoHearing> SeedVideoHearing(bool addSuitabilityAnswer = false)
        {
            return SeedVideoHearing(null, addSuitabilityAnswer);
        }

        public async Task<VideoHearing> SeedVideoHearing(Action<SeedVideoHearingOptions> configureOptions, bool addSuitabilityAnswer = false)
        {
            var options = new SeedVideoHearingOptions();
            configureOptions?.Invoke(options);
            var caseType = GetCaseTypeFromDb(options.CaseTypeName);

            var claimantCaseRole = caseType.CaseRoles.First(x => x.Name == options.ClaimantRole);
            var defendantCaseRole = caseType.CaseRoles.First(x => x.Name == options.DefendentRole);
            var judgeCaseRole = caseType.CaseRoles.First(x => x.Name == "Judge");

            var claimantLipHearingRole = claimantCaseRole.HearingRoles.First(x => x.Name == options.ClaimantHearingRole);
            var claimantSolicitorHearingRole = claimantCaseRole.HearingRoles.First(x => x.Name == "Solicitor");
            var defendantSolicitorHearingRole = defendantCaseRole.HearingRoles.First(x => x.Name == "Solicitor");
            var judgeHearingRole = judgeCaseRole.HearingRoles.First(x => x.Name == "Judge");

            var hearingType = caseType.HearingTypes.First(x => x.Name == options.HearingTypeName);

            var venues = new RefDataBuilder().HearingVenues;

            var person1 = new PersonBuilder(true).WithOrganisation().WithAddress().Build();
            var person2 = new PersonBuilder(true).Build();
            var person3 = new PersonBuilder(true).Build();
            var person4 = new PersonBuilder(true).Build();
            var scheduledDate = DateTime.Today.AddDays(1).AddHours(10).AddMinutes(30);
            var duration = 45;
            var hearingRoomName = "Room02";
            var otherInformation = "OtherInformation02";
            var createdBy = "test@integration.com";
            const bool questionnaireNotRequired = false;
            var videoHearing = new VideoHearing(caseType, hearingType, scheduledDate, duration,
                venues.First(), hearingRoomName, otherInformation, createdBy, questionnaireNotRequired);

            videoHearing.AddIndividual(person1, claimantLipHearingRole, claimantCaseRole,
                 $"{person1.FirstName} {person1.LastName}");

            videoHearing.AddSolicitor(person2, claimantSolicitorHearingRole, claimantCaseRole,
                $"{person2.FirstName} {person2.LastName}", string.Empty, "Ms X");

            videoHearing.AddSolicitor(person3, defendantSolicitorHearingRole, defendantCaseRole,
                $"{person3.FirstName} {person3.LastName}", string.Empty, "Ms Y");

            videoHearing.AddJudge(person4, judgeHearingRole, judgeCaseRole, $"{person4.FirstName} {person4.LastName}");

            videoHearing.AddCase($"{Faker.RandomNumber.Next(1000, 9999)}/{Faker.RandomNumber.Next(1000, 9999)}", $"Bookings Api Integration Test {Faker.RandomNumber.Next(900000, 999999)}", true);
            videoHearing.AddCase($"{Faker.RandomNumber.Next(1000, 9999)}/{Faker.RandomNumber.Next(1000, 9999)}", $"Bookings Api Integration Test {Faker.RandomNumber.Next(900000, 999999)}", false);

            using (var db = new BookingsDbContext(_dbContextOptions))
            {
                await db.VideoHearings.AddAsync(videoHearing);
                await db.SaveChangesAsync();
            }
            var hearing = await new GetHearingByIdQueryHandler(new BookingsDbContext(_dbContextOptions)).Handle(
               new GetHearingByIdQuery(videoHearing.Id));
            _individualId = hearing.Participants.FirstOrDefault(x => x.HearingRole.Name.ToLower().IndexOf("judge") < 0 && x.HearingRole.Name.ToLower().IndexOf("solicitor") < 0).Id;
            _participantSolicitorIds = hearing.Participants.Where(x => x.HearingRole.Name.ToLower().IndexOf("solicitor") >= 0).Select(x => x.Id).ToList();

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
            using (var db = new BookingsDbContext(_dbContextOptions))
            {
                AddIndividualQuestionnaire(db);
                AddRepresentativeQuestionnaire(db);

                await db.SaveChangesAsync();
            }
        }

        private void AddIndividualQuestionnaire(BookingsDbContext db)
        {
            var participant = db.Participants
                .Include(x => x.Questionnaire)
                .FirstOrDefault(x => x.Id == _individualId);
            participant.Questionnaire = new Questionnaire { Participant = participant, ParticipantId = _individualId };
            participant.Questionnaire.AddSuitabilityAnswer("INTERPRETER", "No", "");
            participant.Questionnaire.AddSuitabilityAnswer("ROOM", "Yes", "");
            participant.UpdatedDate = DateTime.UtcNow;
            db.Participants.Update(participant);
        }

        private void AddRepresentativeQuestionnaire(BookingsDbContext db)
        {
            foreach (var item in _participantSolicitorIds)
            {
                var participantSolicitor = db.Participants
                .Include(x => x.Questionnaire)
                .FirstOrDefault(x => x.Id == item);
                participantSolicitor.Questionnaire = new Questionnaire { Participant = participantSolicitor, ParticipantId = item };

                participantSolicitor.Questionnaire.AddSuitabilityAnswer("ABOUT_YOUR_CLIENT", "No", "");
                participantSolicitor.Questionnaire.AddSuitabilityAnswer("ROOM", "No", "Comments");
                participantSolicitor.UpdatedDate = DateTime.UtcNow;
                db.Participants.Update(participantSolicitor);
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
            using (var db = new BookingsDbContext(_dbContextOptions))
            {
                var hearing = await db.VideoHearings.Include("HearingCases.Case")
                        .SingleOrDefaultAsync(x => x.Id == hearingId);
                if (hearing != null)
                {
                    var persons = hearing.Participants.Select(x => x.Person);
                    db.RemoveRange(persons);
                    db.RemoveRange(hearing.GetCases());
                    db.Remove(hearing);
                    await db.SaveChangesAsync();
                }
            }
        }

        public string[] GetIndividualHearingRoles => new[] { "Claimant LIP", "Defendant LIP", "Applicant LIP", "Respondent LIP" };
    }
}