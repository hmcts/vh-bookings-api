using BookingsApi.Domain;

namespace BookingsApi.UnitTests.Domain.EndPoints;

public class UpdateExternalIdsTests
{
    private Endpoint _endpoint;

    [SetUp]
    public void Setup()
    {
        _endpoint = new Endpoint(Guid.NewGuid().ToString(),"Original DisplayName", "sip@videohearings.net", "1234", null);
    }
    
    [Test]
    public void Should_update_external_ids()
    {
        // Arrange
        const string externalReferenceId = "external-reference-id";
        const string measuresExternalReferenceId = "measures-external-reference-id";
        _endpoint.SetProtected(nameof(_endpoint.UpdatedDate), DateTime.UtcNow.AddMinutes(-1));
        var originalUpdatedDate = _endpoint.UpdatedDate;
        
        // Act
        _endpoint.UpdateExternalIds(externalReferenceId, measuresExternalReferenceId);
        
        // Assert
        _endpoint.ExternalReferenceId.Should().Be(externalReferenceId);
        _endpoint.MeasuresExternalId.Should().Be(measuresExternalReferenceId);
        _endpoint.UpdatedDate.Should().BeAfter(originalUpdatedDate);
    }
    
    [Test]
    public void Should_not_update_external_ids_when_not_changed()
    {
        // Arrange
        const string externalReferenceId = "external-reference-id";
        const string measuresExternalReferenceId = "measures-external-reference-id";
        _endpoint.UpdateExternalIds(externalReferenceId, measuresExternalReferenceId);
        _endpoint.SetProtected(nameof(_endpoint.UpdatedDate), DateTime.UtcNow.AddMinutes(-1));
        var originalUpdatedDate = _endpoint.UpdatedDate;
        
        // Act
        _endpoint.UpdateExternalIds(externalReferenceId, measuresExternalReferenceId);
        
        // Assert
        _endpoint.ExternalReferenceId.Should().Be(externalReferenceId);
        _endpoint.MeasuresExternalId.Should().Be(measuresExternalReferenceId);
        _endpoint.UpdatedDate.Should().Be(originalUpdatedDate);
    }
}