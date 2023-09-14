using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.Validations;

namespace BookingsApi.UnitTests.Domain.Hearing;

public class CanHearingBeUpdatedTests
{
    [Test]
    public void should_allow_changes_when_hearing_is_not_confirmed()
    {
        //Arrange
        var dateTime = DateTime.UtcNow.AddMinutes(35);
        var hearing = new VideoHearingBuilder(dateTime).Build();

        // Act/Assert
        Assert.DoesNotThrow(() =>hearing.ValidateChangeAllowed());
    }

    [Test]
    public void should_allow_changes_if_hearing_is_confirmed_and_scheduled_to_start_more_than_thirty_minutes_from_now()
    {
        //Arrange
        var dateTime = DateTime.UtcNow.AddMinutes(35);
        var hearing = new VideoHearingBuilder(dateTime).Build();
        hearing.UpdateStatus(BookingStatus.Created, "test@test.com", null);

        //Act
        Assert.DoesNotThrow(() =>hearing.ValidateChangeAllowed());
    }

    [Test]
    public void should_not_allow_changes_if_hearing_is_cancelled()
    {
        //Arrange
        var dateTime = DateTime.UtcNow.AddMinutes(35);
        var hearing = new VideoHearingBuilder(dateTime).Build();
        hearing.UpdateStatus(BookingStatus.Cancelled, "test@test.com", "cancelled by test");

        //Act/Assert
        Assert.Throws<DomainRuleException>(() => hearing.ValidateChangeAllowed())!.ValidationFailures.Should()
            .Contain(x => x.Message == DomainRuleErrorMessages.CannotEditACancelledHearing);
    }

    [Test]
    public void
        should_not_allow_changes_if_hearing_is_confirmed_and_scheduled_to_start_less_than_thirty_minutes_from_now()
    {
        //Arrange
        var dateTime = DateTime.UtcNow.AddMinutes(25);
        var hearing = new VideoHearingBuilder(dateTime).Build();
        hearing.UpdateStatus(BookingStatus.Created, "test@test.com", null);
        
        //Act/Assert
        Assert.Throws<DomainRuleException>(() => hearing.ValidateChangeAllowed())!.ValidationFailures.Should()
            .Contain(x => x.Message == DomainRuleErrorMessages.CannotEditAHearingCloseToStartTime);
    }
}