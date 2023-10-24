using BookingsApi.Common.Services;
using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Exceptions;
using BookingsApi.DAL.Helper;
using BookingsApi.DAL.Queries;
using BookingsApi.DAL.Services;
using BookingsApi.Domain.Constants;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.Participants;
using BookingsApi.Domain.RefData;
using Faker;
using Microsoft.Extensions.Configuration;
using NuGet.Packaging;
using Testing.Common.Builders.Domain;
using Testing.Common.Configuration;

namespace BookingsApi.IntegrationTests.Helper
{
    public class TestDataManager
    {
        private readonly DbContextOptions<BookingsDbContext> _dbContextOptions;
        private readonly List<Guid> _seededHearings = new();
        public List<string> JudiciaryPersons { get; } = new();
        private readonly List<Guid> _seededJusticeUserIds = new();
        private readonly List<long> _seededAllocationIds = new();
        public static string CaseNumber => "2222/3511";
        private readonly string _defaultCaseName;

        public TestDataManager(DbContextOptions<BookingsDbContext> dbContextOptions, string defaultCaseName)
        {
            _dbContextOptions = dbContextOptions;
            _defaultCaseName = defaultCaseName;
        }

        public void AddHearingForCleanup(Guid id)
        {
            _seededHearings.Add(id);
        }

        public void AddJusticeUserForCleanup(Guid id)
        {
            _seededJusticeUserIds.Add(id);
        }

        public void AddJudiciaryPersonsForCleanup(params string[] ids)
        {
            JudiciaryPersons.AddRange(ids);
        }

        public async Task<JusticeUser> SeedJusticeUser(string userName, string firstName, string lastName,
            bool isTeamLead = false, bool isDeleted = false, bool initWorkHours = true)
        {
            await using var db = new BookingsDbContext(_dbContextOptions);

            var justiceUser = new JusticeUser(firstName, lastName, userName, userName)
            {
                CreatedBy = "integration.test@test.com",
                CreatedDate = DateTime.UtcNow,
            };

            var userRoles = await db.UserRoles.ToListAsync();

            if (isTeamLead)
            {
                var teamLeadRole = userRoles.First(x => x.Id == (int) UserRoleId.VhTeamLead);
                justiceUser.AddRoles(teamLeadRole);
            }
            else
            {
                var vhoRole = userRoles.First(x => x.Id == (int) UserRoleId.Vho);
                justiceUser.AddRoles(vhoRole);
            }

            if (initWorkHours)
            {
                justiceUser.VhoWorkHours.AddRange(new List<VhoWorkHours>()
                {
                    new() {DayOfWeekId = 1, StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(17, 0, 0)},
                    new() {DayOfWeekId = 2, StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(17, 0, 0)},
                    new() {DayOfWeekId = 3, StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(17, 0, 0)},
                    new() {DayOfWeekId = 4, StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(17, 0, 0)},
                    new() {DayOfWeekId = 5, StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(17, 0, 0)},
                    new() {DayOfWeekId = 6, StartTime = null, EndTime = null},
                    new() {DayOfWeekId = 7, StartTime = null, EndTime = null},

                });
            }

            if (isDeleted)
            {
                justiceUser.Delete();
            }

            await db.JusticeUsers.AddAsync(justiceUser);
            await db.SaveChangesAsync();

            _seededJusticeUserIds.Add(justiceUser.Id);

            return justiceUser;
        }

        public async Task<JusticeUser> SeedAllocatedJusticeUser(string userName, string firstName, string lastName)
        {
            await using var db = new BookingsDbContext(_dbContextOptions);

            var justiceUser = db.JusticeUsers.Add(new JusticeUser
            {
                ContactEmail = userName,
                Username = userName,
                CreatedBy = "integration.test@test.com",
                CreatedDate = DateTime.Now,
                FirstName = firstName,
                Lastname = lastName,
            });
            var hearing = await db.VideoHearings.Include(x => x.Allocations).ThenInclude(x => x.JusticeUser)
                .FirstAsync();

            hearing.AllocateJusticeUser(justiceUser.Entity);

            _seededJusticeUserIds.Add(justiceUser.Entity.Id);
            await SeedJusticeUsersRole(db, justiceUser.Entity, (int) UserRoleId.Vho);
            await db.SaveChangesAsync();
            return justiceUser.Entity;
        }

