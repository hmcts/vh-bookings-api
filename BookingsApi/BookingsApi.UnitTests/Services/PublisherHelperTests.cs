using BookingsApi.Domain.Participants;
using BookingsApi.Infrastructure.Services.Publishers;

namespace BookingsApi.UnitTests.Services
{
    public class PublisherHelperTests
    {
        [Test]
        public void Should_return_new_participants_when_new_persons_added_as_participants()
        {
            var hearing = new VideoHearingBuilder().WithCase().Build();
            
            PublisherHelper.GetNewParticipantsSinceLastUpdate(hearing).Count().Should().Be(hearing.Participants.Count(x => x is not Judge));
        }

        [Test]
        public void Should_return_ONLY_new_participants_when_new_persons_added_as_participants_WITH_one_exisitng_person()
        {
            var hearing = new VideoHearingBuilder().WithCase().Build();
            var firstPerson = hearing.Participants.Where(x => x is not Judge).ToList()[0].Person;
            firstPerson.GetType().GetProperty("CreatedDate").SetValue(firstPerson,firstPerson.CreatedDate.AddDays(-10), null);

            PublisherHelper.GetNewParticipantsSinceLastUpdate(hearing).Count().Should().Be(hearing.Participants.Count(x => x is not Judge) - 1);
        }

        [Test]
        public void Should_return_new_participants_when_new_persons_added_as_participants_WITH_one_exisitng_person_AND_with_username_not_set()
        {
            var hearing = new VideoHearingBuilder().WithCase().Build();
            var participants = hearing.Participants.Where(x => x is not Judge).ToList();
            participants[0].GetType().GetProperty("CreatedDate").SetValue(participants[0], participants[0].CreatedDate.AddDays(-10), null);
            participants[1].Person.GetType().GetProperty("Username").SetValue(participants[1].Person, participants[1].Person.ContactEmail, null);

            PublisherHelper.GetNewParticipantsSinceLastUpdate(hearing).Count().Should().Be(hearing.Participants.Count(x => x is not Judge) - 1);
        }

        [Test]
        public void Should_return_ONLY_new_participants_when_new_persons_added_as_participants_TO_existing_hearing()
        {
            var hearing = new VideoHearingBuilder().WithCase().Build();
            var dateWhenHearingCreated = hearing.CreatedDate.AddDays(-10);
            hearing.GetType().GetProperty("CreatedDate").SetValue(hearing, dateWhenHearingCreated, null);

            var participants = hearing.Participants.Where(x => x is not Judge).ToList();
            participants.Skip(1).ToList().ForEach(x => x.GetType().GetProperty("CreatedDate").SetValue(x, dateWhenHearingCreated, null));

            PublisherHelper.GetNewParticipantsSinceLastUpdate(hearing).Count().Should().Be(1);
        }

        [Test]
        public void Should_return_All_existing_participants_including_Judge()
        {
            var hearing = new VideoHearingBuilder().WithCase().Build();
            var newParticipants = 2;
            var participants = hearing.Participants.Where(x => x is not Judge).ToList();
            participants.Skip(newParticipants).ToList().ForEach(x => x.GetType().GetProperty("CreatedDate").SetValue(x, x.CreatedDate.AddDays(-10), null));

            PublisherHelper.GetExistingParticipantsSinceLastUpdate(hearing).Count().Should().Be(hearing.Participants.Count - newParticipants);
        }

        [Test]
        public void Should_return_All_existing_participants_including_Judge_AND_Excluding_existing_person_Username_not_set()
        {
            var hearing = new VideoHearingBuilder().WithCase().Build();
            var newParticipants = 2;
            var participantWithAccountNotSet = 1;
            var participants = hearing.Participants.Where(x => x is not Judge).ToList();
            var firstParticipant = participants[0];

            participants.SkipLast(newParticipants).ToList().ForEach(x => x.GetType().GetProperty("CreatedDate").SetValue(x, x.CreatedDate.AddDays(-10), null));
            firstParticipant.Person.GetType().GetProperty("Username").SetValue(firstParticipant.Person, firstParticipant.Person.ContactEmail, null);

            PublisherHelper.GetExistingParticipantsSinceLastUpdate(hearing).Count().Should().Be(hearing.Participants.Count - newParticipants - participantWithAccountNotSet);
        }

        [Test]
        public void Should_return_ONLY_existing_participants_when_existing_persons_added_as_participants_TO_existing_hearing()
        {
            var hearing = new VideoHearingBuilder().WithCase().Build();
            var dateWhenHearingCreated = hearing.CreatedDate.AddDays(-10);
            var existingParticipants = 2;
            var judgeCount = 1;
            hearing.GetType().GetProperty("CreatedDate").SetValue(hearing, dateWhenHearingCreated, null);

            var participants = hearing.Participants.Where(x => x is not Judge).ToList();
            participants.Skip(participants.Count - existingParticipants).ToList().ForEach(x => x.GetType().GetProperty("CreatedDate").SetValue(x, dateWhenHearingCreated, null));

            PublisherHelper.GetNewParticipantsSinceLastUpdate(hearing).Count().Should().Be(existingParticipants + judgeCount);
        }

    }
}