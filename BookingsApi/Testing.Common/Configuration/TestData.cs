﻿using System;
using System.Collections.Generic;
using BookingsApi.Contract.V1.Requests;
using BookingsApi.Contract.V1.Responses;
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
        public BookNewHearingRequest CreateHearingRequest { get; set; }
        public HearingDetailsResponse Hearing { get; set; }
        public Guid NewHearingId { get; set; }
        public Guid OldHearingId { get; set; }
        public Hearing SeededHearing { get; set; }
        public Participant Participant { get; set; }
        public List<ParticipantRequest> Participants { get; set; }
        public List<ParticipantResponse> ParticipantsResponses { get; set; }
        public List<EndpointResponse> EndPointResponses { get; set; }
        public List<string> RemovedPersons { get; set; }
        public UpdateHearingRequest UpdateHearingRequest { get; set; }
        public Dictionary<string, dynamic> TestContextData { get; set; }
        public bool? ZipStatus { get; set; }
    }
}
