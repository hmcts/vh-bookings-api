using BookingsApi.Domain;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.Participants;
using BookingsApi.Domain.SpecialMeasure;
using BookingsApi.Infrastructure.Services;
using BookingsApi.Infrastructure.Services.Dtos;

namespace BookingsApi.UnitTests.Extensions;

public class EndpointExtensions
{
    private VideoHearing _hearing;
    
    [SetUp]
    public void Setup()
    {
        _hearing = new VideoHearingBuilder().Build();
        _hearing.AddEndpoint(new Endpoint(Guid.NewGuid().ToString(), "Ep1", Guid.NewGuid().ToString(), "pin"));
        _hearing.AddEndpoint(new Endpoint(Guid.NewGuid().ToString(), "Ep2", Guid.NewGuid().ToString(), "pin"));
    }
    
    [Test]
    public void should_return_role_guest_when_no_screening_found()
    {
        foreach (var participant in _hearing.Participants)
        {
            participant.Screening = null;
        }
        
        foreach (var endpoint in _hearing.Endpoints)
        {
            endpoint.Screening = null;
        }


        _hearing.GetEndpoints()[0].GetEndpointConferenceRole(_hearing.GetParticipants(), _hearing.GetEndpoints())
            .Should().Be(ConferenceRole.Guest);
    }
    
    [Test]
    public void should_return_role_guest_when_screen_list_is_empty()
    {
        foreach (var participant in _hearing.Participants)
        {
            participant.Screening = new Screening(ScreeningType.Specific, participant);
        }
        
        foreach (var endpoint in _hearing.Endpoints)
        {
            endpoint.Screening = new Screening(ScreeningType.Specific, endpoint);
        }
        
        _hearing.GetEndpoints()[0].GetEndpointConferenceRole(_hearing.GetParticipants(), _hearing.GetEndpoints())
            .Should().Be(ConferenceRole.Guest);
    }
    
    [Test]
    public void should_return_role_guest_when_endpoint_needs_screening()
    {
        foreach (var p in _hearing.Participants)
        {
            p.Screening = new Screening(ScreeningType.Specific, p);
        }
        
        foreach (var ep in _hearing.Endpoints)
        {
            ep.Screening = new Screening(ScreeningType.All, ep);
        }

        var participant = _hearing.GetParticipants().First(x=> x is Individual);
        var endpoint = _hearing.GetEndpoints()[0];
        _hearing.AssignScreeningForEndpoint(endpoint, ScreeningType.Specific, [participant.ExternalReferenceId]);
        
        endpoint.GetEndpointConferenceRole(_hearing.GetParticipants(), _hearing.GetEndpoints())
            .Should().Be(ConferenceRole.Guest);
    }
    
    [Test]
    public void should_return_role_guest_when_endpoint_is_screened()
    {
        foreach (var p in _hearing.Participants)
        {
            p.Screening = new Screening(ScreeningType.Specific, p);
        }
        
        foreach (var ep in _hearing.Endpoints)
        {
            ep.Screening = new Screening(ScreeningType.All, ep);
        }

        var participant = _hearing.GetParticipants().First(x=> x is Individual);
        var endpoint = _hearing.GetEndpoints()[0];
        _hearing.AssignScreeningForParticipant(participant, ScreeningType.Specific, [endpoint.ExternalReferenceId]);
        
        endpoint.GetEndpointConferenceRole(_hearing.GetParticipants(), _hearing.GetEndpoints())
            .Should().Be(ConferenceRole.Guest);
    }
    
    [Test]
    public void should_return_role_host_when_hearing_has_screening_but_not_for_endpoint()
    {
        foreach (var p in _hearing.Participants)
        {
            p.Screening = new Screening(ScreeningType.Specific, p);
        }
        
        foreach (var ep in _hearing.Endpoints)
        {
            ep.Screening = new Screening(ScreeningType.All, ep);
        }

        var participant = _hearing.GetParticipants().Where(x=> x is Individual).ToList();
        var endpoint = _hearing.GetEndpoints()[0];
        _hearing.AssignScreeningForParticipant(participant[0], ScreeningType.Specific, [participant[1].ExternalReferenceId]);
        
        endpoint.GetEndpointConferenceRole(_hearing.GetParticipants(), _hearing.GetEndpoints())
            .Should().Be(ConferenceRole.Guest);
    }
}