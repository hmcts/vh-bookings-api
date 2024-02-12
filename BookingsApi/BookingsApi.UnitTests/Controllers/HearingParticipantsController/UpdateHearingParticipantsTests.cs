using BookingsApi.DAL.Queries;
using BookingsApi.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using BookingsApi.Contract.V1.Enums;
using BookingsApi.Contract.V1.Requests;
using Testing.Common.Assertions;

namespace BookingsApi.UnitTests.Controllers.HearingParticipantsController
{
    public class UpdateHearingParticipantsTests : HearingParticipantsControllerTest
    {
        private UpdateHearingParticipantsRequest _request;
        private List<UpdateParticipantRequest> _existingParticipants { get; set; }
        private List<ParticipantRequest> _newParticipants { get; set; }
        private List<Guid> _removedParticipantIds { get; set; }
        private List<LinkedParticipantRequest> _linkedParticipants { get; set; }

        [SetUp]
        public void SetUp()
        {
            var existingParticipant = GetVideoHearing().Participants[0];

            _existingParticipants = new List<UpdateParticipantRequest>
            {
                new UpdateParticipantRequest
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
                new ParticipantRequest
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
                new LinkedParticipantRequest
                {
                    LinkedParticipantContactEmail = "participant@notLinked.com",
                    ParticipantContactEmail = "participant@linked.com",
                    Type = LinkedParticipantType.Interpreter
                }
            };
        }

        [Test]
        public async Task Should_return_bad_request_when_hearing_id_is_empty()
        {
            //Arrange
            _request = BuildRequest();

            //Act
            var response = await Controller.UpdateHearingParticipants(Guid.Empty, _request);

            //Assert
            response.Should().NotBeNull();
            var objectResult = (ObjectResult)response;
            ((ValidationProblemDetails)objectResult.Value).ContainsKeyAndErrorMessage(nameof(hearingId), $"Please provide a valid {nameof(hearingId)}");
        }

        [Test]
        public async Task Should_return_bad_request_when_request_validation_fails()
        {
            //Arrange

            //Act
            var response = await Controller.UpdateHearingParticipants(hearingId, new UpdateHearingParticipantsRequest());

            //Assert
            response.Should().NotBeNull();
            var objectResult = (ObjectResult)response;
            ((ValidationProblemDetails)objectResult.Value).Should().NotBeNull();
        }

        [Test]
        public async Task Should_return_bad_request_for_given_invalid_representative_info()
        {
            //Arrange
            _request = BuildRequest();
            _request.NewParticipants[0].HearingRoleName = "Litigant in person";
            _request.NewParticipants[0].Representee = string.Empty;

            //Act
            var response = await Controller.UpdateHearingParticipants(hearingId, _request);

            response.Should().NotBeNull();
            var objectResult = (ObjectResult)response;
            ((ValidationProblemDetails)objectResult.Value).ContainsKeyAndErrorMessage("NewParticipants[0].Representee", "Representee is required");
        }

        [Test]
        public async Task Should_return_not_found_when_hearing_cannot_be_found()
        {
            //Arrange
            QueryHandler
             .Setup(x => x.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>()))
             .ReturnsAsync((VideoHearing)null);
            _request = BuildRequest();

            //Act
            var response = await Controller.UpdateHearingParticipants(hearingId, _request) as NotFoundResult;

            //Assert
            response.Should().BeOfType<NotFoundResult>();
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
    }
}