        public static async Task SeedJusticeUsersRole(BookingsDbContext context, JusticeUser user, params int[] roleIds)
        {
            var userRoles = await context.UserRoles.Where(x => roleIds.Contains(x.Id)).ToListAsync();
            var entities = userRoles.Select(ur => new JusticeUserRole(user, ur)).ToArray();
            await context.AddRangeAsync(entities);
        }

        /// <summary>
        /// Use when testing V2 endpoints, which use a flat hearing role structure
        /// </summary>
        public async Task<VideoHearing> SeedVideoHearingV2(Action<SeedVideoHearingOptions> configureOptions = null,
            BookingStatus status = BookingStatus.Booked, bool withLinkedParticipants = false,
            bool isMultiDayFirstHearing = false)
        {
            return await SeedVideoHearing(true, configureOptions, status, withLinkedParticipants,
                isMultiDayFirstHearing);
        }

        /// <summary>
        /// Use when testing V1 endpoints
        /// </summary>
        public async Task<VideoHearing> SeedVideoHearing(Action<SeedVideoHearingOptions> configureOptions = null,
            BookingStatus status = BookingStatus.Booked,
            bool withLinkedParticipants = false,
            bool isMultiDayFirstHearing = false)
        {
            return await SeedVideoHearing(false, configureOptions, status, withLinkedParticipants,
                isMultiDayFirstHearing);
        }

        private async Task<VideoHearing> SeedVideoHearing(bool useFlatHearingRoles,
            Action<SeedVideoHearingOptions> configureOptions = null,
            BookingStatus status = BookingStatus.Booked, bool withLinkedParticipants = false,
            bool isMultiDayFirstHearing = false)
        {
            var options = new SeedVideoHearingOptions();
            configureOptions?.Invoke(options);
            var caseType = GetCaseTypeFromDb(options.CaseTypeName);
            var hearingType = options.ExcludeHearingType
                ? null
                : caseType.HearingTypes.First(x => x.Name == options.HearingTypeName);

            await using var db = new BookingsDbContext(_dbContextOptions);
            
            var venue = options.HearingVenue ?? new RefDataBuilder().HearingVenues[0];
            var hearingVenue = await db.Venues.FirstOrDefaultAsync(x => x.Id == venue.Id);
            var scheduledDate = options.ScheduledDate ?? DateTime.Today.AddDays(1).AddHours(10).AddMinutes(30);
            var videoHearing = InitVideoHearing(scheduledDate, hearingVenue, caseType, hearingType, options.Case);

            await AddParticipantsToVideoHearing(videoHearing, caseType, useFlatHearingRoles, options);
            videoHearing.IsFirstDayOfMultiDayHearing = isMultiDayFirstHearing;

            if (options.EndpointsToAdd > 0)
            {
                var r = new RandomGenerator();
                for (int i = 0; i < options.EndpointsToAdd; i++)
                {
                    var sip = r.GetWeakDeterministic(DateTime.UtcNow.Ticks, 1, 10);
                    var pin = r.GetWeakDeterministic(DateTime.UtcNow.Ticks, 1, 4);
                    videoHearing.AddEndpoints(new List<Endpoint>
                    {
                        new Endpoint($"new endpoint {i}", $"{sip}@hmcts.net", pin, null)
                    });
                }
            }

            if (withLinkedParticipants)
            {
                var interpretee = videoHearing.Participants[0];
                var interpreter = videoHearing.Participants[1];
                CreateParticipantLinks(interpretee, interpreter);
            }

            // do we need this as default if we have the above as an option?
            var defenceAdvocate = videoHearing.Participants.First(x => x is Representative);
            videoHearing.AddEndpoints(
                new List<Endpoint>
                {
                    new("new endpoint", Guid.NewGuid().ToString(), "pin", null),
                    new("new endpoint", Guid.NewGuid().ToString(), "pin", defenceAdvocate),
                });

            if (status != BookingStatus.Booked)
            {
                videoHearing.UpdateStatus(status, "automated@test.com", "test");
            }
            
            // then add judge if requested (need to use the db same context here to avoid conflicts)
            if (options.AddJudge)
            {
                await AddJudgeToVideoHearing(videoHearing, caseType, useFlatHearingRoles, db);
            }

            // then add panel member if requested (need to use the db same context here to avoid conflicts)
            if (options.AddPanelMember)
            {
                await AddPanelMemberToVideoHearing(videoHearing, caseType, useFlatHearingRoles, db);
            }

            await db.VideoHearings.AddAsync(videoHearing);
            await db.SaveChangesAsync();

            var hearing = await new GetHearingByIdQueryHandler(db).Handle(new GetHearingByIdQuery(videoHearing.Id));

            _seededHearings.Add(hearing.Id);
            return hearing;
        }

