using BookingsApi.DAL;
using BookingsApi.DAL.Queries;
using BookingsApi.Domain;
using BookingsApi.Domain.RefData;
using FizzWare.NBuilder;
using Microsoft.EntityFrameworkCore;

namespace BookingsApi.UnitTests.DAL.Queries
{
    public class GetHearingShellQueryTest
    {
        private BookingsDbContext _context;
        private VideoHearing _hearing;
        private GetHearingShellByIdQueryHandler _handler;
        private readonly string _caseTypeName = "Generic";
        private readonly string _hearingTypeName = "Automated Test";
        private readonly string _hearingVenueName = "Birmingham Civil and Family Justice Centre";

        [OneTimeSetUp]
        public void InitialSetup()
        {
            var contextOptions = new DbContextOptionsBuilder<BookingsDbContext>()
                .UseInMemoryDatabase("VhBookingsInMemory").Options;
            _context = new BookingsDbContext(contextOptions);
            var refDataBuilder = new RefDataBuilder();
            var venue = refDataBuilder.HearingVenues.First(x => x.Name == _hearingVenueName);
            var caseType = new CaseType(1, _caseTypeName);
            var hearingType = Builder<HearingType>.CreateNew().WithFactory(() => new HearingType(_hearingTypeName)).Build();
            var scheduledDateTime = DateTime.Today.AddDays(1).AddHours(11).AddMinutes(45);
            var duration = 80;
            var hearingRoomName = "Roome03";
            var otherInformation = "OtherInformation03";
            var createdBy = "User03";
            const bool audioRecordingRequired = true;
            var cancelReason = "Online abandonment (incomplete registration)";
            _hearing = new VideoHearing(caseType, scheduledDateTime, duration, venue, hearingRoomName,
                        otherInformation, createdBy, audioRecordingRequired, cancelReason);
            _hearing.SetHearingType(hearingType);
            _context.VideoHearings.Add(_hearing);
            _context.SaveChangesAsync();
        }

        [OneTimeTearDown]
        public void FinalCleanUp()
        {
            _context.Database.EnsureDeleted();
        }

        [Test]
        public async Task Should_create_handler()
        { 
            _handler = new GetHearingShellByIdQueryHandler(_context);
            var hearing = await _handler.Handle(new GetHearingShellByIdQuery(_hearing.Id));
            hearing.Should().NotBeNull();
        }
    }
}


