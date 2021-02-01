namespace Bookings.IntegrationTests.Helper
{
    public class SeedVideoHearingOptions
    {
        public SeedVideoHearingOptions()
        {
            CaseTypeName = CivilMoneyClaimsType;
        }

        private const string CivilMoneyClaimsType = "Civil Money Claims";


        public string CaseTypeName { get; set; }

        public string HearingTypeName =>
            CaseTypeName == CivilMoneyClaimsType
                ? "Application to Set Judgment Aside"
                : "First Directions Appointment";

        public string ClaimantRole =>
            CaseTypeName == CivilMoneyClaimsType ? "Claimant" : "Applicant";

        public string DefendentRole => CaseTypeName == CivilMoneyClaimsType
            ? "Defendant"
            : "Respondent";

        public string LipHearingRole => "Litigant in person";
    }
}