        private async Task AddParticipantsToVideoHearing(VideoHearing videoHearing, CaseType caseType,
            bool useFlatHearingRoles, SeedVideoHearingOptions options)
        {
            await using var db = new BookingsDbContext(_dbContextOptions);

            HearingRole applicantLipHearingRole;
            HearingRole applicantRepresentativeHearingRole;
            HearingRole respondentRepresentativeHearingRole;
            HearingRole respondentLipHearingRole;

            var applicantCaseRole = caseType.CaseRoles.First(x => x.Name == options.ApplicantRole);
            var respondentCaseRole = caseType.CaseRoles.First(x => x.Name == options.RespondentRole);

            var flatHearingRoles = GetFlatHearingRolesFromDb();

            if (useFlatHearingRoles)
            {
                applicantLipHearingRole = flatHearingRoles.First(x => x.Code == HearingRoleCodes.Applicant);
                applicantRepresentativeHearingRole =
                    flatHearingRoles.First(x => x.Code == HearingRoleCodes.Representative);
                respondentRepresentativeHearingRole =
                    flatHearingRoles.First(x => x.Code == HearingRoleCodes.WelfareRepresentative);
                respondentLipHearingRole = flatHearingRoles.First(x => x.Code == HearingRoleCodes.Respondent);
            }
            else
            {
                applicantLipHearingRole = applicantCaseRole.HearingRoles.First(x => x.Name == options.LipHearingRole);
                applicantRepresentativeHearingRole =
                    applicantCaseRole.HearingRoles.First(x => x.Name == "Representative");
                respondentRepresentativeHearingRole =
                    respondentCaseRole.HearingRoles.First(x => x.Name == "Representative");
                respondentLipHearingRole = respondentCaseRole.HearingRoles.First(x => x.Name == options.LipHearingRole);
            }

            var person1 = new PersonBuilder(true).WithOrganisation().Build();
            var person2 = new PersonBuilder(true).Build();
            var person3 = new PersonBuilder(true).Build();
            var person4 = new PersonBuilder($"Automation/{RandomNumber.Next()}@hmcts.net").Build();

            videoHearing.AddIndividual(person1, applicantLipHearingRole, applicantCaseRole,
                $"{person1.FirstName} {person1.LastName}");

            videoHearing.AddRepresentative(person2, applicantRepresentativeHearingRole, applicantCaseRole,
                $"{person2.FirstName} {person2.LastName}", "Ms X");

            videoHearing.AddRepresentative(person3, respondentRepresentativeHearingRole, respondentCaseRole,
                $"{person3.FirstName} {person3.LastName}", "Ms Y");

            videoHearing.AddIndividual(person4, respondentLipHearingRole, respondentCaseRole,
                $"{person4.FirstName} {person4.LastName}");

            if (options.AddStaffMember)
            {
                var staffMemberPerson = new PersonBuilder(true).Build();
                var staffMemberCaseRole = caseType.CaseRoles.First(x => x.Name == "Staff Member");

                var staffMemberHearingRole = useFlatHearingRoles
                    ? flatHearingRoles.First(x => x.Code == HearingRoleCodes.StaffMember)
                    : staffMemberCaseRole.HearingRoles.First(x => x.Name == HearingRoles.StaffMember);
                videoHearing.AddStaffMember(staffMemberPerson, staffMemberHearingRole, staffMemberCaseRole,
                    "Staff Member 1");
            }
        }

