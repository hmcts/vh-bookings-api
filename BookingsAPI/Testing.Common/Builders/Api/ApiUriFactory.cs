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
            ParticipantsEndpoints = new ParticipantsEndpoints();
            PersonEndpoints = new PersonEndpoints();
            HealthCheckEndpoints = new HealthCheckEndpoints();
            SuitabilityAnswerEndpoints = new SuitabilityAnswerEndpoints();
        }

        public HearingVenueEndpoints HearingVenueEndpoints { get; set; }
        public CaseTypesEndpoints CaseTypesEndpoints { get; set; }
        public HearingsEndpoints HearingsEndpoints { get; set; }
        public ParticipantsEndpoints ParticipantsEndpoints { get; set; }
        public PersonEndpoints PersonEndpoints { get; set; }
        public HealthCheckEndpoints HealthCheckEndpoints { get; set; }
        public SuitabilityAnswerEndpoints SuitabilityAnswerEndpoints { get; set; }
    }

    public class HearingVenueEndpoints
    {
        private string ApiRoot => "hearingvenues";
        public string GetVenues => $"{ApiRoot}";
    }

    public class HealthCheckEndpoints
    {
        private string ApiRoot => "healthCheck";
        public string HealthCheck => $"{ApiRoot}/health";
    }

    public class CaseTypesEndpoints
    {
        private string ApiRoot => "casetypes";
        public string GetCaseRolesForCaseType(string caseTypeName) => $"{ApiRoot}/{caseTypeName}/caseroles";
        public string GetHearingRolesForCaseRole(string caseTypeName, string caseRoleName) => $"{ApiRoot}/{caseTypeName}/caseroles/{caseRoleName}/hearingroles";

        public string GetCaseTypes() => $"{ApiRoot}/";
    }

    public class HearingsEndpoints
    {
        private string ApiRoot => "hearings";
        public string GetHearingDetailsById(Guid hearingId) => $"{ApiRoot}/{hearingId}";
        public string BookNewHearing() => $"{ApiRoot}";
        public string UpdateHearingDetails(Guid hearingId) => $"{ApiRoot}/{hearingId}";
        public string RemoveHearing(Guid hearingId) => $"{ApiRoot}/{hearingId}";

        public string GetHearingsByCaseType(int caseType) => $"{ApiRoot}/types?types={caseType}";

        public string GetHearingsByAnyCaseType(int limit = 100) => $"{ApiRoot}/types?limit={limit}";

        public string GetHearingsByAnyCaseTypeAndCursor(string cursor) => $"{ApiRoot}/types?cursor{cursor}";
        public string GetHearingsByUsername(string username) => $"{ApiRoot}/?username={username}";
    }

    public class ParticipantsEndpoints
    {
        private string ApiRoot => "hearings";
        public string GetAllParticipantsInHearing(Guid hearingId) => $"{ApiRoot}/{hearingId}/participants";

        public string GetParticipantInHearing(Guid hearingId, Guid participantId) =>
            $"{ApiRoot}/{hearingId}/participants/{participantId}";

        public string AddParticipantsToHearing(Guid hearingId) => $"{ApiRoot}/{hearingId}/participants";

        public string RemoveParticipantFromHearing(Guid hearingId, Guid participantId) =>
            $"{ApiRoot}/{hearingId}/participants/{participantId}";

        public string UpdateParticipantDetails(Guid hearingId, Guid participantId) =>
            $"{ApiRoot}/{hearingId}/participants/{participantId}";

        public string UpdateSuitabilityAnswers(Guid hearingId, Guid participantId) =>
            $"{ApiRoot}/{hearingId}/participants/{participantId}/suitability-answers";
    }

    public class PersonEndpoints
    {
        private string ApiRoot => "persons";
        public string GetPersonByUsername(string username) => $"{ApiRoot}/username/{username}";
        public string GetPersonByContactEmail(string contactEmail) => $"{ApiRoot}/contactEmail/{contactEmail}";
        public string PostPersonBySearchTerm() => $"{ApiRoot}";
        public string GetPersonSuitabilityAnswers(string username) => $"{ApiRoot}/username/{username}/suitability-answers";
    }

    public class SuitabilityAnswerEndpoints
    {
        private string ApiRoot => "suitability-answers";
        public string GetSuitabilityAnswers(string cursor) => $"{ApiRoot}/{cursor}";
        public string GetSuitabilityAnswerWithLimit(string cursor="", int limit = 100) => $"{ApiRoot}/?cursor={cursor}&limit={limit}";
    }

}