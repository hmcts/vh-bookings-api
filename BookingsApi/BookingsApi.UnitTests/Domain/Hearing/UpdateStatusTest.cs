using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.Participants;
using BookingsApi.Domain.Validations;

namespace BookingsApi.UnitTests.Domain.Hearing
{
    public class UpdateStatusTest
    {
        [Test]
        public void Should_update_hearing_status_from_booked_to_cancelled()
        {
            var hearing = new VideoHearingBuilder().Build();
            var updatedDate = DateTime.UtcNow;
            hearing.UpdateStatus(BookingStatus.Cancelled, "testuser", "cancel reason");
            hearing.UpdatedDate.Should().BeOnOrAfter(updatedDate);
            hearing.Status.Should().Be(BookingStatus.Cancelled);
        }

        [Test]
        public void should_update_status_from_failed_to_created()
        {
            var hearing = new VideoHearingBuilder().Build();
            hearing.SetProtected(nameof(hearing.Status), BookingStatus.Failed);
            hearing.SetProtected(nameof(hearing.UpdatedDate), DateTime.UtcNow);

            var updatedDate = hearing.UpdatedDate;
            var newStatus = BookingStatus.Created;
            hearing.UpdateStatus(newStatus, "testuser", null);
            hearing.Status.Should().Be(BookingStatus.Created);
            hearing.UpdatedDate.Should().BeAfter(updatedDate);
        }

        [Test]
        public void should_not_update_status_from_booked_to_booked()
        {
            var hearing = new VideoHearingBuilder().Build();
            var updatedDate = hearing.UpdatedDate;
            var newStatus = BookingStatus.Booked;
            Action action = () => hearing.UpdateStatus(newStatus, "testuser", null);
            action.Should().Throw<DomainRuleException>().And.ValidationFailures
                .Exists(x => x.Message == $"Cannot change the booking status from {hearing.Status} to {newStatus}")
                .Should().BeTrue();
            hearing.Status.Should().Be(BookingStatus.Booked);
            hearing.UpdatedDate.Should().Be(updatedDate);
        }

        [Test]
        public void should_not_update_status_from_booked_to_created()
        {
            var hearing = new VideoHearingBuilder().Build();
            hearing.UpdateStatus(BookingStatus.Cancelled, "testuser", "Settled");

            var updatedDate = hearing.UpdatedDate;
            var newStatus = BookingStatus.Booked;
            Action action = () => hearing.UpdateStatus(newStatus, "testuser", null);
            action.Should().Throw<DomainRuleException>().And.ValidationFailures
                .Exists(x => x.Message == $"Cannot change the booking status from {hearing.Status} to {newStatus}")
                .Should().BeTrue();
            hearing.Status.Should().Be(BookingStatus.Cancelled);
            hearing.UpdatedDate.Should().Be(updatedDate);
        }

        [Test]
        public void Should_throw_argument_null_exception_when_updatedby_field_is_empty()
        {
            var hearing = new VideoHearingBuilder().Build();

            var newStatus = BookingStatus.Cancelled;
            Action action = () => hearing.UpdateStatus(newStatus, string.Empty, "Settled");
            action.Should().Throw<ArgumentNullException>();
            hearing.Status.Should().Be(BookingStatus.Booked);
        }

        [Test]
        public void Should_throw_argument_null_exception_when_updatedby_field_is_null()
        {
            var hearing = new VideoHearingBuilder().Build();

            var newStatus = BookingStatus.Cancelled;
            Action action = () => hearing.UpdateStatus(newStatus, null, "settled");
            action.Should().Throw<ArgumentNullException>();
            hearing.Status.Should().Be(BookingStatus.Booked);
        }

        [Test]
        public void Should_throw_argument_null_exception_when_cancel_reason_field_is_empty()
        {
            var hearing = new VideoHearingBuilder().Build();

            var newStatus = BookingStatus.Cancelled;
            Action action = () => hearing.UpdateStatus(newStatus, "user", string.Empty);
            action.Should().Throw<ArgumentNullException>();
            hearing.Status.Should().Be(BookingStatus.Booked);
        }

        [Test]
        public void Should_throw_argument_null_exception_when_cancel_reason_field_is_null()
        {
            var hearing = new VideoHearingBuilder().Build();

            var newStatus = BookingStatus.Cancelled;
            Action action = () => hearing.UpdateStatus(newStatus, "user", null);
            action.Should().Throw<ArgumentNullException>();
            hearing.Status.Should().Be(BookingStatus.Booked);
        }

        [Test]
        public void Should_update_hearing_status_to_created()
        {
            var hearing = new VideoHearingBuilder().Build();
            // use reflection because some machines are too quick and the updated date is the same created
            hearing.SetProtected(nameof(hearing.UpdatedDate), DateTime.UtcNow.AddDays(-1));
            var updatedDate = DateTime.UtcNow;
            var updatedBy = "testuser";
            hearing.UpdateStatus(BookingStatus.Created, updatedBy, "");
            hearing.UpdatedDate.Should().BeOnOrAfter(updatedDate);
            hearing.Status.Should().Be(BookingStatus.Created);
            hearing.ConfirmedBy.Should().Be(updatedBy);
            hearing.ConfirmedDate.Should().NotBeNull();
        }
        
        
        [TestCase (BookingStatus.Booked, BookingStatus.BookedWithoutJudge)]
        [TestCase (BookingStatus.BookedWithoutJudge, BookingStatus.Booked)]
        public void Should_update_hearing_status_when_judge_added_or_removed_BookedHearing(BookingStatus currentStatus, BookingStatus expectedStatus)
        {
            var hearing = new VideoHearingBuilder().Build();
            var judge = hearing.Participants.First(e => e is Judge);
            if(currentStatus == BookingStatus.Booked)
                hearing.Participants.Remove(judge);
            else
                hearing.UpdateStatus(BookingStatus.BookedWithoutJudge, "testuser", "");
            hearing.UpdateBookingStatusJudgeRequirement();
            hearing.Status.Should().Be(expectedStatus);
        }

        [TestCase (BookingStatus.ConfirmedWithoutJudge, BookingStatus.Created)]
        [TestCase (BookingStatus.Created, BookingStatus.ConfirmedWithoutJudge)] 
        public void Should_update_hearing_status_when_judge_added_or_removed_ConfirmedHearing(BookingStatus currentStatus, BookingStatus expectedStatus) 
        {
            var hearing = new VideoHearingBuilder().Build();
            var judge = hearing.Participants.First(e => e is Judge);
            if (currentStatus == BookingStatus.Created)
            {
                hearing.UpdateStatus(BookingStatus.Created, "testuser", "");
                hearing.Participants.Remove(judge);
            }
            else
            {
                //Workaround for status to status transition validation rules
                hearing.UpdateStatus(BookingStatus.Created, "testuser", "");
                hearing.UpdateStatus(BookingStatus.ConfirmedWithoutJudge, "testuser", "");
            }

            hearing.UpdateBookingStatusJudgeRequirement();
            hearing.Status.Should().Be(expectedStatus);
            hearing.ConfirmedDate.Should().NotBeNull();
        }
    }
}