        private async Task AddPanelMemberToVideoHearing(VideoHearing videoHearing, CaseType caseType,
            bool useFlatHearingRoles,
            BookingsDbContext db)
        {
            if (useFlatHearingRoles)
            {
                var personalCode = Guid.NewGuid().ToString();
                var judiciaryPanelMemberPerson = await AddJudiciaryPerson(db, personalCode: personalCode);
                videoHearing.AddJudiciaryPanelMember(judiciaryPanelMemberPerson, "Judiciary Panel Member");
            }
            else
            {
                var johPerson = new PersonBuilder(true).Build();
                var johCaseRole = caseType.CaseRoles.First(x => x.Name == "Panel Member");
                var johHearingRole = johCaseRole.HearingRoles.First(x => x.Name == "Panel Member");
                videoHearing.AddJudicialOfficeHolder(johPerson, johHearingRole, johCaseRole,
                    $"{johPerson.FirstName} {johPerson.LastName}");
            }
        }

        private async Task AddJudgeToVideoHearing(VideoHearing videoHearing, CaseType caseType,
            bool useFlatHearingRoles,
            BookingsDbContext db)
        {
            if (useFlatHearingRoles)
            {
                var personalCode = Guid.NewGuid().ToString();
                var judiciaryJudge = await AddJudiciaryPerson(db, personalCode: personalCode);
                videoHearing.AddJudiciaryJudge(judiciaryJudge, "Judiciary Judge");
            }
            else
            {
                var judgePerson = new PersonBuilder(true).Build();
                var judgeCaseRole = caseType.CaseRoles.First(x => x.Name == "Judge");
                var judgeHearingRole = judgeCaseRole.HearingRoles.First(x => x.Name == "Judge");
                videoHearing.AddJudge(judgePerson, judgeHearingRole, judgeCaseRole,
                    $"{judgePerson.FirstName} {judgePerson.LastName}");
            }
        }

        private VideoHearing InitVideoHearing(DateTime scheduledDate, HearingVenue venue, CaseType caseType,
            HearingType hearingType, Case @case)
        {
            const int duration = 45;
            const string hearingRoomName = "Room02";
            const string otherInformation = "OtherInformation02";
            const string createdBy = "test@hmcts.net";
            const bool audioRecordingRequired = true;
            const string cancelReason = "Online abandonment (incomplete registration)";

            var videoHearing = new VideoHearing(caseType, hearingType, scheduledDate, duration,
                venue, hearingRoomName, otherInformation, createdBy, audioRecordingRequired, cancelReason);

            if (@case == null)
            {
                videoHearing.AddCase(CaseNumber, $"{_defaultCaseName} {RandomNumber.Next(900000, 999999)}", true);
            }
            else
            {
                videoHearing.AddCase(@case.Number, @case.Name, true);
            }

            return videoHearing;
        }

        public async Task CloneVideoHearing(Guid hearingId, IList<DateTime> datesOfHearing,
            BookingStatus status = BookingStatus.Booked)
        {
            var dbContext = new BookingsDbContext(_dbContextOptions);
            var hearing = await new GetHearingByIdQueryHandler(dbContext)
                .Handle(new GetHearingByIdQuery(hearingId));

            var orderedDates = datesOfHearing.OrderBy(x => x).ToList();
            var totalDays = orderedDates.Count + 1;
            var commands = orderedDates.Select((newDate, index) =>
            {
                var config = ConfigRootBuilder.Build();
                var hearingDay = index + 2;
                return CloneHearingToCommandMapper.CloneToCommand(hearing, newDate, new RandomGenerator(),
                    config.GetValue<string>("KinlyConfiguration:SipAddressStem"), totalDays, hearingDay);
            }).ToList();

            foreach (var command in commands)
            {
                await new CreateVideoHearingCommandHandler(dbContext, new HearingService(dbContext)).Handle(command);
                var returnedHearing = dbContext.VideoHearings.First(x => x.Id == command.NewHearingId);
                if (status != BookingStatus.Booked)
                    // By default, hearings is set with booked status, it is transitioned into created status when queue subscriber updates it
                    // Need to update the status to set a required status to test different scenarios.
                {
                    returnedHearing.UpdateStatus(status, "test", "test");
                    await dbContext.SaveChangesAsync();
                }
            }
        }

