using System;
using BookingsApi.Domain;

namespace BookingsApi.IntegrationTests.Helper
{
    public class SeedVideoHearingOptions
    {
        public SeedVideoHearingOptions()
        {
            CaseTypeName = Generic;
        }

        private const string Generic = "Generic";

        public string CaseTypeName { get; set; }

        public string HearingTypeName =>
            CaseTypeName == Generic
                ? "Automated Test"
                : "First Directions Appointment";

        public string ApplicantRole = "Applicant";

        public string RespondentRole = "Respondent";

        public string LipHearingRole => "Litigant in person";

        public DateTime? ScheduledDate { get; internal set; }
        
        public HearingVenue HearingVenue { get; set; }
        public bool IncludeJudge { get; set; } = true;
    }
}