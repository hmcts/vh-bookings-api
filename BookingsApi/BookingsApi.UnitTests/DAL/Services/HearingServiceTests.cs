using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using BookingsApi.Common.Exceptions;
using BookingsApi.Common.Services;
using BookingsApi.Contract.Configuration;
using BookingsApi.DAL;
using BookingsApi.DAL.Services;
using BookingsApi.Domain;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.RefData;
using BookingsApi.Services;
using Castle.Core.Internal;
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Testing.Common.Builders.Domain;

namespace BookingsApi.UnitTests.DAL.Services
{
    public class HearingServiceTests
    {
        private AutoMock _mocker;
        private HearingService _service;
        protected DbContextOptions<BookingsDbContext> BookingsDbContextOptions;
        private string _databaseConnectionString;
        private BookingsDbContext _context;
        private VideoHearing _hearing;
        
        [OneTimeSetUp]
        public void InitialSetup()
        {
            var contextOptions = new DbContextOptionsBuilder<BookingsDbContext>()
                .UseInMemoryDatabase("VhBookingsInMemory").Options;
            _context = new BookingsDbContext(contextOptions);
        }
        
        [OneTimeTearDown]
        public void FinalCleanUp()
        {
            _context.Database.EnsureDeleted();
        }

        [SetUp]
        public void SetUp()
        {
            this.SeedHearing();
            
            _service = new HearingService(_context);
        }

        [Test]
        public async Task Get_Unallocated_Hearings()
        {
            var response = await _service.GetUnallocatedHearings();


            response.Should().NotBeNull();
            response.Count.Should().Be(1);
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
        
    }
}