        private CaseType GetCaseTypeFromDb(string caseTypeName)
        {
            using var db = new BookingsDbContext(_dbContextOptions);
            var caseType = db.CaseTypes
                .Include(x => x.CaseRoles)
                .ThenInclude(x => x.HearingRoles)
                .ThenInclude(x => x.UserRole)
                .Include(x => x.HearingTypes)
                .FirstOrDefault(x => x.Name == caseTypeName);

            if (caseType == null)
            {
                throw new InvalidOperationException("Unknown case type: " + caseTypeName);
            }

            return caseType;
        }

        private List<HearingRole> GetFlatHearingRolesFromDb()
        {
            using var db = new BookingsDbContext(_dbContextOptions);
            var hearingRoles = db.HearingRoles
                .Where(x => x.CaseRoleId == null)
                .ToList();

            return hearingRoles;
        }

        private static void CreateParticipantLinks(Participant interpretee, Participant interpreter)
        {
            interpretee.LinkedParticipants.Add(new LinkedParticipant(interpretee.Id, interpreter.Id,
                LinkedParticipantType.Interpreter));
            interpreter.LinkedParticipants.Add(new LinkedParticipant(interpreter.Id, interpretee.Id,
                LinkedParticipantType.Interpreter));
        }

        public DateTime? GetJobLastRunDateTime(string jobName)
        {
            DateTime? lastUpdateDateTime;
            using (var db = new BookingsDbContext(_dbContextOptions))
            {
                lastUpdateDateTime = db.JobHistory
                    .Where(e => e.JobName == jobName && e.IsSuccessful)
                    .OrderByDescending(e => e.LastRunDate)
                    .FirstOrDefault()?
                    .LastRunDate;

            }

            return lastUpdateDateTime;
        }

        public async Task ClearSeededHearings()
        {
            foreach (var hearingId in _seededHearings)
            {
                await RemoveVideoHearing(hearingId);
            }

            _seededHearings.Clear();
        }

        public async Task ClearJudiciaryPersonsAsync()
        {
            foreach (var person in JudiciaryPersons)
            {
                try
                {
                    await using var db = new BookingsDbContext(_dbContextOptions);
                    var jp = await db.JudiciaryPersons.SingleOrDefaultAsync(x => x.PersonalCode == person);
                    if (jp != null)
                    {
                        db.JudiciaryPersons.Remove(jp);
                        await db.SaveChangesAsync();
                    }

                    TestContext.WriteLine(@$"Remove Judiciary Person: {person}.");
                }
                catch (JudiciaryPersonNotFoundException)
                {
                    TestContext.WriteLine(@$"Ignoring cleanup for Judiciary Person: {person}. Does not exist.");
                }
            }
        }

        public async Task ClearSeededJusticeUsersAsync()
        {
            foreach (var id in _seededJusticeUserIds)
            {
                try
                {
                    await using var db = new BookingsDbContext(_dbContextOptions);
                    var justiceUser = await db.JusticeUsers.Include(x => x.JusticeUserRoles)
                        .Include(x => x.VhoWorkHours)
                        .Include(x => x.VhoNonAvailability)
                        .IgnoreQueryFilters().SingleOrDefaultAsync(x => x.Id == id);
                    if (justiceUser != null)
                    {
                        db.JusticeUsers.Remove(justiceUser);
                        await db.SaveChangesAsync();
                    }

                    TestContext.WriteLine(@$"Remove Justice User: {id}.");
                }
                catch (JusticeUserNotFoundException)
                {
                    TestContext.WriteLine(@$"Ignoring cleanup for Justice User: {id}. Does not exist.");
                }
            }

            _seededJusticeUserIds.Clear();
        }

