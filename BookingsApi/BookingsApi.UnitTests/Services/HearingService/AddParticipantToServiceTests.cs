using System.Collections.Generic;
using BookingsApi.DAL;
using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Services;
using BookingsApi.Domain.RefData;
using Microsoft.EntityFrameworkCore;

namespace BookingsApi.UnitTests.Services.HearingService
{
    public class AddParticipantToServiceTests
    {
        private IHearingService _hearingService;

        [SetUp]
        public void SetUp()
        {
            var contextOptions = new DbContextOptionsBuilder<BookingsDbContext>()
                .UseInMemoryDatabase("VhBookingsInMemory").Options;
            var context = new BookingsDbContext(contextOptions);
            _hearingService = new BookingsApi.DAL.Services.HearingService(context);
        }
        
        [TestCase("UserName")]
        [TestCase("")]
        [TestCase(null)]
        public async Task Should_add_individual_to_service(string createdBy)
        {
            // Arrange
            var hearing = new VideoHearingBuilder().WithCase().Build();
            var hearingRole = new HearingRole(1, "Individual")
            {
                UserRole = new UserRole(1, "UserRole") { Name = "Individual" }
            };
            var individual = new NewParticipant
            {
                DisplayName = "Individual",
                Representee = "",
                HearingRole = hearingRole,
                CaseRole = new CaseRole(1, "CaseRole"),
                Person = new PersonBuilder("email@email.com").Build()
            };
            var newParticipants = new List<NewParticipant> { individual };
            var beforeUpdatedDate = hearing.UpdatedDate;

            // Act
            await _hearingService.AddParticipantToService(hearing, newParticipants, createdBy: createdBy);

            // Assert
            AssertHearingUpdatedAuditDetailsAreUpdated(hearing, beforeUpdatedDate, createdBy);
        }

        [TestCase("UserName")]
        [TestCase("")]
        [TestCase(null)]
        public async Task Should_add_representative_to_service(string createdBy)
        {
            var hearing = new VideoHearingBuilder().WithCase().Build();
            var hearingRole = new HearingRole(1, "Representative")
            {
                UserRole = new UserRole(1, "UserRole") { Name = "Representative" }
            };
            var representative = new NewParticipant
            {
                DisplayName = "Representative",
                Representee = "Representee",
                HearingRole = hearingRole,
                CaseRole = new CaseRole(1, "CaseRole"),
                Person = new PersonBuilder("email@email.com").Build()
            };
            var newParticipants = new List<NewParticipant> { representative };
            var beforeUpdatedDate = hearing.UpdatedDate;

            // Act
            await _hearingService.AddParticipantToService(hearing, newParticipants, createdBy: createdBy);

            // Assert
            AssertHearingUpdatedAuditDetailsAreUpdated(hearing, beforeUpdatedDate, createdBy);
        }

        [Test]
        public async Task Should_add_judicial_office_holder_to_service()
        {
            // TODO
            Assert.Fail();
        }

        [Test]
        public async Task Should_add_judge_to_service()
        {
            // TODO
            Assert.Fail();
        }
        
        private static void AssertHearingUpdatedAuditDetailsAreUpdated(BookingsApi.Domain.Hearing hearing, 
            DateTime beforeUpdatedDate, 
            string updatedBy)
        {
            hearing.UpdatedDate.Should().BeAfter(beforeUpdatedDate);
            hearing.UpdatedBy.Should().Be(string.IsNullOrEmpty(updatedBy) ? "System" : updatedBy);
        }
    }
}
