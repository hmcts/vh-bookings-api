using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.DAL;
using BookingsApi.DAL.Services;
using BookingsApi.Domain;
using BookingsApi.Domain.RefData;
using BookingsApi.UnitTests.Constants;
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Testing.Common.Builders.Domain;

namespace BookingsApi.UnitTests.DAL.Services
{
    public class HearingAllocationServiceTests
    {
        private BookingsDbContext _context;
        private HearingAllocationService _service;
        private VideoHearing _hearing;
        private readonly IList<JusticeUser> _seededJusticeUsers = new List<JusticeUser>();

        [OneTimeSetUp]
        public void InitialSetup()
        {
            var contextOptions = new DbContextOptionsBuilder<BookingsDbContext>()
                .UseInMemoryDatabase("VhBookingsInMemory").Options;
            _context = new BookingsDbContext(contextOptions);
            _service = new HearingAllocationService(_context);
            SeedData();
        }
        
        [OneTimeTearDown]
        public void FinalCleanUp()
        {
            _context.Database.EnsureDeleted();
        }

        [Test]
        public void Should_fail_when_hearing_does_not_exist()
        {
            // Arrange
            var hearingId = Guid.NewGuid();

            // Act
            var action = async() => await _service.AllocateCso(hearingId);

            // Assert
            action.Should().Throw<ArgumentException>().And.Message.Should().Be($"Hearing {hearingId} not found");
        }
        
        [Test]
        public async Task Should_fail_when_no_csos_available_due_to_work_hours_not_coinciding()
        {
            // Arrange
            var hearingId = _hearing.Id;
            SetHearingTime(_hearing, 15, 0, 0);

            foreach (var justiceUser in _seededJusticeUsers)
            {
                for (var i = 1; i <= 7; i++)
                {
                    // TODO clear these down between tests
                    justiceUser.VhoWorkHours.Add(new VhoWorkHours
                    {
                        DayOfWeekId = i, 
                        StartTime = new TimeSpan(8, 0, 0), 
                        EndTime = new TimeSpan(10, 0, 0)
                    });
                }
            }

            await _context.SaveChangesAsync();
            
            // Act
            var action = async() => await _service.AllocateCso(hearingId);
            
            // Assert
            action.Should().Throw<InvalidOperationException>().And.Message.Should().Be($"Unable to allocate to hearing {hearingId}, no CSOs available");
        }
        
        // [Test]
        // public async Task Should_fail_when_no_csos_available_due_to_non_availability_hours_coinciding()
        // {
        //     // Arrange
        //     var hearingId = _hearing.Id;
        //
        //     // Act
        //     var action = async() => await _service.AllocateCso(hearingId);
        //     
        //     // Assert
        //     action.Should().Throw<InvalidOperationException>().And.Message.Should().Be($"Unable to allocate, no CSOs available");
        // }

        //
        // [Test]
        // public async Task Should_allocate_successfully_when_one_cso_available()
        // {
        //     
        // }
        //
        // [Test]
        // public async Task Should_allocate_successfully_when_multiple_csos_available_and_one_with_no_allocations()
        // {
        //     
        // }
        //
        // [Test]
        // public async Task Should_allocate_successfully_when_multiple_csos_available_and_zero_with_no_allocations_and_one_with_one_allocation()
        // {
        //     
        // }
        //
        // [Test]
        // public async Task Should_allocate_successfully_when_multiple_csos_available_and_zero_with_no_allocations_and_zero_with_one_allocation_and_one_with_two_allocations()
        // {
        //     
        // }
        //
        // [Test]
        // public async Task Should_return_error_when_multiple_csos_available_and_zero_with_no_allocations_and_zero_with_one_allocation_and_zero_with_two_allocations()
        // {
        //     
        // }
        //
        // [Test]
        // public async Task Should_allocate_randomly_when_multiple_csos_available_and_more_than_one_with_no_allocations()
        // {
        //     
        // }
        //
        // [Test]
        // public async Task Should_allocate_randomly_when_multiple_csos_available_and_zero_with_no_allocations_and_more_than_one_with_one_allocation()
        // {
        //     
        // }
        //
        // [Test]
        // public async Task Should_allocate_randomly_when_multiple_csos_available_and_zero_with_no_allocations_and_zero_with_one_allocation_and_more_than_one_with_two_allocations()
        // {
        //     
        // }
        
