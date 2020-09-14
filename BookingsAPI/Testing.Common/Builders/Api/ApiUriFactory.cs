using System;

namespace Testing.Common.Builders.Api
{
    public static class ApiUriFactory
    {
        public static class CaseTypesEndpoints
        {
            private const string ApiRoot = "casetypes";
            public static string GetCaseRolesForCaseType(string caseTypeName) => $"{ApiRoot}/{caseTypeName}/caseroles";
            public static string GetHearingRolesForCaseRole(string caseTypeName, string caseRoleName) => $"{ApiRoot}/{caseTypeName}/caseroles/{caseRoleName}/hearingroles";
            public static string GetCaseTypes => $"{ApiRoot}/";
        }

        public static class HealthCheckEndpoints
        {
            private static string ApiRoot => "healthCheck";
            public static string HealthCheck => $"{ApiRoot}/health";
        }

        public static class HearingsEndpoints
        {
            private const string ApiRoot = "hearings";
            public static string GetHearingDetailsById(Guid hearingId) => $"{ApiRoot}/{hearingId}";
            public static string BookNewHearing => $"{ApiRoot}";
            public static string UpdateHearingDetails(Guid hearingId) => $"{ApiRoot}/{hearingId}";
            public static string RemoveHearing(Guid hearingId) => $"{ApiRoot}/{hearingId}";
            public static string GetHearingsByCaseType(int caseType) => $"{ApiRoot}/types?types={caseType}";
            public static string GetHearingsByAnyCaseType(int limit = 100) => $"{ApiRoot}/types?limit={limit}";
            public static string GetHearingsByAnyCaseTypeAndCursor(string cursor) => $"{ApiRoot}/types?cursor{cursor}";
            public static string GetHearingsByUsername(string username) => $"{ApiRoot}/?username={username}";
            public static string GetHearingsByCaseNumber(string caseNumber) => $"{ApiRoot}/audiorecording/casenumber?caseNumber={caseNumber}";
            public static string AnonymiseHearings() =>  $"{ApiRoot}/anonymisehearings";
            public static string UpdateAudiorecordingZipStatus(Guid hearingId, bool? zipStatus) => $"{ApiRoot}/{hearingId}/audiorecordingzipsatus/zipStatus?zipstatus={zipStatus}";
        }

        public static class HearingVenueEndpoints
        {
            private const string ApiRoot = "hearingvenues";
            public static string GetVenues => $"{ApiRoot}";
        }

        public static class ParticipantsEndpoints
        {
            private const string ApiRoot = "hearings";
            public static string GetAllParticipantsInHearing(Guid hearingId) => $"{ApiRoot}/{hearingId}/participants";
            public static string GetParticipantInHearing(Guid hearingId, Guid participantId) => $"{ApiRoot}/{hearingId}/participants/{participantId}";
            public static string AddParticipantsToHearing(Guid hearingId) => $"{ApiRoot}/{hearingId}/participants";
            public static string RemoveParticipantFromHearing(Guid hearingId, Guid participantId) => $"{ApiRoot}/{hearingId}/participants/{participantId}";
            public static string UpdateParticipantDetails(Guid hearingId, Guid participantId) => $"{ApiRoot}/{hearingId}/participants/{participantId}";
            public static string UpdateSuitabilityAnswers(Guid hearingId, Guid participantId) => $"{ApiRoot}/{hearingId}/participants/{participantId}/suitability-answers";
        }

        public static class PersonEndpoints
        {
            private const string ApiRoot = "persons";
            public static string GetPersonByUsername(string username) => $"{ApiRoot}/username/{username}";
            public static string GetPersonByContactEmail(string contactEmail) => $"{ApiRoot}/contactEmail/{contactEmail}";
            public static string PostPersonBySearchTerm => $"{ApiRoot}";
            public static string GetPersonSuitabilityAnswers(string username) => $"{ApiRoot}/username/{username}/suitability-answers";
            public static string GetPersonByClosedHearings() => $"{ApiRoot}/userswithclosedhearings";
            public static string GetHearingsByUsernameForDeletion(string username) => $"{ApiRoot}/username/hearings?username={username}";
            public static string AnonymisePerson(string username) => $"{ApiRoot}/username/{username}/anonymise";
        }

        public static class SuitabilityAnswerEndpoints
        {
            private const string ApiRoot = "suitability-answers";
            public static string GetSuitabilityAnswers(string cursor) => $"{ApiRoot}/{cursor}";
            public static string GetSuitabilityAnswerWithLimit(string cursor = "", int limit = 100) => $"{ApiRoot}/?cursor={cursor}&limit={limit}";
        }
            
        public static class JVEndPointEndpoints
        {
            private static string ApiRoot => "hearings";
            public static string AddEndpointToHearing(Guid hearingId) => $"{ApiRoot}/{hearingId}/endpoints";
            public static string UpdateEndpoint(Guid hearingId, Guid endpointId) => $"{ApiRoot}/{hearingId}/endpoints/{endpointId}";
            public static string RemoveEndPointFromHearing(Guid hearingId, Guid endpointId) => $"{ApiRoot}/{hearingId}/endpoints/{endpointId}";
        }
    }
}