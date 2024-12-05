namespace BookingsApi.IntegrationTests.Helper
{
    public class SeedVideoHearingOptions
    {
        private const string Generic = "Generic";
        public string CaseTypeName { get; set; } = Generic;
        public bool ExcludeHearingType { get; set; }
        public DateTime? ScheduledDate { get; internal set; }
        public HearingVenue HearingVenue { get; set; }
        public Case Case { get; set; }
        public bool AddJudge { get; set; } = true;
        public bool AddPanelMember { get; set; } = false;
        public int EndpointsToAdd { get; set; } = 0;
        public int ScheduledDuration { get; set; } = 45;
        public bool AudioRecordingRequired { get; set; } = false;
        
        /// <summary>
        /// If true, it will screen the first individual from the participants list from another and an endpoint 
        /// </summary>
        public bool AddScreening { get; set; } = false;
    }
}