        private void SetHearingTime(VideoHearing hearing, int hour, int minutes, int seconds)
        {
            var newDatetime = new DateTime(hearing.ScheduledDateTime.Year, hearing.ScheduledDateTime.Month, hearing.ScheduledDateTime.Day, hour, minutes, seconds, DateTimeKind.Utc);
            
            hearing.UpdateHearingDetails(
                hearing.HearingVenue, 
                newDatetime, 
                hearing.ScheduledDuration, 
                hearing.HearingRoomName, 
                hearing.OtherInformation, 
                hearing.UpdatedBy, 
                hearing.GetCases().ToList(), 
                hearing.QuestionnaireNotRequired, 
                hearing.AudioRecordingRequired);

            _context.SaveChanges();
        }
        
        private void SeedData()
        {
            SeedHearing();
            SeedJusticeUsers();
        }

        private void SeedHearing()
        {
            var caseTypeName = "Generic";
            var hearingTypeName = "Automated Test";
            var hearingVenueName = "Birmingham Civil and Family Justice Centre";
            
            var refDataBuilder = new RefDataBuilder();
            var venue = refDataBuilder.HearingVenues.First( x=> x.Name == hearingVenueName);
            var caseType = new CaseType(1, caseTypeName);
            var hearingType = Builder<HearingType>.CreateNew().WithFactory(() => new HearingType(hearingTypeName)).Build();
            var scheduledDateTime = DateTime.Today.AddDays(1).AddHours(11).AddMinutes(45);
            var duration = 80;
            var hearingRoomName = "Roome03";
            var otherInformation = "OtherInformation03";
            var createdBy = "User03";
            const bool questionnaireNotRequired = false;
            const bool audioRecordingRequired = true;
            var cancelReason = "Online abandonment (incomplete registration)";

            var videoHearing = Builder<VideoHearing>.CreateNew().WithFactory(() =>
                    new VideoHearing(caseType, hearingType, scheduledDateTime, duration, venue, hearingRoomName,
                        otherInformation, createdBy, questionnaireNotRequired, audioRecordingRequired, cancelReason))
                .Build();

            // Set the navigation properties as well since these would've been set if we got the hearing from DB
            videoHearing.SetProtected(nameof(videoHearing.HearingType), hearingType);
            videoHearing.SetProtected(nameof(videoHearing.CaseType), caseType);
            videoHearing.SetProtected(nameof(videoHearing.HearingVenue), venue);
            
            _context.VideoHearings.Add(videoHearing);
            _context.SaveChanges();

            _hearing = videoHearing;
        }

        private void SeedJusticeUsers()
        {
            SeedJusticeUser("user1@email.com", "User", "1");
            SeedJusticeUser("user2@email.com", "User", "2");
            SeedJusticeUser("user3@email.com", "User", "3");
        }

        private void SeedJusticeUser(string userName, string firstName, string lastName, bool isTeamLead = false)
        {
            var justiceUser = new JusticeUser
            {
                ContactEmail = userName,
                Username = userName,
                UserRoleId = isTeamLead ? (int)UserRoleId.vhTeamLead : (int)UserRoleId.vho,
                CreatedBy = "test@test.com",
                CreatedDate = DateTime.Now,
                FirstName = firstName,
                Lastname = lastName
            };

            var workHours = new List<VhoWorkHours>();
            justiceUser.SetProtected(nameof(justiceUser.VhoWorkHours), workHours);

            _context.JusticeUsers.Add(justiceUser);
            _context.SaveChanges();

            _seededJusticeUsers.Add(justiceUser);
        }
    }
}
