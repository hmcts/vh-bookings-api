using System;
using System.Collections.Generic;
using BookingsApi.Contract.V2.Requests;
using BookingsApi.Contract.V2.Responses;
using BookingsApi.Domain;
using BookingsApi.Domain.Participants;

namespace Testing.Common.Configuration
{
    public class TestData
    {
        public TestData()
        {
            TestContextData = new Dictionary<string, dynamic>();
        }
        
        public string CaseName { get; set; }
        public BookNewHearingRequestV2 CreateHearingRequest { get; set; }
        public HearingDetailsResponseV2 Hearing { get; set; }
        public Guid NewHearingId { get; set; }
        public Guid OldHearingId { get; set; }
        public Hearing SeededHearing { get; set; }
        public Participant Participant { get; set; }
        public List<ParticipantRequestV2> Participants { get; set; }
        public List<ParticipantResponseV2> ParticipantsResponses { get; set; }
        public List<EndpointResponseV2> EndPointResponses { get; set; }
        public List<string> RemovedPersons { get; set; }
        public UpdateHearingRequestV2 UpdateHearingRequest { get; set; }
        public Dictionary<string, dynamic> TestContextData { get; set; }
        public bool? ZipStatus { get; set; }
    }
}
