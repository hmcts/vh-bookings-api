using System;

namespace Testing.Common.Builders.Api
{
    public class ApiUriFactory
    {
        public ApiUriFactory()
        {
            HearingVenueEndpoints = new HearingVenueEndpoints();
            CaseTypesEndpoints = new CaseTypesEndpoints();
            HearingsEndpoints = new HearingsEndpoints();
        }
        
        public HearingVenueEndpoints HearingVenueEndpoints { get; set; }
        public CaseTypesEndpoints CaseTypesEndpoints { get; set; }
        public HearingsEndpoints HearingsEndpoints { get; set; }
    }

    public class HearingVenueEndpoints
    {
        private string ApiRoot => "hearingvenues";
        public string GetVenues => $"{ApiRoot}";
    }
    
    public class CaseTypesEndpoints
    {
        private string ApiRoot => "casetypes";
        public string GetCaseRolesForCaseType(string caseTypeName) => $"{ApiRoot}/{caseTypeName}/caseroles";
        public string GetHearingRolesForCaseRole(string caseTypeName, string caseRoleName) => $"{ApiRoot}/{caseTypeName}/caseroles/{caseRoleName}/hearingroles";
    }

    public class HearingsEndpoints
    {
        private string ApiRoot => "hearings";
        public string GetHearingDetailsById(Guid hearingId) => $"{ApiRoot}/{hearingId}";
        public string BookNewHearing() => $"{ApiRoot}";
        public string UpdateHearingDetails(Guid hearingId) => $"{ApiRoot}/{hearingId}";
    }
}