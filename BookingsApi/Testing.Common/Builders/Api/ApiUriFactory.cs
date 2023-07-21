using System;
using BookingsApi.Contract.V1.Queries;
using BookingsApi.Contract.V1.Requests;

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
            public static string GetHearingShellById(Guid hearingId) => $"{ApiRoot}/{hearingId}/status";
            public static string BookNewHearing => $"{ApiRoot}";
            public static string GetHearingsByTypes => $"{ApiRoot}/all/types";
            public static string CloneHearing(Guid hearingId) => $"{ApiRoot}/{hearingId}/clone";
            public static string UpdateHearingDetails(Guid hearingId) => $"{ApiRoot}/{hearingId}";
            public static string CancelBookingUri(Guid hearingId) => $"{ApiRoot}/{hearingId}/cancel";
            public static string RemoveHearing(Guid hearingId) => $"{ApiRoot}/{hearingId}";
            public static string GetHearingsByUsername(string username) => $"{ApiRoot}/?username={username}";
            public static string GetConfirmedHearingsByUsernameForToday(string username) => $"{ApiRoot}/today/?username={username}";
            public static string SearchForHearings(SearchForHearingsQuery query) =>
                $"{ApiRoot}/audiorecording/search?{QueryStringBuilder.ConvertToQueryString(query)}";
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
            public static string SearchForNonJudicialPersonsByContactEmail(string contactEmail) => $"{ApiRoot}/?contactEmail={contactEmail}";
            public static string UpdatePersonDetails(Guid personId) => $"{ApiRoot}/{personId}";
            public static string UpdatePersonUsername(string contactEmail, string username) => $"{ApiRoot}/user/{contactEmail}?username={username}";
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

        public static class JudiciaryPersonsEndpoints
        {
            private const string ApiRoot = "judiciaryperson";
            public static string BulkJudiciaryPersons() => $"{ApiRoot}/BulkJudiciaryPersons";
        }

        public static class WorkAllocationEndpoints
        {
            private const string ApiRoot = "hearings";
            
            public static string SearchForAllocationHearings(SearchForAllocationHearingsRequest query) =>
                $"{ApiRoot}/allocation/search?{QueryStringBuilder.ConvertToQueryString(query)}";
            
            public static string GetAllocationsForHearings => $"{ApiRoot}/get-allocation";
        }
        
        public static class JusticeUserEndpoints
        {
            private const string ApiRoot = "justiceuser";
            public static string AddJusticeUser => $"{ApiRoot}";
            public static string DeleteJusticeUser(Guid justiceUserId) => $"{ApiRoot}/{justiceUserId}";
            public static string RestoreJusticeUser => $"{ApiRoot}/restore";
            public static string EditJusticeUser => $"{ApiRoot}";
        }
    }
}