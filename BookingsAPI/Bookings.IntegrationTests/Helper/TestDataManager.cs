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
            var videoHearing = new VideoHearing(caseType, hearingType, scheduledDate, duration, venues.First(), hearingRoomName, otherInformation, createdBy);
            
            var participant = videoHearing.AddIndividual(person1, claimantLipHearingRole, claimantCaseRole,
                $"{person1.FirstName} {person1.LastName}");

            var participantSolicitor = videoHearing.AddSolicitor(person2, claimantSolicitorHearingRole, claimantCaseRole,
                $"{person2.FirstName} {person2.LastName}", string.Empty, string.Empty);
            
            videoHearing.AddSolicitor(person3, defendantSolicitorHearingRole, defendantCaseRole,
                $"{person3.FirstName} {person3.LastName}", string.Empty, string.Empty);

            videoHearing.AddJudge(person4, judgeHearingRole, judgeCaseRole, $"{person4.FirstName} {person4.LastName}");

            videoHearing.AddCase("1234567890", "Test Case", true);
            videoHearing.AddCase("1234567891", "Test Case2", false);

            if(addSuitabilityAnswer)
            {
                participant.AddSuitabilityAnswer("NEED_INTERPRETER", "No", "");
                participant.AddSuitabilityAnswer("SUITABLE_ROOM_AVAILABLE", "Yes", "");
                participant.UpdatedDate = DateTime.UtcNow;

                participantSolicitor.AddSuitabilityAnswer("ABOUT_YOUR_CLIENT", "No", "");
                participantSolicitor.AddSuitabilityAnswer("SUITABLE_ROOM_AVAILABLE", "No", "Comments");
                participantSolicitor.UpdatedDate = DateTime.UtcNow;
            }
            
            using (var db = new BookingsDbContext(_dbContextOptions))
            {
                await db.VideoHearings.AddAsync(videoHearing);
                await db.SaveChangesAsync();
            }

            var hearing = await new GetHearingByIdQueryHandler(new BookingsDbContext(_dbContextOptions)).Handle(
                new GetHearingByIdQuery(videoHearing.Id));
            
            _seededHearings.Add(hearing.Id);
            return hearing;
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
                    throw new InvalidOperationException("Unknown case type: "  + caseTypeName);
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
                var hearing = await db.VideoHearings.FindAsync(hearingId);
                if (hearing != null)
                {
                    db.Remove(hearing);
                    await db.SaveChangesAsync();   
                }
            }
        }

        public string[] GetIndividualHearingRoles => new[] { "Claimant LIP", "Defendant LIP", "Applicant LIP", "Respondent LIP" };
    }
}