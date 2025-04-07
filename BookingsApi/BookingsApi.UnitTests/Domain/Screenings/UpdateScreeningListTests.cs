using System.Collections.Generic;
using BookingsApi.Domain;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.Participants;

namespace BookingsApi.UnitTests.Domain.Screenings;

public class UpdateScreeningListTests
{
    private Endpoint _endpoint1;
    private Endpoint _endpoint2;
    private Endpoint _endpoint3;
    private Participant _participant1;
    private Participant _participant2;

    [SetUp]
    public void Initialise()
    {
        _endpoint1 = new Endpoint(Guid.NewGuid().ToString(),"EP 1", "ep1@videohearings.net", "1");
        _endpoint2 = new Endpoint(Guid.NewGuid().ToString(),"EP 2", "ep2@videohearings.net", "2");
        _endpoint3 = new Endpoint(Guid.NewGuid().ToString(),"EP 3", "ep3@videohearings.net", "3");
        var participants = new ParticipantBuilder().Build();
        _participant1 = participants[0];
        _participant2 = participants[1];
    }
    
    [Test]
    public void Should_update_screening_list()
    {
        // Arrange
        var endpointsToScreenFrom = new List<Endpoint> { _endpoint2 };
        var participantsToScreenFrom = new List<Participant> { _participant1 };
        _endpoint1.AssignScreening(ScreeningType.Specific, participantsToScreenFrom, endpointsToScreenFrom);
        _endpoint1.SetProtected(nameof(_endpoint1.UpdatedDate), DateTime.UtcNow.AddMinutes(-1));
        var originalUpdatedDate = _endpoint1.Screening.UpdatedDate;
        
        // Act
        endpointsToScreenFrom = [_endpoint3];
        participantsToScreenFrom = [_participant2];
        _endpoint1.AssignScreening(ScreeningType.Specific, participantsToScreenFrom, endpointsToScreenFrom);

        // Assert
        VerifyScreeningEntitiesForEndpoint(endpointsToScreenFrom, participantsToScreenFrom);
        _endpoint1.Screening.UpdatedDate.Should().BeAfter(originalUpdatedDate);
    }

    [Test]
    public void Should_not_update_screening_list_when_not_changed()
    {
        // Arrange
        var endpointsToScreenFrom = new List<Endpoint> { _endpoint2 };
        var participantsToScreenFrom = new List<Participant> { _participant1 };
        _endpoint1.AssignScreening(ScreeningType.Specific, participantsToScreenFrom, endpointsToScreenFrom);
        _endpoint1.SetProtected(nameof(_endpoint1.UpdatedDate), DateTime.UtcNow.AddMinutes(-1));
        var originalUpdatedDate = _endpoint1.Screening.UpdatedDate;
        
        // Act
        _endpoint1.AssignScreening(ScreeningType.Specific, participantsToScreenFrom, endpointsToScreenFrom);
        
        // Assert
        VerifyScreeningEntitiesForEndpoint(endpointsToScreenFrom, participantsToScreenFrom);
        _endpoint1.Screening.UpdatedDate.Should().Be(originalUpdatedDate);
    }

    private void VerifyScreeningEntitiesForEndpoint(List<Endpoint> endpointsToScreenFrom, List<Participant> participantsToScreenFrom)
    {
        var actualScreeningEntities = _endpoint1.Screening.ScreeningEntities;
        var actualScreeningEndpoints = _endpoint1.Screening.GetEndpoints().Select(se => se.Endpoint).ToList();
        var actualScreeningParticipants = _endpoint1.Screening.GetParticipants().Select(se => se.Participant).ToList();
        
        actualScreeningEntities.Count.Should().Be(endpointsToScreenFrom.Count + participantsToScreenFrom.Count);
        actualScreeningEndpoints.Should().BeEquivalentTo(endpointsToScreenFrom);
        actualScreeningParticipants.Should().BeEquivalentTo(participantsToScreenFrom);
    }
}