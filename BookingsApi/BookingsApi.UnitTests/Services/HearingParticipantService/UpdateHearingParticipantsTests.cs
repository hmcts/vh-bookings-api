using System.Collections.Generic;
using BookingsApi.Contract.V1.Enums;
using BookingsApi.Contract.V1.Requests;
using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Commands.Core;
using BookingsApi.DAL.Queries;
using BookingsApi.DAL.Queries.Core;
using BookingsApi.Domain;
using BookingsApi.Domain.Participants;
using BookingsApi.Domain.RefData;
using BookingsApi.Infrastructure.Services.AsynchronousProcesses;
using BookingsApi.Infrastructure.Services.IntegrationEvents;
using BookingsApi.Infrastructure.Services.IntegrationEvents.Events;

namespace BookingsApi.UnitTests.Services.HearingParticipantService
{
    public class UpdateHearingParticipantsTests
    {
        private UpdateHearingParticipantsRequest _request;
        private List<UpdateParticipantRequest> _existingParticipants;
        private List<ParticipantRequest> _newParticipants;
        private List<Guid> _removedParticipantIds;
        private List<LinkedParticipantRequest> _linkedParticipants;
        private BookingsApi.Services.HearingParticipantService _hearingParticipantService;
        private Mock<IQueryHandler> _queryHandler;
        private Mock<ICommandHandler> _commandHandler;
        private Mock<IEventPublisher> _eventPublisher;
        private Mock<IParticipantAddedToHearingAsynchronousProcess> _participantAddedToHearingAsynchronousProcess;
        private Mock<INewJudiciaryAddedAsynchronousProcesses> _newJudiciaryAddedAsynchronousProcess;
        private VideoHearing _videoHearing;
        private List<Participant> _participants;
        
        private List<Participant> Participants
        {
            get
            {
                if (_participants != null) return _participants;

                _participants = new ParticipantBuilder().Build();

                foreach(var participant in _participants)
                {
                    participant.DisplayName = "Test Participant";
                    participant.CaseRole = new CaseRole(1, "TestCaseRole");

                    if(participant.HearingRole.UserRole.Name == "Representative")
                    {
                        var representative = (Representative)participant;
                        representative.Representee = "Representee";
                    }
                }

                return _participants;
            }
        }

        [SetUp]
        public void SetUp()
        {
            var existingParticipant = GetVideoHearing().Participants[0];

            _existingParticipants = new List<UpdateParticipantRequest>
            {
                new()
                {
                    DisplayName = "DisplayName",
                    OrganisationName = "OrganisationName",
                    ParticipantId = existingParticipant.Id,
                    Representee = "Representee",
                    TelephoneNumber = "07123456789",
                    Title = "Title"
                }
            };

            _newParticipants = new List<ParticipantRequest>
            {
                new()
                {
                    CaseRoleName = "Generic",
                    ContactEmail = "contactme@dontcontactme.com",
                    DisplayName = "DisplayName",
                    FirstName = "FirstName",
                    HearingRoleName = "Litigant in person",
                    LastName = "LastName",
                    MiddleNames = "MiddleNames",
                    OrganisationName = "OrganisationName",
                    Representee = "Representee",
                    TelephoneNumber = "07123456789",
                    Title = "Title",
                    Username = "contactme@dontcontactme.com",
                }
            };

            _removedParticipantIds = new List<Guid> { Guid.NewGuid() };

            _linkedParticipants = new List<LinkedParticipantRequest>
            {
                new()
                {
                    LinkedParticipantContactEmail = "participant@notLinked.com",
                    ParticipantContactEmail = "participant@linked.com",
                    Type = LinkedParticipantType.Interpreter
                }
            };

            _queryHandler = new Mock<IQueryHandler>();
            _commandHandler = new Mock<ICommandHandler>();
            _eventPublisher = new Mock<IEventPublisher>();
            _participantAddedToHearingAsynchronousProcess = new Mock<IParticipantAddedToHearingAsynchronousProcess>();
            _newJudiciaryAddedAsynchronousProcess = new Mock<INewJudiciaryAddedAsynchronousProcesses>();
            _videoHearing = GetVideoHearing();

            _queryHandler.Setup(q => q.Handle<GetParticipantsInHearingQuery, List<Participant>>(It.IsAny<GetParticipantsInHearingQuery>()))
                .ReturnsAsync(Participants);
            _queryHandler.Setup(q => q.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>())).ReturnsAsync(_videoHearing);
            _queryHandler.Setup(q => q.Handle<GetCaseRolesForCaseTypeQuery, CaseType>(It.IsAny<GetCaseRolesForCaseTypeQuery>())).ReturnsAsync(CaseType);
            
