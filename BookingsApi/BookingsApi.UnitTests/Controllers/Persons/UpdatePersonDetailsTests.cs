using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.Contract.Requests;
using BookingsApi.Domain;
using BookingsApi.Domain.Enumerations;
using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Exceptions;
using BookingsApi.DAL.Queries;
using BookingsApi.Infrastructure.Services.IntegrationEvents.Events;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Testing.Common.Builders.Domain;

namespace BookingsApi.UnitTests.Controllers.Persons
{
    public class UpdatePersonDetailsTests : PersonsControllerTest
    {
        [Test]
        public async Task should_return_bad_request_if_payload_is_invalid()
        {
            var personId = Guid.NewGuid();
            var payload = new UpdatePersonDetailsRequest()
            {
                Username = String.Empty,
                FirstName = "New",
                LastName = "Me"
            };
            var result = await Controller.UpdatePersonDetails(personId, payload);
            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Test]
        public async Task should_return_not_found_when_command_throws_person_not_found_exception()
        {
            var personId = Guid.NewGuid();
            var payload = new UpdatePersonDetailsRequest()
            {
                Username = "new.me@hmcts.net",
                FirstName = "New",
                LastName = "Me"
            };
            CommandHandlerMock.Setup(x => x.Handle(It.IsAny<UpdatePersonCommand>()))
                .ThrowsAsync(new PersonNotFoundException(personId));
            var result = await Controller.UpdatePersonDetails(personId, payload);
            result.Result.Should().BeOfType<NotFoundObjectResult>();
            var typed = result.Result.As<NotFoundObjectResult>();
            typed.Value.Should().Be($"{personId} does not exist");
        }

        [Test]
        public async Task should_send_update_event_for_each_hearing_person_is_a_participant_in()
        {
            var personId = Guid.NewGuid();
            var payload = new UpdatePersonDetailsRequest()
            {
                Username = "new.me@hmcts.net",
                FirstName = "New",
                LastName = "Me",
            };

            var hearings = CreateListOfVideoHearings(personId, payload.FirstName, payload.LastName, payload.Username);
            QueryHandlerMock
                .Setup(x =>
                    x.Handle<GetHearingsByUsernameQuery, List<VideoHearing>>(It.IsAny<GetHearingsByUsernameQuery>()))
                .ReturnsAsync(hearings);
            
            var result = await Controller.UpdatePersonDetails(personId, payload);

            result.Result.Should().BeOfType<AcceptedResult>();

            EventPublisherMock.Verify(x => x.PublishAsync(It.IsAny<ParticipantUpdatedIntegrationEvent>()), Times.Once);

        }

        private List<VideoHearing> CreateListOfVideoHearings(Guid personId, string firstName, string lastName, string username)
        {
            var hearing1 = new VideoHearingBuilder().WithCase().Build();
            var hearing2 = new VideoHearingBuilder().WithCase().Build();
            var hearing3 = new VideoHearingBuilder().WithCase().Build();
            var hearing4 = new VideoHearingBuilder().WithCase().Build();
            var hearing5 = new VideoHearingBuilder().WithCase().Build();

            var person1 = hearing1.GetPersons().First();
            var participant1 = hearing1.GetParticipants().First(x => x.Person == person1);
            person1.UpdatePerson(firstName, lastName, username);
            person1.SetProtected(nameof(person1.Id), personId);
            participant1.SetProtected(nameof(participant1.PersonId), personId);
            
            var person2 = hearing2.GetPersons().First();
            var participant2 = hearing2.GetParticipants().First(x => x.Person == person2);
            person2.UpdatePerson(firstName, lastName, username);
            person2.SetProtected(nameof(person2.Id), personId);
            participant2.SetProtected(nameof(participant2.PersonId), personId);
            
            var person3 = hearing3.GetPersons().First();
            var participant3 = hearing3.GetParticipants().First(x => x.Person == person3);
            person3.UpdatePerson(firstName, lastName,  username);
            person3.SetProtected(nameof(person3.Id), personId);
            participant3.SetProtected(nameof(participant3.PersonId), personId);
            
            var person4 = hearing4.GetPersons().First();
            var participant4 = hearing4.GetParticipants().First(x => x.Person == person4);
            person4.UpdatePerson(firstName, lastName, username);
            person4.SetProtected(nameof(person4.Id), personId);
            participant4.SetProtected(nameof(participant4.PersonId), personId);
            
            var person5 = hearing5.GetPersons().First();
            var participant5 = hearing5.GetParticipants().First(x => x.Person == person5);
            person5.UpdatePerson(firstName, lastName, username);
            person5.SetProtected(nameof(person5.Id), personId);
            participant5.SetProtected(nameof(participant5.PersonId), personId);
            participant5.DisplayName = "dfidshfiudsf@hmcts.net";

            hearing1.UpdateStatus(BookingStatus.Failed, "test", null);
            hearing2.UpdateStatus(BookingStatus.Created, "test", null);
            hearing3.UpdateStatus(BookingStatus.Failed, "test", null);
            hearing4.UpdateStatus(BookingStatus.Cancelled, "test", "test cancellation");
            hearing5.UpdateStatus(BookingStatus.Created, "test", null);
            hearing5.AddCase("FUHSTD","IUHFISUHF@hmcts.net", false);
            
            return new List<VideoHearing>
            {
                hearing1, hearing2, hearing3, hearing4, hearing5
            };
        }
    }
}