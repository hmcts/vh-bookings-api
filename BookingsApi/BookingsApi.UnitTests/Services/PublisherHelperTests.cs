using BookingsApi.Domain.Participants;
using BookingsApi.Infrastructure.Services.Publishers;
using BookingsApi.Common;

namespace BookingsApi.UnitTests.Services
{
    /// <summary>
    /// When a booking is created with the builder class, the created date of the persons and participants set to the time when the hearing is created.
    /// So as persons are new and do not exist, the created datetimes are manipulated in the below tests to meet each of the test criteria.
    /// </summary>
    public class PublisherHelperTests
    {
        [Test]
        public void Should_return_new_participants_when_new_persons_added_as_participants()
        {
            var hearing = new VideoHearingBuilder().WithCase().Build();
            var videoHearingUpdateDate = hearing.UpdatedDate.TrimSeconds();
            
            PublisherHelper.GetNewParticipantsSinceLastUpdate(hearing, videoHearingUpdateDate).Count().Should().Be(hearing.Participants.Count(x => x is not Judge));
        }

        [Test]
        public void Should_return_ONLY_new_participants_when_new_persons_added_as_participants_WITH_one_existing_person()
        {
            var hearing = new VideoHearingBuilder().WithCase().Build();
            hearing.Participants.Where(x => x is not Judge).ToList()[0].ChangePerson(new PersonBuilder(treatPersonAsNew: false).Build());
            var videoHearingUpdateDate = hearing.UpdatedDate.TrimSeconds();
            
            PublisherHelper.GetNewParticipantsSinceLastUpdate(hearing, videoHearingUpdateDate).Count().Should().Be(hearing.Participants.Count(x => x is not Judge) - 1);
        }

        [Test]
        public void Should_return_new_participants_when_new_persons_added_as_participants_WITH_one_existing_person_AND_with_username_not_set()
        {
            var hearing = new VideoHearingBuilder().WithCase().Build();
            var participants = hearing.Participants.Where(x => x is not Judge).ToList();
            participants[0].ChangePerson(new PersonBuilder(treatPersonAsNew: false).Build());
            participants[1].ChangePerson(new PersonBuilder(treatPersonAsNew: true).Build());
            var videoHearingUpdateDate = hearing.UpdatedDate.TrimSeconds();
            
            PublisherHelper.GetNewParticipantsSinceLastUpdate(hearing, videoHearingUpdateDate).Count().Should().Be(hearing.Participants.Count(x => x is not Judge) - 1);
        }

        [Test]
        public void Should_return_ONLY_new_participants_when_new_persons_added_as_participants_TO_existing_hearing()
        {
            var hearing = new VideoHearingBuilder().WithCase().Build();
            var dateWhenHearingCreated = hearing.CreatedDate.AddDays(-10);
            hearing.GetType().GetProperty("CreatedDate")?.SetValue(hearing, dateWhenHearingCreated, null);

            var participants = hearing.Participants.Where(x => x is not Judge).ToList();
            participants.Skip(1).ToList().ForEach(x => x.Person.GetType().GetProperty("CreatedDate")?.SetValue(x.Person, dateWhenHearingCreated, null));
            participants.Skip(1).ToList().ForEach(x => x.GetType().GetProperty("CreatedDate")?.SetValue(x, dateWhenHearingCreated.AddDays(1), null));
            var videoHearingUpdateDate = hearing.UpdatedDate.TrimSeconds();
            
            PublisherHelper.GetNewParticipantsSinceLastUpdate(hearing, videoHearingUpdateDate).Count().Should().Be(1);
        }

        [Test]
        public void Should_not_return_judge_as_new_participant()
        {
            var hearing = new VideoHearingBuilder().WithCase().Build();
            var videoHearingUpdateDate = hearing.UpdatedDate.TrimSeconds();
            PublisherHelper.GetNewParticipantsSinceLastUpdate(hearing, videoHearingUpdateDate).Should().NotContain(x => x is Judge);
        }
        