            _hearingParticipantService = new BookingsApi.Services.HearingParticipantService(_commandHandler.Object,
                _eventPublisher.Object, _participantAddedToHearingAsynchronousProcess.Object, _newJudiciaryAddedAsynchronousProcess.Object,
                _queryHandler.Object);
        }
        
        [Test]
        public async Task Should_call_update_hearing_participants_command()
        {
            //Arrange
            var hearing = GetVideoHearing();
            hearing.Participants[0].Person.ContactEmail = "contactme@dontcontactme.com";
            hearing.Participants[1].Person.ContactEmail = "participant@notLinked.com";
            hearing.Participants[2].Person.ContactEmail = "participant@linked.com";
            hearing.UpdateStatus(BookingsApi.Domain.Enumerations.BookingStatus.Created, "test", "");
            _queryHandler.Setup(q => q.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>())).ReturnsAsync(hearing);

            _existingParticipants = new List<UpdateParticipantRequest>
            {
                new()
                {
                    DisplayName = "DisplayName",
                    OrganisationName = "OrganisationName",
                    ParticipantId = hearing.Participants[0].Id,
                    Representee = "Representee",
                    TelephoneNumber = "07123456789",
                    Title = "Title"
                }
            };
            _request = BuildRequest();

            //Act
            await _hearingParticipantService.UpdateParticipants(_request, hearing);

            //Assert
            _commandHandler.Verify(ch => ch.Handle(It.Is<UpdateHearingParticipantsCommand>(x =>
                x.HearingId == hearing.Id
                
                && x.ExistingParticipants[0].DisplayName == _request.ExistingParticipants[0].DisplayName
                && x.ExistingParticipants[0].OrganisationName == _request.ExistingParticipants[0].OrganisationName
                && x.ExistingParticipants[0].ParticipantId == _request.ExistingParticipants[0].ParticipantId
                && x.ExistingParticipants[0].RepresentativeInformation.Representee == _request.ExistingParticipants[0].Representee
                && x.ExistingParticipants[0].TelephoneNumber == _request.ExistingParticipants[0].TelephoneNumber
                && x.ExistingParticipants[0].Title == _request.ExistingParticipants[0].Title

                && x.NewParticipants[0].CaseRole.Name == _request.NewParticipants[0].CaseRoleName
                && x.NewParticipants[0].DisplayName == _request.NewParticipants[0].DisplayName
                && x.NewParticipants[0].HearingRole.Name == _request.NewParticipants[0].HearingRoleName
                && x.NewParticipants[0].Representee == _request.NewParticipants[0].Representee
                && x.NewParticipants[0].Person.ContactEmail == _request.NewParticipants[0].ContactEmail
                && x.NewParticipants[0].Person.FirstName == _request.NewParticipants[0].FirstName
                && x.NewParticipants[0].Person.LastName == _request.NewParticipants[0].LastName
                && x.NewParticipants[0].Person.MiddleNames == _request.NewParticipants[0].MiddleNames
                && x.NewParticipants[0].Person.TelephoneNumber == _request.NewParticipants[0].TelephoneNumber
                && x.NewParticipants[0].Person.Title == _request.NewParticipants[0].Title

                && x.RemovedParticipantIds[0] == _request.RemovedParticipantIds[0]

                && x.LinkedParticipants[0].LinkedParticipantContactEmail == _request.LinkedParticipants[0].LinkedParticipantContactEmail
                && x.LinkedParticipants[0].ParticipantContactEmail == _request.LinkedParticipants[0].ParticipantContactEmail
            )), Times.Once);

