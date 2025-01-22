using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.SpecialMeasure;

namespace BookingsApi.UnitTests.Domain.Screenings;

public class UpdateTypeTests : DomainTests
{
    [Test]
    public async Task Should_update_type()
    {
        // Arrange
        var participant = new ParticipantBuilder().IndividualParticipantApplicant;
        var screening = new Screening(ScreeningType.Specific, participant);
        var newType = ScreeningType.All;
        var originalUpdatedDate = screening.UpdatedDate;
        
        // Act
        await ApplyDelay();
        screening.UpdateType(newType);
        
        // Assert
        screening.Type.Should().Be(newType);
        screening.UpdatedDate.Should().BeAfter(originalUpdatedDate);
    }

    [Test]
    public async Task Should_not_update_type_when_not_changed()
    {
        // Arrange
        var participant = new ParticipantBuilder().IndividualParticipantApplicant;
        var screening = new Screening(ScreeningType.Specific, participant);
        var type = screening.Type;
        var originalUpdatedDate = screening.UpdatedDate;
        
        // Act
        await ApplyDelay();
        screening.UpdateType(type);
        
        // Assert
        screening.Type.Should().Be(type);
        screening.UpdatedDate.Should().Be(originalUpdatedDate);
    }
}