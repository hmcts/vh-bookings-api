using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.Validations;

namespace BookingsApi.UnitTests.Domain.Hearing;

public class OverrideSupplierTests
{
    [Test]
    public void should_override_supplier_if_not_set()
    {
        // arrange
        var newSupplier = VideoSupplier.Vodafone;
        var hearing = new VideoHearingBuilder().Build();
        
        // act
        hearing.OverrideSupplier(newSupplier);
        
        // assert
        hearing.GetConferenceSupplier().Should().Be(newSupplier);
    }
    
    [Test]
    public void should_override_supplier_if_already_set_and_not_confirmed()
    {
        // arrange
        var newSupplier = VideoSupplier.Vodafone;
        var hearing = new VideoHearingBuilder().Build();
        hearing.OverrideSupplier(VideoSupplier.Kinly);
        
        // act
        hearing.OverrideSupplier(newSupplier);
        
        // assert
        hearing.GetConferenceSupplier().Should().Be(newSupplier);
    }
    
    [TestCase(BookingStatus.Created)]
    [TestCase(BookingStatus.ConfirmedWithoutJudge)]
    public void should_not_override_supplier_if_hearing_is_confirmed(BookingStatus status)
    {
        // arrange
        var newSupplier = VideoSupplier.Vodafone;
        var hearing = new VideoHearingBuilder().Build();
        hearing.OverrideSupplier(VideoSupplier.Kinly);
        hearing.UpdateStatus(status, "test", null);
        
        // act / assert
        var action = () => hearing.OverrideSupplier(newSupplier);
        action.Should().Throw<DomainRuleException>().And.ValidationFailures
            .Exists(x => x.Message ==DomainRuleErrorMessages.ConferenceSupplierAlreadyExists).Should().BeTrue();
    }
}