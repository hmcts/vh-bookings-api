using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.SpecialMeasure;

namespace BookingsApi.UnitTests.Domain.Screenings;

public class UpdateTypeTests
{
    [Test]
    public void Should_update_type()
    {
        // Arrange
        var participant = new ParticipantBuilder().IndividualParticipantApplicant;
        var screening = new Screening(ScreeningType.Specific, participant);
        const ScreeningType newType = ScreeningType.All;
        screening.SetProtected(nameof(screening.UpdatedDate), DateTime.UtcNow.AddMinutes(-1));
        var originalUpdatedDate = screening.UpdatedDate;
        
        // Act
        screening.UpdateType(newType);
        
        // Assert
        screening.Type.Should().Be(newType);
        screening.UpdatedDate.Should().BeAfter(originalUpdatedDate);
    }

    [Test]
    public void Should_not_update_type_when_not_changed()
    {
        // Arrange
        var participant = new ParticipantBuilder().IndividualParticipantApplicant;
        var screening = new Screening(ScreeningType.Specific, participant);
        var type = screening.Type;
        screening.SetProtected(nameof(screening.UpdatedDate), DateTime.UtcNow.AddMinutes(-1));
        var originalUpdatedDate = screening.UpdatedDate;
        
        // Act
        screening.UpdateType(type);
        
        // Assert
        screening.Type.Should().Be(type);
        screening.UpdatedDate.Should().Be(originalUpdatedDate);
    }
}