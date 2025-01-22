using BookingsApi.Domain;

namespace BookingsApi.UnitTests.Domain.EndPoints;

public class UpdateExternalIdsTests : DomainTests
{
    private Endpoint _endpoint;

    [SetUp]
    public void Setup()
    {
        _endpoint = new Endpoint(Guid.NewGuid().ToString(),"Original DisplayName", "sip@videohearings.net", "1234", null);
    }
    
    [Test]
    public async Task Should_update_external_ids()
    {
        // Arrange
        const string externalReferenceId = "external-reference-id";
        const string measuresExternalReferenceId = "measures-external-reference-id";
        var originalUpdatedDate = _endpoint.UpdatedDate;
        
        // Act
        await ApplyDelay();
        _endpoint.UpdateExternalIds(externalReferenceId, measuresExternalReferenceId);
        
        // Assert
        _endpoint.ExternalReferenceId.Should().Be(externalReferenceId);
        _endpoint.MeasuresExternalId.Should().Be(measuresExternalReferenceId);
        _endpoint.UpdatedDate.Should().BeAfter(originalUpdatedDate);
    }
    
    [Test]
    public async Task Should_not_update_external_ids_when_not_changed()
    {
        // Arrange
        const string externalReferenceId = "external-reference-id";
        const string measuresExternalReferenceId = "measures-external-reference-id";
        _endpoint.UpdateExternalIds(externalReferenceId, measuresExternalReferenceId);
        var originalUpdatedDate = _endpoint.UpdatedDate;
        
        // Act
        await ApplyDelay();
        _endpoint.UpdateExternalIds(externalReferenceId, measuresExternalReferenceId);
        
        // Assert
        _endpoint.ExternalReferenceId.Should().Be(externalReferenceId);
        _endpoint.MeasuresExternalId.Should().Be(measuresExternalReferenceId);
        _endpoint.UpdatedDate.Should().Be(originalUpdatedDate);
    }
}