using System;
using System.Collections.Generic;
using Bookings.Api.Contract.Requests;
using Bookings.Api.Contract.Responses;
using Bookings.Domain.Participants;

namespace Testing.Common.Configuration
{
    public class TestData
    {
        public List<SuitabilityAnswersRequest> Answers { get; set; }
        public BookNewHearingRequest CreateHearingRequest { get; set; }
        public HearingDetailsResponse Hearing { get; set; }
        public Guid NewHearingId { get; set; }
        public Guid OldHearingId { get; set; }
        public Participant Participant { get; set; }
        public List<ParticipantRequest> Participants { get; set; }
        public List<ParticipantResponse> ParticipantsResponses { get; set; }
        public List<string> RemovedPersons { get; set; }
        public UpdateHearingRequest UpdateHearingRequest { get; set; }
    }
}
