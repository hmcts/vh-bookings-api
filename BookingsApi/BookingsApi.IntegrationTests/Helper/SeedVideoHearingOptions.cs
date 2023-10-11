namespace BookingsApi.IntegrationTests.Helper
{
    public class SeedVideoHearingOptions
    {
        public SeedVideoHearingOptions()
        {
            CaseTypeName = Generic;
            AddJudge = true;
            AddPanelMember = false;
            EndpointsToAdd = 0;
            AddStaffMember = false;
        }

        private const string Generic = "Generic";
        public string CaseTypeName { get; set; }
        public string HearingTypeName { get; set; } = "Automated Test";
        public string ApplicantRole = "Applicant";
        public string RespondentRole = "Respondent";
        public string LipHearingRole => "Litigant in person";
        public DateTime? ScheduledDate { get; internal set; }
        public HearingVenue HearingVenue { get; set; }
        public Case Case { get; set; }
        public bool AddJudge { get; set; }
        public bool AddPanelMember { get; set; }
        public bool AddStaffMember { get; set; }
        public int EndpointsToAdd { get; set; }
    }
}