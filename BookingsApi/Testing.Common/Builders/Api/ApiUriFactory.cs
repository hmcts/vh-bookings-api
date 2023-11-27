using System;
using System.Linq;
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
            public static string GetHearingDetailsById(string hearingId) => $"{ApiRoot}/{hearingId}";
            public static string GetHearingShellById(Guid hearingId) => $"{ApiRoot}/{hearingId}/status";
            public static string BookNewHearing => $"{ApiRoot}";
            public static string GetHearingsByTypes => $"{ApiRoot}/types";
            public static string CloneHearing(Guid hearingId) => $"{ApiRoot}/{hearingId}/clone";
            public static string UpdateHearingDetails(Guid hearingId) => $"{ApiRoot}/{hearingId}";
            public static string CancelBookingUri(Guid hearingId) => $"{ApiRoot}/{hearingId}/cancel";
            public static string RemoveHearing(Guid hearingId) => $"{ApiRoot}/{hearingId}";
            public static string GetHearingsByUsername(string username) => $"{ApiRoot}/?username={username}";
            public static string GetConfirmedHearingsByUsernameForToday(string username) => $"{ApiRoot}/today/username?username={username}";
            public static string SearchForHearings(SearchForHearingsQuery query) =>
                $"{ApiRoot}/audiorecording/search?{QueryStringBuilder.ConvertToQueryString(query)}";
            public static string AnonymiseHearings() =>  $"{ApiRoot}/anonymisehearings";
            public static string UpdateAudiorecordingZipStatus(Guid hearingId, bool? zipStatus) => $"{ApiRoot}/{hearingId}/audiorecordingzipsatus/zipStatus?zipstatus={zipStatus}";
            public static string GetHearingsByGroupId(Guid groupId) => $"{ApiRoot}/{groupId}/hearings";
            public static string GetHearingsForNotification() => $"{ApiRoot}/notifications/gethearings";
            public static string RebookHearing(Guid hearingId) => $"{ApiRoot}/{hearingId}/conferences";
            public static string UpdateBookingStatus(Guid hearingId) => $"{ApiRoot}/{hearingId}";
            public static string GetBookingStatusById(Guid hearingId) => $"{ApiRoot}/{hearingId}/status";
        }
        
        public static class HearingsEndpointsV2
        {
            private const string ApiRoot = "v2/hearings";
            public static string BookNewHearing => $"{ApiRoot}";
            public static string GetHearingDetailsById(string hearingId) => $"{ApiRoot}/{hearingId}";
            public static string UpdateHearingDetails(Guid hearingId) => $"{ApiRoot}/{hearingId}";

            public static string GetHearingsByGroupId(Guid groupId) => $"{ApiRoot}/{groupId}/hearings";
        }
        
        public static class HearingParticipantsEndpoints
        {
            private const string ApiRoot = "hearings";
            public static string UpdateHearingParticipants(Guid hearingId) => $"{ApiRoot}/{hearingId}/updateParticipants";
        }
        
        public static class HearingParticipantsEndpointsV2
        {
            private const string ApiRoot = "v2/hearings";
            public static string AddParticipantsToHearing(Guid hearingId) => $"{ApiRoot}/{hearingId}/participants";
            public static string UpdateHearingParticipants(Guid hearingId) => $"{ApiRoot}/{hearingId}/updateParticipants";
            public static string UpdateParticipantDetails(Guid hearingId, Guid participantId) => $"{ApiRoot}/{hearingId}/participants/{participantId}";
        }
        
        public static class HearingRolesEndpoints
        {
            private const string ApiRoot = "hearingroles";
            
            public static string GetHearingRoles() => $"{ApiRoot}";
        }
                
        public static class CaseTypesEndpointsV2
        {
            private const string ApiRoot = "v2/casetypes";
            public static string GetCaseRolesForCaseType(string serviceId) => $"{ApiRoot}/{serviceId}/caseroles";
            public static string GetHearingRolesForCaseRole(string serviceId, string caseRoleName) => $"{ApiRoot}/{serviceId}/caseroles/{caseRoleName}/hearingroles";
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
            public static string GetParticipantByUsername(string username) => $"participants/username/{username}";
        }

        public static class PersonEndpoints
        {
            private const string ApiRoot = "persons";
            public static string GetPersonByUsername(string username) => $"{ApiRoot}/username/{username}";
            public static string GetPersonByContactEmail(string contactEmail) => $"{ApiRoot}/contactEmail/{contactEmail}";
            public static string PostPersonBySearchTerm => $"{ApiRoot}";
            public static string GetPersonByClosedHearings() => $"{ApiRoot}/userswithclosedhearings";
            public static string GetHearingsByUsernameForDeletion(string username) => $"{ApiRoot}/username/hearings?username={username}";
            public static string AnonymisePerson(string username) => $"{ApiRoot}/username/{username}/anonymise";
            public static string SearchForNonJudicialPersonsByContactEmail(string contactEmail) => $"{ApiRoot}/?contactEmail={contactEmail}";
            public static string UpdatePersonDetails(Guid personId) => $"{ApiRoot}/{personId}";
            public static string UpdatePersonUsername(string contactEmail, string username) => $"{ApiRoot}/user/{contactEmail}?username={username}";
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
            public static string AllocateHearingManually => $"{ApiRoot}/allocations";

            public static string AllocateHearingAutomatically(Guid hearingId) => $"{ApiRoot}/{hearingId}/allocations/automatic";
        }

        public static class WorkHoursEndpoints
        {
            private const string ApiRoot = "work-hours";
            public static string SaveWorkHours => $"{ApiRoot}/SaveWorkHours";

            public static string UpdateVhoNonAvailabilityHours(string username) => $"NonAvailability/VHO/{username}";

            public static string DeleteVhoNonAvailabilityHours(string username, long id) =>
                $"/NonAvailability/VHO/{username}/{id}";
            public static string GetVhoWorkAvailabilityHours(string username) => $"{ApiRoot}/VHO?username={username}";
            public static string GetVhoNonAvailabilityHours(string username) => $"NonAvailability/VHO?username={username}";
        }
        
        public static class JusticeUserEndpoints
        {
            private const string ApiRoot = "justiceuser";
            public static string AddJusticeUser => $"{ApiRoot}";
            public static string DeleteJusticeUser(Guid justiceUserId) => $"{ApiRoot}/{justiceUserId}";
            public static string RestoreJusticeUser => $"{ApiRoot}/restore";
            public static string EditJusticeUser => $"{ApiRoot}";
            public static string GetJusticeUserByUsername(string username) => $"{ApiRoot}/GetJusticeUserByUsername?username={username}";
            public static string GetJusticeUserList(string term, bool includeDeleted) => $"{ApiRoot}/GetJusticeUserList?term={term}&includeDeleted={includeDeleted.ToString()}";
        }

        public static class JudiciaryParticipantEndpoints
        {
            private const string ApiRoot = "hearings";
            public static string AddJudiciaryParticipantsToHearing(Guid hearingId) => $"{ApiRoot}/{hearingId}/joh";
            public static string RemoveJudiciaryParticipantFromHearing(Guid hearingId, string personalCode) => $"{ApiRoot}/{hearingId}/joh/{personalCode}";
            public static string UpdateJudiciaryParticipant(Guid hearingId, string personalCode) => $"{ApiRoot}/{hearingId}/joh/{personalCode}";
        }

        public static class StaffMemberEndpoints
        {
            private const string ApiRoot = "staffmember";
            public static string GetStaffMemberBySearchTerm(string term) => $"{ApiRoot}?term={term}";
        }
    }
}