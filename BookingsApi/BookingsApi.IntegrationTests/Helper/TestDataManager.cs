using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.Common.Services;
using BookingsApi.DAL;
using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Exceptions;
using BookingsApi.DAL.Helper;
using BookingsApi.DAL.Queries;
using BookingsApi.DAL.Services;
using BookingsApi.Domain;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.Participants;
using BookingsApi.Domain.RefData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using Testing.Common.Builders.Domain;

namespace BookingsApi.IntegrationTests.Helper
{
    public class TestDataManager
    {
        private readonly DbContextOptions<BookingsDbContext> _dbContextOptions;
        private readonly List<Guid> _seededHearings = new List<Guid>();
        public List<Guid> JudiciaryPersons { get; } = new List<Guid>();
        public string CaseNumber { get; } = "2222/3511";
        private Guid _individualId;
        private List<Guid> _participantRepresentativeIds;
        private readonly string _defaultCaseName;

        public void AddHearingForCleanup(Guid id)
        {
            _seededHearings.Add(id);
        }
        
        public void AddJudiciaryPersonsForCleanup(params Guid[] ids)
        {
            JudiciaryPersons.AddRange(ids);
        }
        
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
            bool addSuitabilityAnswer = false, BookingStatus status = BookingStatus.Booked, int endPointsToAdd = 0, bool addJoh = false, bool withLinkedParticipants = false)
        {
            var options = new SeedVideoHearingOptions();
            configureOptions?.Invoke(options);
            var caseType = GetCaseTypeFromDb(options.CaseTypeName);

            var applicantCaseRole = caseType.CaseRoles.First(x => x.Name == options.ApplicantRole);
            var respondentCaseRole = caseType.CaseRoles.First(x => x.Name == options.RespondentRole);
            var judgeCaseRole = caseType.CaseRoles.First(x => x.Name == "Judge");
            
            var applicantLipHearingRole = applicantCaseRole.HearingRoles.First(x => x.Name == options.LipHearingRole);
            var applicantRepresentativeHearingRole = applicantCaseRole.HearingRoles.First(x => x.Name == "Representative");
            var respondentRepresentativeHearingRole = respondentCaseRole.HearingRoles.First(x => x.Name == "Representative");
            var respondentLipHearingRole = respondentCaseRole.HearingRoles.First(x => x.Name == options.LipHearingRole);
            var judgeHearingRole = judgeCaseRole.HearingRoles.First(x => x.Name == "Judge");

            var hearingType = caseType.HearingTypes.First(x => x.Name == options.HearingTypeName);

            var venue = options.HearingVenue;
            if (venue == null)
            {
                var venues = new RefDataBuilder().HearingVenues;
                venue = venues.First();
            }

            var person1 = new PersonBuilder(true).WithOrganisation().Build();
            var person2 = new PersonBuilder(true).Build();
            var person3 = new PersonBuilder(true).Build();
            var person4 = new PersonBuilder(true).Build();
            var judgePerson = new PersonBuilder(true).Build();
            var johPerson = new PersonBuilder(true).Build();
            var scheduledDate = options.ScheduledDate ?? DateTime.Today.AddDays(1).AddHours(10).AddMinutes(30);
            const int duration = 45;
            const string hearingRoomName = "Room02";
            const string otherInformation = "OtherInformation02";
            const string createdBy = "test@hmcts.net";
            const bool questionnaireNotRequired = false;
            const bool audioRecordingRequired = true;
            var cancelReason = "Online abandonment (incomplete registration)";

            var videoHearing = new VideoHearing(caseType, hearingType, scheduledDate, duration,
                venue, hearingRoomName, otherInformation, createdBy, questionnaireNotRequired,
                audioRecordingRequired, cancelReason);

            videoHearing.AddIndividual(person1, applicantLipHearingRole, applicantCaseRole,
                $"{person1.FirstName} {person1.LastName}");
            
            videoHearing.AddRepresentative(person2, applicantRepresentativeHearingRole, applicantCaseRole,
                $"{person2.FirstName} {person2.LastName}", "Ms X");

            videoHearing.AddRepresentative(person3, respondentRepresentativeHearingRole, respondentCaseRole,
                $"{person3.FirstName} {person3.LastName}", "Ms Y");
            
            videoHearing.AddIndividual(person4, respondentLipHearingRole, respondentCaseRole,
                $"{person4.FirstName} {person4.LastName}");

            videoHearing.AddJudge(judgePerson, judgeHearingRole, judgeCaseRole, $"{judgePerson.FirstName} {judgePerson.LastName}");

            if (addJoh)
            {
                var johCaseRole = caseType.CaseRoles.First(x => x.Name == "Judicial Office Holder");
                var johHearingRole = johCaseRole.HearingRoles.First(x => x.Name == "Judicial Office Holder");
                videoHearing.AddJudicialOfficeHolder(johPerson, johHearingRole, johCaseRole,
                    $"{johPerson.FirstName} {johPerson.LastName}");
            }
            
            if (endPointsToAdd > 0)
            {
                var r = new RandomGenerator();
                for (int i = 0; i < endPointsToAdd; i++)
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

            videoHearing.AddCase($"{Faker.RandomNumber.Next(1000, 9999)}/{Faker.RandomNumber.Next(1000, 9999)}",
                $"{_defaultCaseName} {Faker.RandomNumber.Next(900000, 999999)}", true);
            videoHearing.AddCase($"{Faker.RandomNumber.Next(1000, 9999)}/{Faker.RandomNumber.Next(1000, 9999)}",
                $"{_defaultCaseName} {Faker.RandomNumber.Next(900000, 999999)}", false);
            videoHearing.AddCase(CaseNumber, $"{_defaultCaseName} {Faker.RandomNumber.Next(900000, 999999)}", false);

            var dA = videoHearing.Participants[1];
            videoHearing.AddEndpoints(
                new List<Endpoint> {
                    new Endpoint("new endpoint", Guid.NewGuid().ToString(), "pin", null),
                    new Endpoint("new endpoint", Guid.NewGuid().ToString(), "pin", dA),
                });

            if (status == BookingStatus.Created)
            {
                videoHearing.UpdateStatus(BookingStatus.Created, createdBy, null);
            }

            await using (var db = new BookingsDbContext(_dbContextOptions))
            {
                if(!db.Cases.Any(r => r.Number == CaseNumber)) 
                {
                    db.Cases.Add(new Case (CaseNumber, $"{_defaultCaseName} {Faker.RandomNumber.Next(900000, 999999)}"));
                }

                await db.VideoHearings.AddAsync(videoHearing);
                await db.SaveChangesAsync();
            }

            var hearing = await new GetHearingByIdQueryHandler(new BookingsDbContext(_dbContextOptions)).Handle(
                new GetHearingByIdQuery(videoHearing.Id));
            _individualId = hearing.Participants.First(x => x.HearingRole.UserRole.IsIndividual).Id;
            _participantRepresentativeIds = hearing.Participants
                .Where(x => x.HearingRole.UserRole.IsRepresentative).Select(x => x.Id).ToList();

            if (addSuitabilityAnswer)
            {
                await AddQuestionnaire();
            }

            hearing = await new GetHearingByIdQueryHandler(new BookingsDbContext(_dbContextOptions)).Handle(
                new GetHearingByIdQuery(videoHearing.Id));
            _seededHearings.Add(hearing.Id);
            return hearing;
        }

        public async Task<VideoHearing> SeedVideoHearingWithNoJudge(Action<SeedVideoHearingOptions> configureOptions = null)
        {
            var options = new SeedVideoHearingOptions();
            configureOptions?.Invoke(options);
            var caseType = GetCaseTypeFromDb(options.CaseTypeName);

            var applicantCaseRole = caseType.CaseRoles.First(x => x.Name == options.ApplicantRole);
            var respondentCaseRole = caseType.CaseRoles.First(x => x.Name == options.RespondentRole);
            var applicantLipHearingRole = applicantCaseRole.HearingRoles.First(x => x.Name == options.LipHearingRole);
            var applicantRepresentativeHearingRole = applicantCaseRole.HearingRoles.First(x => x.Name == "Representative");
            var respondentRepresentativeHearingRole = respondentCaseRole.HearingRoles.First(x => x.Name == "Representative");
            var respondentLipHearingRole = respondentCaseRole.HearingRoles.First(x => x.Name == options.LipHearingRole);
            var hearingType = caseType.HearingTypes.First(x => x.Name == options.HearingTypeName);

            var venue = options.HearingVenue ?? new RefDataBuilder().HearingVenues.First();

            var person1 = new PersonBuilder(true).WithOrganisation().Build();
            var person2 = new PersonBuilder(true).Build();
            var person3 = new PersonBuilder(true).Build();
            var person4 = new PersonBuilder(true).Build();

            var scheduledDate = options.ScheduledDate ?? DateTime.Today.AddDays(1).AddHours(10).AddMinutes(30);

            const int duration = 45;
            const string hearingRoomName = "Room02";
            const string otherInformation = "OtherInformation02";
            const string createdBy = "test@hmcts.net";
            const bool questionnaireNotRequired = false;
            const bool audioRecordingRequired = true;
            const string cancelReason = "Online abandonment (incomplete registration)";

            var videoHearing = new VideoHearing(caseType, hearingType, scheduledDate, duration,
                venue, hearingRoomName, otherInformation, createdBy, questionnaireNotRequired,
                audioRecordingRequired, cancelReason);

            videoHearing.AddIndividual(person1, applicantLipHearingRole, applicantCaseRole,
                $"{person1.FirstName} {person1.LastName}");

            videoHearing.AddRepresentative(person2, applicantRepresentativeHearingRole, applicantCaseRole,
                $"{person2.FirstName} {person2.LastName}", "Ms X");

            videoHearing.AddRepresentative(person3, respondentRepresentativeHearingRole, respondentCaseRole,
                $"{person3.FirstName} {person3.LastName}", "Ms Y");

            videoHearing.AddIndividual(person4, respondentLipHearingRole, respondentCaseRole,
                $"{person4.FirstName} {person4.LastName}");

            videoHearing.AddCase($"{Faker.RandomNumber.Next(1000, 9999)}/{Faker.RandomNumber.Next(1000, 9999)}",
                $"{_defaultCaseName} {Faker.RandomNumber.Next(900000, 999999)}", true);
            videoHearing.AddCase($"{Faker.RandomNumber.Next(1000, 9999)}/{Faker.RandomNumber.Next(1000, 9999)}",
                $"{_defaultCaseName} {Faker.RandomNumber.Next(900000, 999999)}", false);
            videoHearing.AddCase(CaseNumber, $"{_defaultCaseName} {Faker.RandomNumber.Next(900000, 999999)}", false);

            return await SaveVideoHearing(videoHearing);
        }

        public async Task<VideoHearing> SaveVideoHearing(VideoHearing videoHearing)
        {
            await using var db = new BookingsDbContext(_dbContextOptions);
            await db.VideoHearings.AddAsync(videoHearing);
            await db.SaveChangesAsync();

            _seededHearings.Add(videoHearing.Id);

            return videoHearing;
        }

        public async Task CloneVideoHearing(Guid hearingId, IList<DateTime> datesOfHearing)
        {
            var dbContext = new BookingsDbContext(_dbContextOptions);
            var hearing = await new GetHearingByIdQueryHandler(dbContext)
                .Handle(new GetHearingByIdQuery(hearingId));
            
            var orderedDates = datesOfHearing.OrderBy(x => x).ToList();
            var totalDays = orderedDates.Count + 1;
            var commands = orderedDates.Select((newDate, index) =>
            {
                var config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();
                var hearingDay = index + 2; 
                return CloneHearingToCommandMapper.CloneToCommand(hearing, newDate, new RandomGenerator(),
                    config.GetValue<string>("KinlyConfiguration:SipAddressStem"), totalDays, hearingDay);
            }).ToList();
            
            foreach (var command in commands)
            {
                await new CreateVideoHearingCommandHandler(dbContext, new HearingService(dbContext)).Handle(command);
            }
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
                .Include(x => x.Questionnaire).First(x => x.Id == _individualId);
            participant.Questionnaire = new Questionnaire { Participant = participant, ParticipantId = _individualId };
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
                    .Include(x => x.Questionnaire).First(x => x.Id == item);
                participantRepresentative.Questionnaire = new Questionnaire
                { Participant = participantRepresentative, ParticipantId = item };

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
        
        private void CreateParticipantLinks(Participant interpretee, Participant interpreter)
        {
            interpretee.LinkedParticipants.Add(new LinkedParticipant(interpretee.Id,interpreter.Id,LinkedParticipantType.Interpreter));
            interpreter.LinkedParticipants.Add(new LinkedParticipant(interpreter.Id, interpretee.Id, LinkedParticipantType.Interpreter));
        }

        public DateTime? GetJobLastRunDateTime()
        {
            DateTime? lastUpdateDateTime;
            using (var db = new BookingsDbContext(_dbContextOptions))
            {
                lastUpdateDateTime = db.JobHistory.FirstOrDefault()?.LastRunDate;
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
            foreach (var externalRefId in JudiciaryPersons)
            {
                try
                {
                    await using var db = new BookingsDbContext(_dbContextOptions);
                    var jp = await db.JudiciaryPersons.SingleOrDefaultAsync(x => x.ExternalRefId == externalRefId);
                    if (jp != null)
                    {
                        db.JudiciaryPersons.Remove(jp);
                        await db.SaveChangesAsync();    
                    }
                    TestContext.WriteLine(@$"Remove Judiciary Person: {externalRefId}.");
                }
                catch (JudiciaryPersonNotFoundException)
                {
                    TestContext.WriteLine(@$"Ignoring cleanup for Judiciary Person: {externalRefId}. Does not exist.");
                }
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

        public string[] GetIndividualHearingRoles => new[] { "Litigant in person" };

        public Task<VideoHearing> SeedPastHearings(DateTime scheduledDate)
        {
            return SeedPastVideoHearing(scheduledDate, null, false, BookingStatus.Booked);
        }

        public async Task<VideoHearing> SeedPastVideoHearing(DateTime pastScheduledDate, Action<SeedVideoHearingOptions> configureOptions,
            bool addSuitabilityAnswer = false, BookingStatus status = BookingStatus.Booked)
        {
            var options = new SeedVideoHearingOptions();
            configureOptions?.Invoke(options);
            var caseType = GetCaseTypeFromDb(options.CaseTypeName);

            var applicantCaseRole = caseType.CaseRoles.First(x => x.Name == options.ApplicantRole);
            var respondentCaseRole = caseType.CaseRoles.First(x => x.Name == options.RespondentRole);
            var judgeCaseRole = caseType.CaseRoles.First(x => x.Name == "Judge");

            var applicantLipHearingRole = applicantCaseRole.HearingRoles.First(x => x.Name == options.LipHearingRole);
            var applicantRepresentativeHearingRole = applicantCaseRole.HearingRoles.First(x => x.Name == "Representative");
            var respondentRepresentativeHearingRole = respondentCaseRole.HearingRoles.First(x => x.Name == "Representative");
            var judgeHearingRole = judgeCaseRole.HearingRoles.First(x => x.Name == "Judge");

            var johCaseRole = caseType.CaseRoles.First(x => x.Name == "Panel Member");
            var johHearingRole = johCaseRole.HearingRoles.First(x => x.Name == "Panel Member");

            var hearingType = caseType.HearingTypes.First(x => x.Name == options.HearingTypeName);

            var venues = new RefDataBuilder().HearingVenues;

            var person1 = new PersonBuilder(true).WithOrganisation().Build();
            var person2 = new PersonBuilder(true).Build();
            var person3 = new PersonBuilder(true).Build();
            var person4 = new PersonBuilder(true).Build();
            var person5 = new PersonBuilder(true).Build();

            var scheduledDate = DateTime.Today.AddDays(1).AddHours(10).AddMinutes(30);
            const int duration = 45;
            const string hearingRoomName = "Room02";
            const string otherInformation = "OtherInformation02";
            const string createdBy = "test@hmcts.net";
            const bool questionnaireNotRequired = false;
            const bool audioRecordingRequired = true;
            var cancelReason = "Online abandonment (incomplete registration)";

            var videoHearing = new VideoHearing(caseType, hearingType, scheduledDate, duration,
                venues.First(), hearingRoomName, otherInformation, createdBy, questionnaireNotRequired,
                audioRecordingRequired, cancelReason);

            videoHearing.AddIndividual(person1, applicantLipHearingRole, applicantCaseRole,
                $"{person1.FirstName} {person1.LastName}");

            videoHearing.AddRepresentative(person2, applicantRepresentativeHearingRole, applicantCaseRole,
                $"{person2.FirstName} {person2.LastName}", "Ms X");

            videoHearing.AddRepresentative(person3, respondentRepresentativeHearingRole, respondentCaseRole,
                $"{person3.FirstName} {person3.LastName}", "Ms Y");

            videoHearing.AddJudge(person4, judgeHearingRole, judgeCaseRole, $"{person4.FirstName} {person4.LastName}");

            videoHearing.AddJudicialOfficeHolder(person5, johHearingRole, johCaseRole,
                $"{person5.FirstName} {person5.LastName}");

            videoHearing.AddCase($"{Faker.RandomNumber.Next(1000, 9999)}/{Faker.RandomNumber.Next(1000, 9999)}",
                $"{_defaultCaseName} {Faker.RandomNumber.Next(900000, 999999)}", true);
            videoHearing.AddCase($"{Faker.RandomNumber.Next(1000, 9999)}/{Faker.RandomNumber.Next(1000, 9999)}",
                $"{_defaultCaseName} {Faker.RandomNumber.Next(900000, 999999)}", false);

            if (status == BookingStatus.Created)
            {
                videoHearing.UpdateStatus(BookingStatus.Created, createdBy, null);
            }

            var videohearingType = typeof(VideoHearing);
            videohearingType.GetProperty("ScheduledDateTime").SetValue(videoHearing, pastScheduledDate);

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

        public async Task<VideoHearing> SeedVideoHearingLinkedParticipants(Action<SeedVideoHearingOptions> configureOptions)
        {
            var options = new SeedVideoHearingOptions();
            configureOptions?.Invoke(options);
            var caseType = GetCaseTypeFromDb(options.CaseTypeName);

            var applicantCaseRole = caseType.CaseRoles.First(x => x.Name == options.ApplicantRole);
            var respondentCaseRole = caseType.CaseRoles.First(x => x.Name == options.RespondentRole);
            var judgeCaseRole = caseType.CaseRoles.First(x => x.Name == "Judge");

            var applicantLipHearingRole = applicantCaseRole.HearingRoles.First(x => x.Name == options.LipHearingRole);
            var respondentLipHearingRole = respondentCaseRole.HearingRoles.First(x => x.Name == options.LipHearingRole);
            var judgeHearingRole = judgeCaseRole.HearingRoles.First(x => x.Name == "Judge");

            var hearingType = caseType.HearingTypes.First(x => x.Name == options.HearingTypeName);

            var venues = new RefDataBuilder().HearingVenues;

            var person1 = new PersonBuilder(true).Build();
            var person2 = new PersonBuilder(true).Build();
            var judgePerson = new PersonBuilder(true).Build();

            var scheduledDate = DateTime.Today.AddDays(1).AddHours(10).AddMinutes(30);
            const int duration = 45;
            const string hearingRoomName = "Room02";
            const string otherInformation = "OtherInformation02";
            const string createdBy = "test@hmcts.net";
            const bool questionnaireNotRequired = false;
            const bool audioRecordingRequired = true;
            var cancelReason = "Online abandonment (incomplete registration)";

            var videoHearing = new VideoHearing(caseType, hearingType, scheduledDate, duration,
                venues.First(), hearingRoomName, otherInformation, createdBy, questionnaireNotRequired,
                audioRecordingRequired, cancelReason);

            videoHearing.AddIndividual(person1, applicantLipHearingRole, applicantCaseRole,
                $"{person1.FirstName} {person1.LastName}");

            videoHearing.AddIndividual(person2, respondentLipHearingRole, respondentCaseRole,
                $"{person2.FirstName} {person2.LastName}");

            videoHearing.AddJudge(judgePerson, judgeHearingRole, judgeCaseRole, $"{judgePerson.FirstName} {judgePerson.LastName}");

            var interpretee = videoHearing.Participants[0];
            var interpreter = videoHearing.Participants[1];
            CreateParticipantLinks(interpretee, interpreter);

            await using (var db = new BookingsDbContext(_dbContextOptions))
            {
                await db.VideoHearings.AddAsync(videoHearing);
                await db.SaveChangesAsync();
            }

            var hearing = await new GetHearingByIdQueryHandler(new BookingsDbContext(_dbContextOptions)).Handle(
                new GetHearingByIdQuery(videoHearing.Id));
            _individualId = hearing.Participants.First(x => x.HearingRole.UserRole.IsIndividual).Id;
            _participantRepresentativeIds = hearing.Participants
                .Where(x => x.HearingRole.UserRole.IsRepresentative).Select(x => x.Id).ToList();

            hearing = await new GetHearingByIdQueryHandler(new BookingsDbContext(_dbContextOptions)).Handle(
                new GetHearingByIdQuery(videoHearing.Id));
            _seededHearings.Add(hearing.Id);
            return hearing;
        }

        public async Task AddJudiciaryPersonStaging()
        {
            await using var db = new BookingsDbContext(_dbContextOptions);

            var judiciaryPersonStaging = new JudiciaryPersonStaging(Faker.Name.First(), Faker.Name.First(), Faker.Name.First(), Faker.Name.First(),Faker.Name.First(),Faker.Name.First(),Faker.Name.First(),Faker.Name.First(),Faker.Name.First(),Faker.Name.First());;
            await db.JudiciaryPersonsStaging.AddAsync(judiciaryPersonStaging);

            await db.SaveChangesAsync();
        }
        
        public async Task AddJudiciaryPerson(Guid? externalRefId = null)
        {
            await using var db = new BookingsDbContext(_dbContextOptions);

            var judiciaryPerson = new JudiciaryPersonBuilder(externalRefId).Build();
            await db.JudiciaryPersons.AddAsync(judiciaryPerson);

            await db.SaveChangesAsync();
            AddJudiciaryPersonsForCleanup(judiciaryPerson.ExternalRefId);
        }
        
        public async Task RemoveJudiciaryPersonAsync(JudiciaryPerson judiciaryPerson)
        {
            await using var db = new BookingsDbContext(_dbContextOptions);

            db.JudiciaryPersons.Remove(judiciaryPerson);

            await db.SaveChangesAsync();
        }
    }
}