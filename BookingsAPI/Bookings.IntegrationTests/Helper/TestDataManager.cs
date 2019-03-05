using System;
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
        private BuilderSettings BuilderSettings { get; set; }

        public TestDataManager(DbContextOptions<BookingsDbContext> dbContextOptions)
        {
            _dbContextOptions = dbContextOptions;
            
            BuilderSettings = new BuilderSettings();
        }

        public async Task<VideoHearing> SeedVideoHearing()
        {
            var caseTypeName = "Civil Money Claims";
            var caseType = GetCaseTypeFromDb(caseTypeName);

            var claimantCaseRole = caseType.CaseRoles.First(x => x.Name == "Claimant");
            var defendantCaseRole = caseType.CaseRoles.First(x => x.Name == "Defendant");

            var claimantLipHearingRole = claimantCaseRole.HearingRoles.First(x => x.Name == "Claimant LIP");
            var claimantSolicitorHearingRole = claimantCaseRole.HearingRoles.First(x => x.Name == "Solicitor");
            var defendantSolicitorHearingRole = defendantCaseRole.HearingRoles.First(x => x.Name == "Solicitor");

            var hearingTypeName = "Application to Set Judgment Aside";
            var hearingType = caseType.HearingTypes.First(x => x.Name == hearingTypeName);

            var venues = new RefDataBuilder().HearingVenues;

            var person1 = new PersonBuilder(true).Build();
            var person2 = new PersonBuilder(true).Build();
            var person3 = new PersonBuilder(true).Build();

            var scheduledDate = DateTime.Today.AddHours(10).AddMinutes(30);
            var duration = 45;
            var videoHearing = new VideoHearing(caseType, hearingType, scheduledDate, duration, venues.First())
            {
                CreatedBy = "test@integration.com"
            };
            
            videoHearing.AddIndividual(person1, claimantLipHearingRole, claimantCaseRole,
                $"{person1.FirstName} {person1.LastName}");

            videoHearing.AddSolicitor(person2, claimantSolicitorHearingRole, claimantCaseRole,
                $"{person2.FirstName} {person2.LastName}", string.Empty, string.Empty);
            
            videoHearing.AddSolicitor(person3, defendantSolicitorHearingRole, defendantCaseRole,
                $"{person3.FirstName} {person3.LastName}", string.Empty, string.Empty);
            
            videoHearing.AddCase("1234567890", "Test Case", true);
            videoHearing.AddCase("1234567891", "Test Case2", false);

            using (var db = new BookingsDbContext(_dbContextOptions))
            {
                await db.VideoHearings.AddAsync(videoHearing);
                await db.SaveChangesAsync();
            }

            return await new GetHearingByIdQueryHandler(new BookingsDbContext(_dbContextOptions)).Handle(
                new GetHearingByIdQuery(videoHearing.Id)); 
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
                    .First(x => x.Name == caseTypeName);
            }

            return caseType;
        }

        public async Task RemoveVideoHearing(Guid hearingId)
        {
            using (var db = new BookingsDbContext(_dbContextOptions))
            {
                var hearing = await db.VideoHearings.FindAsync(hearingId);
                db.Remove(hearing);
                await db.SaveChangesAsync();
            }
        }
    }
}