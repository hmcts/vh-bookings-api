﻿using BookingsApi.DAL.Queries;
using BookingsApi.Domain.Enumerations;

namespace BookingsApi.IntegrationTests.Database.Queries
{
    public class GetAudioRecordedHearingsBySearchQueryHandlerTests : DatabaseTestsBase
    {
        private GetAudioRecordedHearingsBySearchQueryHandler _handler;

        [SetUp]
        public void Setup()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            _handler = new GetAudioRecordedHearingsBySearchQueryHandler(context);
        }

        [Test]
        public async Task Should_get_hearing_details_by_case_number()
        {
            var seededHearing1 = await Hooks.SeedVideoHearing(options => options.AudioRecordingRequired = true,
                status: BookingStatus.Created);

            TestContext.WriteLine($"New seeded video hearing id: { seededHearing1.Id }");

            var caseData = seededHearing1.HearingCases[0];
            TestContext.WriteLine($"New seeded video caseNumber: { caseData.Case.Number }");

            var hearing = await _handler.Handle(new GetAudioRecordedHearingsBySearchQuery(caseData.Case.Number));

            hearing.Should().NotBeNull();
            hearing.Any().Should().BeTrue();

            hearing[0].ScheduledDateTime.Should().Be(seededHearing1.ScheduledDateTime);
            hearing[0].HearingRoomName.Should().Be(seededHearing1.HearingRoomName);
            hearing[0].HearingVenue.Name.Should().Be(seededHearing1.HearingVenue.Name);
        }
        
        [Test]
        public async Task Should_get_hearing_details_by_partial_case_number()
        {
            var seededHearing1 = await Hooks.SeedVideoHearing(options => options.AudioRecordingRequired = true,
                status: BookingStatus.Created);

            TestContext.WriteLine($"New seeded video hearing id: { seededHearing1.Id }");

            var caseData = seededHearing1.HearingCases[0];
            TestContext.WriteLine($"New seeded video caseNumber: { caseData.Case.Number }");

            var caseNumber = caseData.Case.Number.Substring(0, 3);
            var hearings = await _handler.Handle(new GetAudioRecordedHearingsBySearchQuery(caseNumber));

            hearings.Should().NotBeNull();
            hearings.Any().Should().BeTrue();

            var hearing = hearings.Single(x => x.Id == seededHearing1.Id);
            hearing.ScheduledDateTime.Should().Be(seededHearing1.ScheduledDateTime);
            hearing.HearingRoomName.Should().Be(seededHearing1.HearingRoomName);
            hearing.HearingVenue.Name.Should().Be(seededHearing1.HearingVenue.Name);
        }
        
        [Test]
        public async Task Should_get_hearing_details_by_case_number_and_date()
        {
            var seededHearing1 = await Hooks.SeedVideoHearing(options => options.AudioRecordingRequired = true,
                status: BookingStatus.Created);

            TestContext.WriteLine($"New seeded video hearing id: { seededHearing1.Id }");

            var caseData = seededHearing1.HearingCases[0];
            TestContext.WriteLine($"New seeded video caseNumber: { caseData.Case.Number }");

            var caseNumber = caseData.Case.Number.Substring(0, 3);
            var hearings = await _handler.Handle(new GetAudioRecordedHearingsBySearchQuery(caseNumber, seededHearing1.ScheduledDateTime));

            hearings.Should().NotBeNull();
            hearings.Any().Should().BeTrue();

            var hearing = hearings.Single(x => x.Id == seededHearing1.Id);
            hearing.ScheduledDateTime.Should().Be(seededHearing1.ScheduledDateTime);
            hearing.HearingRoomName.Should().Be(seededHearing1.HearingRoomName);
            hearing.HearingVenue.Name.Should().Be(seededHearing1.HearingVenue.Name);
        }
        
        [Test]
        public async Task Should_get_hearing_details_by_date()
        {
            var seededHearing1 = await Hooks.SeedVideoHearing(options => options.AudioRecordingRequired = true,
                status: BookingStatus.Created);

            TestContext.WriteLine($"New seeded video hearing id: { seededHearing1.Id }");

            var caseData = seededHearing1.HearingCases[0];
            TestContext.WriteLine($"New seeded video caseNumber: { caseData.Case.Number }");
            var hearings = await _handler.Handle(new GetAudioRecordedHearingsBySearchQuery(null, seededHearing1.ScheduledDateTime));

            hearings.Should().NotBeNull();
            hearings.Any().Should().BeTrue();

            var hearing = hearings.Single(x => x.Id == seededHearing1.Id);
            hearing.ScheduledDateTime.Should().Be(seededHearing1.ScheduledDateTime);
            hearing.HearingRoomName.Should().Be(seededHearing1.HearingRoomName);
            hearing.HearingVenue.Name.Should().Be(seededHearing1.HearingVenue.Name);
        }
        
        [Test]
        public async Task Should_not_get_hearing_details_by_case_number_and_wrong_date()
        {
            var seededHearing1 = await Hooks.SeedVideoHearing(options => options.AudioRecordingRequired = true,
                status: BookingStatus.Created);

            TestContext.WriteLine($"New seeded video hearing id: { seededHearing1.Id }");

            var caseData = seededHearing1.HearingCases[0];
            TestContext.WriteLine($"New seeded video caseNumber: { caseData.Case.Number }");

            var caseNumber = caseData.Case.Number.Substring(0, 3);
            var date = seededHearing1.ScheduledDateTime.AddDays(1);
            var hearings = await _handler.Handle(new GetAudioRecordedHearingsBySearchQuery(caseNumber, date));

            hearings.Should().NotBeNull();
            hearings.Exists(x => x.Id == seededHearing1.Id).Should().BeFalse();
        }
    }
}