            _eventPublisher.Verify(x => x.PublishAsync(It.IsAny<HearingParticipantsUpdatedIntegrationEvent>()), Times.Once);
        }

        [Test]
        public async Task Should_publish_CreateAndNotifyUser_and_HearingNotification_integration_events_when_no_judge()
        {
            //Arrange
            var hearing = GetVideoHearing();
            var part1 = hearing.Participants.First(x => x.HearingRole.Name == "Name");
            part1.Person.ContactEmail = "contactme@dontcontactme.com";
            _queryHandler.Setup(q => q.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>())).ReturnsAsync(hearing);

            _existingParticipants = new List<UpdateParticipantRequest>
            {
                new()
                {
                    DisplayName = "DisplayName",
                    OrganisationName = "OrganisationName",
                    ParticipantId = hearing.Participants[0].Id,
                    Representee = "Representee",
                    TelephoneNumber = "07123456789",
                    Title = "Title"
                }
            };
            _request = BuildRequest();
            
            //Act
            await _hearingParticipantService.UpdateParticipants(_request, hearing);

            //Assert
            _eventPublisher.Verify(x => x.PublishAsync(It.IsAny<HearingParticipantsUpdatedIntegrationEvent>()), Times.Once);
        }

        [TestCase("Email@email.com", "email@email.com")]
        [TestCase("email@email.com", "Email@email.com")]
        public async Task Should_publish_events_for_updated_participants_ignoring_contact_email_case(
            string personEmail, string newParticipantEmail)
        {
            // ie if the contact email for the new participant is the same as the existing person record but the case is different,
            // we should still publish a new participant event
            
            // Arrange
            var hearing = GetVideoHearing();
            var person = hearing.Participants.First(x => x.HearingRole.Name == "Name");
            person.Person.ContactEmail = personEmail;
            _queryHandler.Setup(q => q.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>())).ReturnsAsync(hearing);
            
            _existingParticipants = new List<UpdateParticipantRequest>
            {
                new()
                {
                    DisplayName = "DisplayName",
                    OrganisationName = "OrganisationName",
                    ParticipantId = hearing.Participants[0].Id,
                    Representee = "Representee",
                    TelephoneNumber = "07123456789",
                    Title = "Title"
                }
            };
            _newParticipants[0].ContactEmail = newParticipantEmail;
            _request = BuildRequest();
            
            // Act
            await _hearingParticipantService.UpdateParticipants(_request, hearing);

            // Assert
            _eventPublisher.Verify(x => x.PublishAsync(It.IsAny<HearingParticipantsUpdatedIntegrationEvent>()), Times.Once);
        }

        [Test]
        public async Task Should_publish_HearingIsReadyForVideo_integration_events_when_judge_added()
        {
            //Arrange
            var hearing = GetVideoHearing();
            var part1 = hearing.Participants.First(x => x.HearingRole.Name == "Judge");
            part1.HearingRole = new HearingRole(102, "Judge") { UserRole = new UserRole(1, "Judge") };
            part1.Person.ContactEmail = "contactme@dontcontactme.com";
            _queryHandler.Setup(q => q.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>())).ReturnsAsync(hearing);

            _existingParticipants = new List<UpdateParticipantRequest>
            {
                new()
                {
                    DisplayName = "DisplayName",
                    OrganisationName = "OrganisationName",
                    ParticipantId = hearing.Participants[0].Id,
                    Representee = "Representee",
                    TelephoneNumber = "07123456789",
                    Title = "Title"
                }
            };
            _request = BuildRequest();

            //Act
            await _hearingParticipantService.UpdateParticipants(_request, hearing);

            //Assert
            _eventPublisher.Verify(x => x.PublishAsync(It.IsAny<HearingParticipantsUpdatedIntegrationEvent>()), Times.Once);
        }

        [Test]
        public async Task Should_publish_HearingParticipantsUpdated_integration_events_when_hearing_status_is_created()
        {
            //Arrange
            var hearing = GetVideoHearing();
            hearing.Participants[0].Person.ContactEmail = "contactme@dontcontactme.com";
            hearing.Participants[1].Person.ContactEmail = "participant@notLinked.com";
            hearing.Participants[2].Person.ContactEmail = "participant@linked.com";

            hearing.UpdateStatus(BookingsApi.Domain.Enumerations.BookingStatus.Created, "test", "");
            _queryHandler.Setup(q => q.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>())).ReturnsAsync(hearing);

            _existingParticipants = new List<UpdateParticipantRequest>
            {
                new()
                {
                    DisplayName = "DisplayName",
                    OrganisationName = "OrganisationName",
                    ParticipantId = hearing.Participants[0].Id,
                    Representee = "Representee",
                    TelephoneNumber = "07123456789",
                    Title = "Title"
                }
            };
            _request = BuildRequest();

            //Act
            await _hearingParticipantService.UpdateParticipants(_request, hearing);

            //Assert
            _eventPublisher.Verify(x => x.PublishAsync(It.IsAny<HearingParticipantsUpdatedIntegrationEvent>()), Times.Once);
        }


        [Test]
        public async Task Should_add_given_participants_to_hearing_with_created_status_and_publishevent_if_several_matching_participant_with_contactemail()
        {
            var hearing = GetVideoHearing(true);
            hearing.Participants[0].HearingRole = new HearingRole(1, "Name") { UserRole = new UserRole(1, "User"), };
            
            _queryHandler.Setup(q => q.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>())).ReturnsAsync(hearing);
            _request = BuildRequest(withLinkedParticipants: false);
            _request.NewParticipants[0].ContactEmail = hearing.Participants[0].Person.ContactEmail;

            var response = await _hearingParticipantService.UpdateParticipants(_request, hearing);

            response.Should().NotBeNull();
            _eventPublisher.Verify(x => x.PublishAsync(It.IsAny<HearingParticipantsUpdatedIntegrationEvent>()), Times.Once);
        }

        [Test]
        public async Task Should_add_given_participants_to_hearing_with_judge_and_publish_event_if_several_matching_participant_with_contact_email()
        {
            var hearing = GetVideoHearing();
            var judge = hearing.Participants.First(e => e is Judge);  
            judge.HearingRole = new HearingRole(1, "Generic") { UserRole = new UserRole(1, "Judge")};
            _queryHandler.Setup(q => q.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>())).ReturnsAsync(hearing);
            _request = BuildRequest(withLinkedParticipants: false);
            _request.NewParticipants[0].ContactEmail = judge.Person.ContactEmail;

            var response = await _hearingParticipantService.UpdateParticipants(_request, hearing);

            response.Should().NotBeNull();
            _eventPublisher.Verify(x => x.PublishAsync(It.IsAny<HearingParticipantsUpdatedIntegrationEvent>()), Times.Once);
        }

        [Test]
        public async Task Should_add_given_participants_to_hearing_without_judge_and_publish_event_if_several_matching_participant_with_contactemail()
        {
            var hearing = GetVideoHearing();
            hearing.Participants[0].HearingRole = new HearingRole(1, "Name") { UserRole = new UserRole(1, "User"), };
            _queryHandler.Setup(q => q.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>())).ReturnsAsync(hearing);
            _request = BuildRequest(withLinkedParticipants: false);
            _request.NewParticipants[0].ContactEmail = hearing.Participants[0].Person.ContactEmail;

            var response = await _hearingParticipantService.UpdateParticipants(_request, hearing);

            response.Should().NotBeNull();
            _eventPublisher.Verify(x => x.PublishAsync(It.IsAny<HearingParticipantsUpdatedIntegrationEvent>()), Times.Once);
        }

        [Test]
        public async Task Should_publish_HearingParticipantsUpdated_integration_events_when_participant_is_removed()
        {
            //Arrange
            var hearing = GetVideoHearing();
            hearing.Participants[0].Person.ContactEmail = "contactme@dontcontactme.com";
            hearing.Participants[1].Person.ContactEmail = "participantToDelete@notLinked.com";

            hearing.UpdateStatus(BookingsApi.Domain.Enumerations.BookingStatus.Created, "test", "");
            _queryHandler.Setup(q => q.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>())).ReturnsAsync(hearing);

            _existingParticipants = new List<UpdateParticipantRequest>
            {
                new()
                {
                    DisplayName = "DisplayName",
                    OrganisationName = "OrganisationName",
                    ParticipantId = hearing.Participants[0].Id,
                    Representee = "Representee",
                    TelephoneNumber = "07123456789",
                    Title = "Title"
                }
            };
            _request = BuildRequest();
            _request.LinkedParticipants = new List<LinkedParticipantRequest>();
            _request.NewParticipants = new List<ParticipantRequest>();
            _request.RemovedParticipantIds = new List<Guid> { hearing.Participants[1].Id };

            //Act
            await _hearingParticipantService.UpdateParticipants(_request, hearing);

            //Assert
            _eventPublisher.Verify(x => x.PublishAsync(It.IsAny<HearingParticipantsUpdatedIntegrationEvent>()), Times.Once);
        }

        [Test]
        public async Task Should_publish_Participant_and_Judge_UpdatedIntegrationEvent_on_each_participant_when_no_new_participants_added()
        {
            //Arrange
            var hearing = GetVideoHearing();
            hearing.UpdateStatus(BookingsApi.Domain.Enumerations.BookingStatus.Created, "test", "");
            _queryHandler.Setup(q => q.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>())).ReturnsAsync(hearing);

            _existingParticipants = new List<UpdateParticipantRequest>
            {
                new()
                {
                    DisplayName = "DisplayName",
                    OrganisationName = "OrganisationName",
                    ParticipantId = hearing.Participants[0].Id,
                    Representee = "Representee",
                    TelephoneNumber = "07123456789",
                    Title = "Title"
                },     
                new()
                {
                    DisplayName = "DisplayName",
                    OrganisationName = "OrganisationName",
                    ParticipantId = hearing.Participants.First(e=>e is Judge).Id, 
                    Representee = "Representee",
                    TelephoneNumber = "07123456789",
                    ContactEmail = "new@email.com",
                    Title = "Title"
                }
            };
            _request = BuildRequest();
            _request.LinkedParticipants = new List<LinkedParticipantRequest>();
            _request.NewParticipants = new List<ParticipantRequest>();
            _request.RemovedParticipantIds = new List<Guid>();

            //Act
            await _hearingParticipantService.UpdateParticipants(_request, hearing);

            //Assert
            _eventPublisher.Verify(x => x.PublishAsync(It.IsAny<ParticipantUpdatedIntegrationEvent>()), Times.Once);
            _eventPublisher.Verify(x => x.PublishAsync(It.IsAny<JudgeUpdatedIntegrationEvent>()), Times.Once);
        }
        
        private UpdateHearingParticipantsRequest BuildRequest(bool withLinkedParticipants = true)
        {
            return new UpdateHearingParticipantsRequest
            {
                ExistingParticipants = _existingParticipants,
                NewParticipants = _newParticipants,
                RemovedParticipantIds = _removedParticipantIds,
                LinkedParticipants = withLinkedParticipants ? _linkedParticipants : new List<LinkedParticipantRequest>()
            };
        }
        
        private static VideoHearing GetVideoHearing(bool createdStatus = false)
        { 
            var hearing = new VideoHearingBuilder().Build();
            hearing.AddCase("123", "Case name", true);
            hearing.CaseType = CaseType;
            foreach (var participant in hearing.Participants)
            {
                participant.HearingRole = participant is Judge 
                    ? new HearingRole(1, "Judge") { UserRole = new UserRole(1, "Judge")}
                    : new HearingRole(1, "Name") { UserRole = new UserRole(1, "User")};
                participant.CaseRole = new CaseRole(1, "Generic");
            }

            if(createdStatus)
                hearing.UpdateStatus(BookingsApi.Domain.Enumerations.BookingStatus.Created, "administrator", string.Empty);

            return hearing; 
        }
        
        private static CaseType CaseType => new(1, "Civil") { CaseRoles = new List<CaseRole> {
            CreateCaseAndHearingRoles(1, "Generic", "representative", new List<string> { "Litigant in person" }),
            CreateCaseAndHearingRoles(2, "Test", "Judge", new List<string> { "Judge" })
        } };
        
        private static CaseRole CreateCaseAndHearingRoles(int caseId, string caseRoleName,string userRole, List<string> roles)
        {
            var hearingRoles = new List<HearingRole>();

            foreach (var role in roles)
            {
                hearingRoles.Add(new HearingRole(1, role) { UserRole = new UserRole(1, userRole) });
            }

            var caseRole = new CaseRole(caseId, caseRoleName) { HearingRoles = hearingRoles };

            return caseRole;
        }
    }
}
