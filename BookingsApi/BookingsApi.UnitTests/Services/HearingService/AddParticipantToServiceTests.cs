using System.Collections.Generic;
using BookingsApi.DAL;
using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Helper;
using BookingsApi.DAL.Services;
using BookingsApi.Domain;
using BookingsApi.Domain.RefData;
using Microsoft.EntityFrameworkCore;

namespace BookingsApi.UnitTests.Services.HearingService
{
    public class AddParticipantToServiceTests
    {
        private IHearingService _hearingService;
        private DateTime _beforeUpdatedDate;
        private int _beforeParticipantCount;

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
            var individual = BuildParticipant(UserRoles.Individual);
            var newParticipants = new List<NewParticipant> { individual };
            CacheHearing(hearing);

            // Act
            await _hearingService.AddParticipantToService(hearing, newParticipants, createdBy: createdBy);

            // Assert
            AssertParticipantAdded(hearing, individual, createdBy);
        }

        [TestCase("UserName")]
        [TestCase("")]
        [TestCase(null)]
        public async Task Should_add_representative_to_service(string createdBy)
        {
            // Arrange
            var hearing = new VideoHearingBuilder().WithCase().Build();
            var representative = BuildParticipant(UserRoles.Representative);
            var newParticipants = new List<NewParticipant> { representative };
            CacheHearing(hearing);

            // Act
            await _hearingService.AddParticipantToService(hearing, newParticipants, createdBy: createdBy);

            // Assert
            AssertParticipantAdded(hearing, representative, createdBy);
        }

        [TestCase("UserName")]
        [TestCase("")]
        [TestCase(null)]
        public async Task Should_add_judicial_office_holder_to_service(string createdBy)
        {
            // Arrange
            var hearing = new VideoHearingBuilder().WithCase().Build();
            var joh = BuildParticipant(UserRoles.JudicialOfficeHolder);
            var newParticipants = new List<NewParticipant> { joh };
            CacheHearing(hearing);

            // Act
            await _hearingService.AddParticipantToService(hearing, newParticipants, createdBy: createdBy);

            // Assert
            AssertParticipantAdded(hearing, joh, createdBy);
        }

        [TestCase("UserName")]
        [TestCase("")]
        [TestCase(null)]
        public async Task Should_add_judge_to_service(string createdBy)
        {
            // Arrange
            var hearing = new VideoHearingBuilder(addJudge: false).WithCase().Build();
            var judge = BuildParticipant(UserRoles.Judge);
            var newParticipants = new List<NewParticipant> { judge };
            CacheHearing(hearing);

            // Act
            await _hearingService.AddParticipantToService(hearing, newParticipants, createdBy: createdBy);

            // Assert
            AssertParticipantAdded(hearing, judge, createdBy);
        }

        private static NewParticipant BuildParticipant(string userRole)
        {
            var participantHearingRole = new HearingRole(1, userRole)
            {
                UserRole = new UserRole(1, "UserRole") { Name = userRole }
            };

            return new NewParticipant
            {
                DisplayName = userRole,
                HearingRole = participantHearingRole,
                CaseRole = new CaseRole(1, "CaseRole"),
                Person = new PersonBuilder("email@email.com").Build(),
                Representee = userRole == UserRoles.Representative ? "Representee" : ""
            };
        }

        private void CacheHearing(Hearing hearing)
        {
            _beforeUpdatedDate = hearing.UpdatedDate;
            _beforeParticipantCount = hearing.Participants.Count;
        }

        private void AssertParticipantAdded(Hearing hearing, NewParticipant participant, string createdBy)
        {
            hearing.Participants.Count.Should().Be(_beforeParticipantCount + 1);
            hearing.Participants.Should().Contain(p => p.Person.ContactEmail == participant.Person.ContactEmail);
            
            hearing.UpdatedDate.Should().BeAfter(_beforeUpdatedDate);
            hearing.UpdatedBy.Should().Be(string.IsNullOrEmpty(createdBy) ? "System" : createdBy);
        }
    }
}