        public async Task ClearAllJusticeUsersAsync()
        {
            try
            {
                await using var db = new BookingsDbContext(_dbContextOptions);
                var list = new List<JusticeUser>();
                var users = await db.JusticeUsers.Include(x => x.JusticeUserRoles).Include(x => x.VhoWorkHours)
                    .Include(x => x.VhoNonAvailability).IgnoreQueryFilters().ToListAsync();
                foreach (var user in users)
                {
                    list.Add(user);
                }

                foreach (var user in list)
                {
                    db.JusticeUsers.Remove(user);
                    await db.SaveChangesAsync();
                    TestContext.WriteLine(@$"Remove Justice User: {user.Id}.");
                }


            }
            catch (JudiciaryPersonNotFoundException)
            {
                TestContext.WriteLine(@"Ignoring cleanup for Justice User. Does not exist.");
            }
        }

        public async Task RemoveVideoHearing(Guid hearingId)
        {
            try
            {
                await using var db = new BookingsDbContext(_dbContextOptions);
                await new RemoveHearingCommandHandler(db).Handle(new RemoveHearingCommand(hearingId));
                TestContext.WriteLine(@$"Removed hearing: {hearingId}.");
            }
            catch (HearingNotFoundException)
            {
                TestContext.WriteLine(@$"Ignoring cleanup for: {hearingId}. Does not exist.");
            }
        }

        public async Task ClearUnattachedPersons(IEnumerable<string> removedPersons)
        {
            foreach (var personEmail in removedPersons)
            {
                await using var db = new BookingsDbContext(_dbContextOptions);
                var person = await db.Persons
                    .Include(x => x.Organisation)
                    .SingleOrDefaultAsync(x => x.ContactEmail == personEmail);

                if (person == null) return;
                if (person.Organisation != null) db.Remove(person.Organisation);
                db.Remove(person);

                await db.SaveChangesAsync();
            }
        }

        public static string[] GetIndividualHearingRoles => new[] {"Litigant in person"};

        public Task<VideoHearing> SeedPastHearings(DateTime scheduledDate)
        {
            return SeedPastVideoHearing(scheduledDate, null);
        }

        public async Task<VideoHearing> SeedPastVideoHearing(DateTime pastScheduledDate,
            Action<SeedVideoHearingOptions> configureOptions, BookingStatus status = BookingStatus.Booked,
            bool isMultiDayFirstHearing = false)
        {
            var options = new SeedVideoHearingOptions();
            configureOptions?.Invoke(options);
            options.AddJudge = true;
            options.AddPanelMember = true;
            options.EndpointsToAdd = 0;

            var caseType = GetCaseTypeFromDb(options.CaseTypeName);
            var hearingType = options.ExcludeHearingType
                ? null
                : caseType.HearingTypes.First(x => x.Name == options.HearingTypeName);

            await using var db = new BookingsDbContext(_dbContextOptions);
            
            var venue = options.HearingVenue ?? new RefDataBuilder().HearingVenues[0];
            var hearingVenue = await db.Venues.FirstOrDefaultAsync(x => x.Id == venue.Id);

            // create a hearing in the future to avoid validation failures and use reflection to update the scheduled date
            var scheduledDate = DateTime.Today.AddDays(1).AddHours(10).AddMinutes(30);
            var videoHearing = InitVideoHearing(scheduledDate, hearingVenue, caseType, hearingType, options.Case);
            var videoHearingType = typeof(VideoHearing);
            videoHearingType.GetProperty("ScheduledDateTime")?.SetValue(videoHearing, pastScheduledDate);

            await AddParticipantsToVideoHearing(videoHearing, caseType, useFlatHearingRoles: false, options);

            if (status == BookingStatus.Created)
            {
                videoHearing.UpdateStatus(BookingStatus.Created, "automated@test.com", null);
            }

            // then add judge if requested (need to use the db same context here to avoid conflicts)
            if (options.AddJudge)
            {
                await AddJudgeToVideoHearing(videoHearing, caseType, false, db);
            }

            // then add panel member if requested (need to use the db same context here to avoid conflicts)
            if (options.AddPanelMember)
            {
                await AddPanelMemberToVideoHearing(videoHearing, caseType, false, db);
            }

            videoHearing.IsFirstDayOfMultiDayHearing = isMultiDayFirstHearing;


            await db.VideoHearings.AddAsync(videoHearing);
            await db.SaveChangesAsync();


            var hearing = await new GetHearingByIdQueryHandler(new BookingsDbContext(_dbContextOptions)).Handle(
                new GetHearingByIdQuery(videoHearing.Id));
            _seededHearings.Add(hearing.Id);
            return hearing;
        }

