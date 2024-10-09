using System;
using BookingsApi.Domain.Enumerations;

namespace BookingsApi.Infrastructure.Services.Dtos
{
    public class HearingDto
    {
        public Guid HearingId { get; set; }
        public Guid? GroupId { get; set; }
        public DateTime ScheduledDateTime { get; set;}
        public int ScheduledDuration { get;set; }
        public string CaseType { get; set;}
        public string CaseNumber { get; set;}
        public string CaseName { get; set;}
        public string HearingVenueName { get; set;}
        public bool RecordAudio { get; set; }
        public string HearingType { get; set; }
        public string CaseTypeServiceId { get; set; }
        public VideoSupplier VideoSupplier { get; set; }
        public ConferenceRoomType ConferenceRoomType { get; set; }
    }

    public enum ConferenceRoomType
    {
        VMR,
        VA
    }
}