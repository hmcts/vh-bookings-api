namespace BookingsApi.IntegrationTests.Helper
{
    public class SeedVideoHearingOptions
    {
        private const string Generic = "Generic";
        public string CaseTypeName { get; set; } = Generic;
        public string HearingTypeName =>
            CaseTypeName == Generic
                ? "Automated Test"
                : "First Directions Appointment"; 
        public bool ExcludeHearingType { get; set; }
        public string ApplicantRole = "Applicant";
        public string RespondentRole = "Respondent";
        public string LipHearingRole => "Litigant in person";
        public DateTime? ScheduledDate { get; internal set; }
        public HearingVenue HearingVenue { get; set; }
        public Case Case { get; set; }
        public bool AddJudge { get; set; } = true;
        public bool AddPanelMember { get; set; } = false;
        public bool AddStaffMember { get; set; } = false;
        public int EndpointsToAdd { get; set; } = 0;
        public int ScheduledDuration { get; set; } = 45;
    }
}