        public async Task<JudiciaryPersonStaging> AddJudiciaryPersonStaging()
        {
            await using var db = new BookingsDbContext(_dbContextOptions);

            var judiciaryPersonStaging =
                new JudiciaryPersonStaging(
                    Name.First(),
                    Name.First(),
                    Name.First(),
                    Name.First(),
                    Name.First(),
                    Name.First(),
                    Name.First(),
                    Name.First(),
                    Name.First(),
                    Name.First());
            await db.JudiciaryPersonsStaging.AddAsync(judiciaryPersonStaging);

            await db.SaveChangesAsync();
            return judiciaryPersonStaging;
        }

        public async Task<JudiciaryPerson> AddJudiciaryPerson(string personalCode = null)
        {
            await using var db = new BookingsDbContext(_dbContextOptions);

            var judiciaryPerson = new JudiciaryPersonBuilder(personalCode).Build();
            await db.JudiciaryPersons.AddAsync(judiciaryPerson);

            await db.SaveChangesAsync();
            AddJudiciaryPersonsForCleanup(judiciaryPerson.PersonalCode);

            return judiciaryPerson;
        }

        public async Task<JudiciaryPerson> AddJudiciaryPerson(BookingsDbContext db, string personalCode = null)
        {
            var judiciaryPerson = new JudiciaryPersonBuilder(personalCode).Build();
            await db.JudiciaryPersons.AddAsync(judiciaryPerson);

            await db.SaveChangesAsync();
            AddJudiciaryPersonsForCleanup(judiciaryPerson.PersonalCode);

            var person = db.JudiciaryPersons.FirstOrDefault(x => x.PersonalCode == personalCode);

            return person;
        }

        public async Task RemoveJudiciaryPersonAsync(JudiciaryPerson judiciaryPerson)
        {
            await using var db = new BookingsDbContext(_dbContextOptions);

            db.JudiciaryPersons.Remove(judiciaryPerson);

            await db.SaveChangesAsync();
        }

        public async Task<Allocation> AddAllocation(VideoHearing hearing, JusticeUser user)
        {
            user ??= await SeedJusticeUser(userName: "testUser", null, null, isTeamLead: true);

            await using var db = new BookingsDbContext(_dbContextOptions);
            var dbHearing = await db.VideoHearings.Include(x => x.Allocations).ThenInclude(a => a.JusticeUser)
                .ThenInclude(x => x.Allocations)
                .FirstAsync(x => x.Id == hearing.Id);
            dbHearing.AllocateJusticeUser(user);
            await db.SaveChangesAsync();

            var allocation = dbHearing.Allocations.First(x => x.JusticeUserId == user.Id);
            _seededAllocationIds.Add(allocation.Id);
            return allocation;
        }

        public async Task ClearAllocationsAsync()
        {
            await using var db = new BookingsDbContext(_dbContextOptions);
            var seededForRemoval = await db.VideoHearings.Include(x => x.Allocations).Where(x =>
                x.Allocations.Any(allocation => _seededAllocationIds.Contains(allocation.Id))).ToListAsync();

            seededForRemoval.ForEach(vh => vh.Deallocate());
            await db.SaveChangesAsync();
        }
    }
}