        [Test]
        public void Should_return_All_existing_participants_including_Judge()
        {
            var hearing = new VideoHearingBuilder().WithCase().Build();
            var newParticipants = 2;
            var participants = hearing.Participants.Where(x => x is not Judge).ToList();

            participants.SkipLast(newParticipants).ToList().ForEach(x => x.ChangePerson(new PersonBuilder(treatPersonAsNew:false).Build()));
            var videoHearingUpdateDate = hearing.UpdatedDate.TrimSeconds();
            
            PublisherHelper.GetExistingParticipantsSinceLastUpdate(hearing, videoHearingUpdateDate).Count().Should().Be(hearing.Participants.Count - newParticipants);
        }

        [Test]
        public void Should_return_All_existing_participants_including_Judge_AND_Excluding_existing_person_Username_not_set()
        {
            var hearing = new VideoHearingBuilder().WithCase().Build();
            var newParticipants = 2;
            var participantWithAccountNotSet = 1;
            var participants = hearing.Participants.Where(x => x is not Judge).ToList();
            var firstParticipant = participants[0];

            participants.SkipLast(newParticipants).ToList().ForEach(x => x.ChangePerson(new PersonBuilder(treatPersonAsNew:false).Build()));
            firstParticipant.Person.GetType().GetProperty("Username")?.SetValue(firstParticipant.Person, firstParticipant.Person.ContactEmail, null);
            var videoHearingUpdateDate = hearing.UpdatedDate.TrimSeconds();
            
            PublisherHelper.GetExistingParticipantsSinceLastUpdate(hearing, videoHearingUpdateDate).Count().Should().Be(hearing.Participants.Count - newParticipants - participantWithAccountNotSet);
        }

        [Test]
        public void Should_return_ONLY_existing_participants_when_existing_persons_added_as_participants_TO_existing_hearing()
        {
            var hearing = new VideoHearingBuilder().WithCase().Build();
            var dateWhenHearingCreated = hearing.CreatedDate.AddDays(-10);
            var existingParticipants = 2;
            var judgeCount = 1;
            hearing.GetType().GetProperty("CreatedDate")?.SetValue(hearing, dateWhenHearingCreated, null);

            var participants = hearing.Participants.Where(x => x is not Judge).ToList();
            participants.Skip(participants.Count - existingParticipants).ToList().ForEach(x => x.ChangePerson(new PersonBuilder(treatPersonAsNew:false).Build()));
            var videoHearingUpdateDate = hearing.UpdatedDate.TrimSeconds();
            
            PublisherHelper.GetExistingParticipantsSinceLastUpdate(hearing, videoHearingUpdateDate).Count().Should().Be(existingParticipants + judgeCount);
        }

        [Test]
        public void Should_return_judge_as_existing_participant()
        {
            var hearing = new VideoHearingBuilder().WithCase().Build();
            var videoHearingUpdateDate = hearing.UpdatedDate.TrimSeconds();
            PublisherHelper.GetExistingParticipantsSinceLastUpdate(hearing, videoHearingUpdateDate).Should().Contain(x => x is Judge);
        }

        [Test]
        public void Should_return_participant_as_existing_participant_when_person_null_as_could_be_judiciary_participant()
        {
            var hearing = new VideoHearingBuilder().WithCase().Build();
            var existingParticipants = 2;
            var participants = hearing.Participants.Where(x => x is not Judge).ToList();
            participants.Skip(participants.Count - existingParticipants).ToList().ForEach(x => x.ChangePerson(new PersonBuilder(treatPersonAsNew:false).Build()));
            var videoHearingUpdateDate = hearing.UpdatedDate.TrimSeconds();
            
            PublisherHelper.GetExistingParticipantsSinceLastUpdate(hearing, videoHearingUpdateDate).Should().Contain(x => x is Judge);
        }

        [Test]
        public void Should_return_participant_as_added_when_participants_Added_to_existing_booking()
        {
            var hearing = new VideoHearingBuilder().WithCase().Build();
            var existingParticipants = 2;
            var participants = hearing.Participants.Where(x => x is not Judge).ToList();
            participants.Skip(participants.Count - existingParticipants).ToList().ForEach(x => x.ChangePerson(new PersonBuilder(treatPersonAsNew:false).Build()));

            PublisherHelper.GetAddedParticipantsSinceLastUpdate(hearing).Should().Contain(x => x is Judge);
        }
    }
}