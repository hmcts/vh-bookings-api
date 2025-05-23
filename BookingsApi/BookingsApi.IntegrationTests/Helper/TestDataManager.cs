using Bogus;
using BookingsApi.Common.Services;
using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Exceptions;
using BookingsApi.DAL.Helper;
using BookingsApi.DAL.Queries;
using BookingsApi.DAL.Queries.BaseQueries;
using BookingsApi.DAL.Services;
using BookingsApi.Domain.Constants;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.Participants;
using BookingsApi.Domain.RefData;
using Microsoft.Extensions.Configuration;
using NuGet.Packaging;
using Testing.Common.Builders.Domain;
using Testing.Common.Configuration;
using JobHistory = BookingsApi.Domain.JobHistory;
using Person = BookingsApi.Domain.Person;

namespace BookingsApi.IntegrationTests.Helper
{
    public class TestDataManager(
        DbContextOptions<BookingsDbContext> dbContextOptions,
        string defaultCaseName)
    {
        private readonly List<Guid> _seededHearings = new();
        public List<string> JudiciaryPersons { get; } = new();
        private readonly List<Guid> _seededJusticeUserIds = new();
        private readonly List<Guid> _seededPersonIds = new();
        private readonly List<long> _seededAllocationIds = new();
        private readonly List<Guid> _seededJobHistoryIds = [];
        public static string CaseNumber => "2222/3511";
        private static readonly Faker Faker = new();

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
            bool isTeamLead = false, bool isDeleted = false, bool initWorkHours = true, bool initNonAvailabilities = false)
        {
            await using var db = new BookingsDbContext(dbContextOptions);

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

            if (initNonAvailabilities)
            {
                var date = DateTime.UtcNow.Date;
                var tomorrow = date.AddDays(1);
                justiceUser.AddOrUpdateNonAvailability(date.AddHours(6), date.AddHours(8));
                justiceUser.AddOrUpdateNonAvailability(tomorrow.AddHours(6), tomorrow.AddHours(8));
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

        public async Task<Person> SeedJudgePerson(string title,string userName, string firstName, string lastName, string contactEmail, string telpehoneNumber)
        {
            await using var db = new BookingsDbContext(dbContextOptions);
            var person = await db.Persons.SingleOrDefaultAsync(x => x.ContactEmail == contactEmail);
            if(person == null)
            {
                await db.Persons.AddAsync(new Person(title, firstName, lastName, contactEmail, userName));
                await db.SaveChangesAsync();
            }

            return person;
        }

        public async Task<JusticeUser> SeedAllocatedJusticeUser(string userName, string firstName, string lastName)
        {
            await using var db = new BookingsDbContext(dbContextOptions);

            var justiceUser = await db.JusticeUsers.AddAsync(new JusticeUser
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
            return await SeedVideoHearing(configureOptions, status, withLinkedParticipants,
                isMultiDayFirstHearing);
        }

        private async Task<VideoHearing> SeedVideoHearing(Action<SeedVideoHearingOptions> configureOptions = null,
            BookingStatus status = BookingStatus.Booked, bool withLinkedParticipants = false,
            bool isMultiDayFirstHearing = false)
        {
            var options = new SeedVideoHearingOptions();
            configureOptions?.Invoke(options);
            var caseType = GetCaseTypeFromDb(options.CaseTypeName);
            
            await using var db = new BookingsDbContext(dbContextOptions);
            
            var venue = options.HearingVenue ?? new RefDataBuilder().HearingVenues[0];
            var hearingVenue = await db.Venues.FirstOrDefaultAsync(x => x.Id == venue.Id);
            var scheduledDate = options.ScheduledDate ?? DateTime.Today.AddDays(1).AddHours(10).AddMinutes(30);
            
            var videoHearing = InitVideoHearing(scheduledDate, hearingVenue, caseType, options.Case, options.ScheduledDuration, options.AudioRecordingRequired);

            await AddParticipantsToVideoHearing(videoHearing);
            videoHearing.IsFirstDayOfMultiDayHearing = isMultiDayFirstHearing;

            if (options.EndpointsToAdd > 0)
            {
                var r = new RandomGenerator();
                for (var i = 0; i < options.EndpointsToAdd; i++)
                {
                    var sip = r.GetWeakDeterministic(DateTime.UtcNow.Ticks, 1, 10);
                    var pin = r.GetWeakDeterministic(DateTime.UtcNow.Ticks, 1, 4);
                    videoHearing.AddEndpoints(new List<Endpoint>
                    {
                        new(Guid.NewGuid().ToString(), $"new endpoint {i}", $"{sip}@hmcts.net", pin)
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
                    new(Guid.NewGuid().ToString(),"new endpoint 01", Guid.NewGuid().ToString(), "pin"),
                    new(Guid.NewGuid().ToString(),"new endpoint 02", Guid.NewGuid().ToString(), "pin", defenceAdvocate),
                });

            if (status != BookingStatus.Booked)
            {
                videoHearing.UpdateStatus(status, "automated@test.com", "test");
            }
            
            // then add judge if requested (need to use the db same context here to avoid conflicts)
            if (options.AddJudge)
            {
                await AddJudgeToVideoHearing(videoHearing, db);
            }

            // then add panel member if requested (need to use the db same context here to avoid conflicts)
            if (options.AddPanelMember)
            {
                await AddPanelMemberToVideoHearing(videoHearing, db);
            }

            if (options.AddScreening)
            {
                var individuals = videoHearing.Participants.Where(x => x is Individual).ToList();
                var endpoints = videoHearing.Endpoints.ToList();
                videoHearing.AssignScreeningForParticipant(individuals[0], ScreeningType.Specific,
                    [individuals[1].ExternalReferenceId, endpoints[0].ExternalReferenceId]);
                videoHearing.AssignScreeningForEndpoint(endpoints[0], ScreeningType.Specific,
                    [individuals[1].ExternalReferenceId]);
            }

            if (options.AddInterpreterLanguages)
            {
                var endpoints = videoHearing.Endpoints.ToList();
                var language = await db.InterpreterLanguages.FirstAsync(l => l.Code == "spa");
                endpoints[0].UpdateLanguagePreferences(language, null);
            }

            await db.VideoHearings.AddAsync(videoHearing);
            await db.SaveChangesAsync();

            var hearing = await new GetHearingByIdQueryHandler(db).Handle(new GetHearingByIdQuery(videoHearing.Id));

            _seededHearings.Add(hearing.Id);
            return hearing;
        }

        private async Task AddParticipantsToVideoHearing(VideoHearing videoHearing)
        {
            await using var db = new BookingsDbContext(dbContextOptions);

            var flatHearingRoles = GetFlatHearingRolesFromDb();

            var person1 = new PersonBuilder(true).WithOrganisation().Build();
            var person2 = new PersonBuilder(true).Build();
            var person3 = new PersonBuilder(true).Build();
            var person4 = new PersonBuilder($"Automation/{Faker.Random.Number(0, 9999999)}@hmcts.net").Build();

            var applicantLipHearingRole = flatHearingRoles.First(x => x.Code == HearingRoleCodes.Applicant);
            var applicantRepresentativeHearingRole = flatHearingRoles.First(x => x.Code == HearingRoleCodes.Representative);
            var respondentRepresentativeHearingRole = flatHearingRoles.First(x => x.Code == HearingRoleCodes.WelfareRepresentative);
            var respondentLipHearingRole = flatHearingRoles.First(x => x.Code == HearingRoleCodes.Respondent);

            videoHearing.AddIndividual(Guid.NewGuid().ToString(), person1, applicantLipHearingRole,
                $"{person1.FirstName} {person1.LastName}");

            videoHearing.AddRepresentative(Guid.NewGuid().ToString(), person2, applicantRepresentativeHearingRole,
                $"{person2.FirstName} {person2.LastName}", "Ms X");

            videoHearing.AddRepresentative(Guid.NewGuid().ToString(), person3, respondentRepresentativeHearingRole,
                $"{person3.FirstName} {person3.LastName}", "Ms Y");

            videoHearing.AddIndividual(Guid.NewGuid().ToString(), person4, respondentLipHearingRole,
                $"{person4.FirstName} {person4.LastName}");
        }

        private async Task AddPanelMemberToVideoHearing(VideoHearing videoHearing, BookingsDbContext db)
        {
            var personalCode = Guid.NewGuid().ToString();
            var judiciaryPanelMemberPerson = await AddJudiciaryPerson(db, personalCode: personalCode);
            videoHearing.AddJudiciaryPanelMember(judiciaryPanelMemberPerson, "Judiciary Panel Member");
        }

        private async Task AddJudgeToVideoHearing(VideoHearing videoHearing, BookingsDbContext db)
        {
            var personalCode = Guid.NewGuid().ToString();
            var judiciaryJudge = await AddJudiciaryPerson(db, personalCode: personalCode);
            videoHearing.AddJudiciaryJudge(judiciaryJudge, "Judiciary Judge");
        }

        private VideoHearing InitVideoHearing(DateTime scheduledDate, HearingVenue venue, CaseType caseType, Case @case,
            int duration, bool audioRecordingRequired)
        {
            const string hearingRoomName = "Room02";
            const string otherInformation = "OtherInformation02";
            const string createdBy = "test@hmcts.net";

            var videoHearing = new VideoHearing(caseType, scheduledDate, duration,
                venue, hearingRoomName, otherInformation, createdBy, audioRecordingRequired);

            if (@case == null)
            {
                videoHearing.AddCase(CaseNumber, $"{defaultCaseName} {Faker.Random.Number(0, 999999)}", true);
            }
            else
            {
                videoHearing.AddCase(@case.Number, @case.Name, true);
            }

            return videoHearing;
        }

        public async Task CloneVideoHearing(Guid hearingId, IList<DateTime> datesOfHearing,
            BookingStatus status = BookingStatus.Booked, int duration = Contract.V1.Constants.CloneHearings.DefaultScheduledDuration)
        {
            var dbContext = new BookingsDbContext(dbContextOptions);
            var hearing = await new GetHearingByIdQueryHandler(dbContext)
                .Handle(new GetHearingByIdQuery(hearingId));

            var orderedDates = datesOfHearing.OrderBy(x => x).ToList();
            var totalDays = orderedDates.Count + 1;
            var commands = orderedDates.Select((newDate, index) =>
            {
                var config = ConfigRootBuilder.Build();
                var hearingDay = index + 2;
                var stem = config.GetValue<string>("SupplierConfiguration:SipAddressStemVodafone");
                return CloneHearingToCommandMapper.CloneToCommand(hearing, newDate, new RandomGenerator(), stem,
                    totalDays, hearingDay, duration);
            }).ToList();

            foreach (var command in commands)
            {
                await new CreateVideoHearingCommandHandler(dbContext, new HearingService(dbContext)).Handle(command);
                var returnedHearing = await dbContext.VideoHearings.FirstAsync(x => x.Id == command.NewHearingId);
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
            using var db = new BookingsDbContext(dbContextOptions);
            var caseTypes = CaseTypes.Get(db)
                .AsNoTracking()
                .Where(x => x.Name == caseTypeName).ToList();
            var caseType = caseTypes[0];
            if (caseType == null)
            {
                throw new InvalidOperationException("Unknown case type: " + caseTypeName);
            }

            return caseType;
        }

        private List<HearingRole> GetFlatHearingRolesFromDb()
        {
            using var db = new BookingsDbContext(dbContextOptions);
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
            using (var db = new BookingsDbContext(dbContextOptions))
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
                    await using var db = new BookingsDbContext(dbContextOptions);
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
                    await using var db = new BookingsDbContext(dbContextOptions);
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
        
        public async Task ClearSeededPersonsAsync()
        {
            foreach (var id in _seededPersonIds)
            {
                try
                {
                    await using var db = new BookingsDbContext(dbContextOptions);
                    var persons = await db.Persons.Where(x => x.Id == id).ToListAsync();
                    var person = persons.FirstOrDefault();
                    if (person != null)
                    {
                        db.Persons.Remove(person);
                        await db.SaveChangesAsync();
                    }

                    TestContext.WriteLine(@$"Remove Person: {id}.");
                }
                catch (PersonNotFoundException)
                {
                    TestContext.WriteLine(@$"Ignoring cleanup for Person: {id}. Does not exist.");
                }
            }

            _seededPersonIds.Clear();
        }

        public async Task ClearAllJusticeUsersAsync()
        {
            try
            {
                await using var db = new BookingsDbContext(dbContextOptions);
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
                await using var db = new BookingsDbContext(dbContextOptions);
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
                await using var db = new BookingsDbContext(dbContextOptions);
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

            await using var db = new BookingsDbContext(dbContextOptions);
            
            var venue = options.HearingVenue ?? new RefDataBuilder().HearingVenues[0];
            var hearingVenue = await db.Venues.FirstOrDefaultAsync(x => x.Id == venue.Id);

            // create a hearing in the future to avoid validation failures and use reflection to update the scheduled date
            var scheduledDate = DateTime.Today.AddDays(1).AddHours(10).AddMinutes(30);
            var videoHearing = InitVideoHearing(scheduledDate, hearingVenue, caseType, options.Case, options.ScheduledDuration, options.AudioRecordingRequired);
            var videoHearingType = typeof(VideoHearing);
            videoHearingType.GetProperty("ScheduledDateTime")?.SetValue(videoHearing, pastScheduledDate);

            await AddParticipantsToVideoHearing(videoHearing);

            if (status == BookingStatus.Created)
            {
                videoHearing.UpdateStatus(BookingStatus.Created, "automated@test.com", null);
            }

            // then add judge if requested (need to use the db same context here to avoid conflicts)
            if (options.AddJudge)
            {
                await AddJudgeToVideoHearing(videoHearing, db);
            }

            // then add panel member if requested (need to use the db same context here to avoid conflicts)
            if (options.AddPanelMember)
            {
                await AddPanelMemberToVideoHearing(videoHearing, db);
            }

            videoHearing.IsFirstDayOfMultiDayHearing = isMultiDayFirstHearing;


            await db.VideoHearings.AddAsync(videoHearing);
            await db.SaveChangesAsync();


            var hearing = await new GetHearingByIdQueryHandler(new BookingsDbContext(dbContextOptions)).Handle(
                new GetHearingByIdQuery(videoHearing.Id));
            _seededHearings.Add(hearing.Id);
            return hearing;
        }

        public async Task<JudiciaryPersonStaging> AddJudiciaryPersonStaging()
        {
            await using var db = new BookingsDbContext(dbContextOptions);

            var judiciaryPersonStaging =
                new JudiciaryPersonStaging(
                    Guid.NewGuid().ToString(),
                    Guid.NewGuid().ToString(),
                    Faker.Name.Prefix(),
                    Faker.Name.FirstName(),
                    Faker.Name.LastName(),
                    Faker.Name.FullName(),
                    Faker.Name.Suffix(),
                    Faker.Internet.Email(),
                    Faker.Phone.PhoneNumber(),
                    "Yes",
                    "No");
            await db.JudiciaryPersonsStaging.AddAsync(judiciaryPersonStaging);

            await db.SaveChangesAsync();
            return judiciaryPersonStaging;
        }

        public async Task<JudiciaryPerson> AddJudiciaryPerson(string personalCode = null, bool isGeneric = false, string email = null)
        {
            await using var db = new BookingsDbContext(dbContextOptions);

            var judiciaryPerson = new JudiciaryPersonBuilder(personalCode).Build();
            judiciaryPerson.IsGeneric = isGeneric;
            if (!string.IsNullOrEmpty(email))
                judiciaryPerson.Email = email;
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

            var person = await db.JudiciaryPersons.FirstOrDefaultAsync(x => x.PersonalCode == personalCode);

            return person;
        }

        public async Task AddDeletedJudiciaryPerson(string personalCode, string deletedOn, string email = null)
        {
            await using var db = new BookingsDbContext(dbContextOptions);

            var judiciaryPerson = new JudiciaryPersonBuilder(personalCode).Build();
            judiciaryPerson.SetProtected(nameof(judiciaryPerson.Deleted), true);
            judiciaryPerson.SetProtected(nameof(judiciaryPerson.DeletedOn), deletedOn);
            if (!string.IsNullOrEmpty(email))
                judiciaryPerson.Email = email;
            await db.JudiciaryPersons.AddAsync(judiciaryPerson);

            await db.SaveChangesAsync();
            AddJudiciaryPersonsForCleanup(judiciaryPerson.PersonalCode);
        }
        
        public async Task AddLeaverJudiciaryPerson(string personalCode, string leftOn, string email = null)
        {
            await using var db = new BookingsDbContext(dbContextOptions);

            var judiciaryPerson = new JudiciaryPersonBuilder(personalCode).Build();
            judiciaryPerson.Leaver = true;
            judiciaryPerson.HasLeft = true;
            judiciaryPerson.LeftOn = leftOn;
            if (!string.IsNullOrEmpty(email))
                judiciaryPerson.Email = email;
            await db.JudiciaryPersons.AddAsync(judiciaryPerson);

            await db.SaveChangesAsync();
            AddJudiciaryPersonsForCleanup(judiciaryPerson.PersonalCode);
        }

        public async Task RemoveJudiciaryPersonAsync(JudiciaryPerson judiciaryPerson)
        {
            await using var db = new BookingsDbContext(dbContextOptions);

            db.JudiciaryPersons.Remove(judiciaryPerson);

            await db.SaveChangesAsync();
        }

        public async Task<Allocation> AddAllocation(VideoHearing hearing, JusticeUser user)
        {
            user ??= await SeedJusticeUser(userName: "testUser", null, null, isTeamLead: true);

            await using var db = new BookingsDbContext(dbContextOptions);
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
            await using var db = new BookingsDbContext(dbContextOptions);
            var seededForRemoval = await db.VideoHearings.Include(x => x.Allocations).Where(x =>
                x.Allocations.Any(allocation => _seededAllocationIds.Contains(allocation.Id))).ToListAsync();

            seededForRemoval.ForEach(vh => vh.Deallocate());
            await db.SaveChangesAsync();
        }

        public async Task<List<VideoHearing>> SeedMultiDayHearing(bool useV2, 
            IEnumerable<DateTime> dates, int scheduledDuration = 45, 
            bool addPanelMember = false)
        {
            await using var db = new BookingsDbContext(dbContextOptions);

            var orderedDates = dates.OrderBy(x => x.Date).ToList();

            // Create the first day
            var firstDayHearing = await SeedVideoHearing(configureOptions: options =>
            {
                options.AddJudge = true;
                options.AddPanelMember = addPanelMember;
                options.ScheduledDate = orderedDates[0];
                options.ScheduledDuration = scheduledDuration;
            }, status: BookingStatus.Created, isMultiDayFirstHearing: true);

            // Create the subsequent days
            var datesOfSubsequentDays = orderedDates.Skip(1).ToList();
            await CloneVideoHearing(firstDayHearing.Id, datesOfSubsequentDays, status: BookingStatus.Created, duration: scheduledDuration);

            var hearings = new List<VideoHearing>();
            var multiDayHearings = await db.VideoHearings.Where(x => x.SourceId == firstDayHearing.Id).ToListAsync();
            foreach (var multiDayHearing in multiDayHearings)
            {
                hearings.Add(await new GetHearingByIdQueryHandler(db).Handle(new GetHearingByIdQuery(multiDayHearing.Id)));
            }

            hearings = hearings.OrderBy(x => x.ScheduledDateTime).ToList();

            return hearings;
        }

        public async Task AddJudiciaryPanelMember(VideoHearing videoHearing, 
            JudiciaryPerson judiciaryPerson, string displayName)
        {
            await using var db = new BookingsDbContext(dbContextOptions);
            var dbHearing = await db.VideoHearings.Include(x => x.JudiciaryParticipants).ThenInclude(a => a.JudiciaryPerson)
                .FirstAsync(x => x.Id == videoHearing.Id);
            var person = await db.JudiciaryPersons.FirstAsync(p => p.PersonalCode == judiciaryPerson.PersonalCode);
            dbHearing.AddJudiciaryPanelMember(person, displayName);
            await db.SaveChangesAsync();
        }

        public async Task AddPanelMember(VideoHearing videoHearing, CaseType caseType)
        {
            await using var db = new BookingsDbContext(dbContextOptions);
            await AddPanelMemberToVideoHearing(videoHearing, db);
        }

        /// <summary>
        /// Seed a person with a unique email address. Not intended to be used to be added to hearings
        /// </summary>
        /// <returns></returns>
        public async Task<Person> SeedPerson(bool withOrganisation = false)
        {
            var builder = new PersonBuilder(true);

            if (withOrganisation)
            {
                builder.WithOrganisation();
            }

            var person = builder.Build();
            person.ContactEmail = $"{Guid.NewGuid():N}@auto.com";
            await using var db = new BookingsDbContext(dbContextOptions);
            await db.Persons.AddAsync(person);
            await db.SaveChangesAsync();
            _seededPersonIds.Add(person.Id);
            return person;
        }

        public async Task<JobHistory> SeedJobHistory(string jobName, bool successful = true)
        {
            await using var db = new BookingsDbContext(dbContextOptions);
            var jobHistory = new UpdateJobHistory(jobName, true);
            await db.JobHistory.AddAsync(jobHistory);
            await db.SaveChangesAsync();
            _seededJobHistoryIds.Add(jobHistory.Id);
            return jobHistory;
        }

        public async Task ClearSeededJobHistory()
        {
            await using var db = new BookingsDbContext(dbContextOptions);
            foreach (var id in _seededJobHistoryIds)
            {
                var jobHistory = await db.JobHistory.FindAsync(id);
                if (jobHistory != null)
                {
                    db.JobHistory.Remove(jobHistory);
                    await db.SaveChangesAsync();
                }
            }

            _seededJobHistoryIds.Clear();
        }

        public void AddJobHistoryToBeDeleted(Guid id)
        {
            _seededJobHistoryIds.Add(id);
        }
    }
}