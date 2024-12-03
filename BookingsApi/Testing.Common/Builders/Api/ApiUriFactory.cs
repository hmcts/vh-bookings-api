using System;
using BookingsApi.Contract.V1.Requests;

namespace Testing.Common.Builders.Api
{
    public static class ApiUriFactory
    {
        public static class HearingsEndpoints
        {
            private const string ApiRoot = "hearings";
            public static string CancelHearingsInGroupId(Guid groupId) => $"{ApiRoot}/{groupId}/hearings/cancel";
            public static string RebookHearing(Guid hearingId) => $"{ApiRoot}/{hearingId}/conferences";
            public static string RemoveHearing(Guid hearingId) => $"{ApiRoot}/{hearingId}";
            public static string UpdateBookingStatus(Guid hearingId) => $"{ApiRoot}/{hearingId}";
            public static string FailBookingUri(Guid hearingId) => $"{ApiRoot}/{hearingId}/fail";
            public static string CancelBookingUri(Guid hearingId) => $"{ApiRoot}/{hearingId}/cancel";
            public static string GetBookingStatusById(Guid hearingId) => $"{ApiRoot}/{hearingId}/status";
            public static string GetHearingsByTypes => $"{ApiRoot}/types";
        }
        
        public static class HearingsEndpointsV2
        {
            private const string ApiRoot = "v2/hearings";
            public static string BookNewHearing => $"{ApiRoot}";
            public static string GetHearingDetailsById(string hearingId) => $"{ApiRoot}/{hearingId}";
            public static string UpdateHearingDetails(Guid hearingId) => $"{ApiRoot}/{hearingId}";

            public static string GetHearingsByGroupId(Guid groupId) => $"{ApiRoot}/{groupId}/hearings";
            public static string UpdateHearingsInGroupId(Guid groupId) => $"{ApiRoot}/{groupId}/hearings";
            public static string GetHearingsForToday() => $"{ApiRoot}/today";
            public static string GetHearingsForTodayByVenue() => $"{ApiRoot}/today/venue";
            public static string GetConfirmedHearingsByUsernameForToday(string username) => $"{ApiRoot}/today/username?username={username}";
            public static string CloneHearing(Guid hearingId) => $"{ApiRoot}/{hearingId}/clone";
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
        
        public static class HearingRolesEndpointsV2
        {
            private const string ApiRoot = "v2/hearingroles";
            public static string GetHearingRoles() => $"{ApiRoot}";
        }
                
        public static class CaseTypesEndpointsV2
        {
            private const string ApiRoot = "v2/casetypes";
            public static string GetCaseTypes => $"{ApiRoot}";
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
            public static string UpdatePersonUsername(string contactEmail, string username) => $"{ApiRoot}/user/{contactEmail}?username={username}";
        }
        
        public static class PersonEndpointsV2
        {
            private const string ApiRoot = "v2/persons";
            public static string UpdatePersonDetails(Guid personId) => $"{ApiRoot}/{personId}";
            public static string SearchForNonJudicialPersonsByContactEmail(string contactEmail) => $"{ApiRoot}/?contactEmail={contactEmail}";
        }
            
        public static class JvEndPointEndpoints
        {
            private static string ApiRoot => "hearings";
            public static string RemoveEndPointFromHearing(Guid hearingId, Guid endpointId) => $"{ApiRoot}/{hearingId}/endpoints/{endpointId}";
        }

        public static class JudiciaryPersonsEndpoints
        {
            private const string ApiRoot = "judiciaryperson";
            public static string BulkJudiciaryPersons() => $"{ApiRoot}/BulkJudiciaryPersons";
            public static string PostJudiciaryPersonBySearchTerm() => $"{ApiRoot}/search";
        }

        public static class JudiciaryPersonsStagingEndpoints
        {
            private const string ApiRoot = "judiciarypersonstaging";
            public static string BulkJudiciaryPersonsStaging() => $"{ApiRoot}/BulkJudiciaryPersonsStaging";
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
            private const string ApiRoot = "v2/hearings";
            public static string AddJudiciaryParticipantsToHearing(Guid hearingId) => $"{ApiRoot}/{hearingId}/joh";
            public static string RemoveJudiciaryParticipantFromHearing(Guid hearingId, string personalCode) => $"{ApiRoot}/{hearingId}/joh/{personalCode}";
            public static string UpdateJudiciaryParticipant(Guid hearingId, string personalCode) => $"{ApiRoot}/{hearingId}/joh/{personalCode}";
            public static string ReassignJudiciaryJudge(Guid hearingId) => $"{ApiRoot}/{hearingId}/joh/judge";
        }

        public static class HearingListsEndpoints
        {
            private const string ApiRoot = "v2/hearings";
            public static string GetHearingsForTodayByCsos => $"{ApiRoot}/today/csos";
        }
    }
}