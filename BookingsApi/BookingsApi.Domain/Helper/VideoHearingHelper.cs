namespace BookingsApi.Domain.Helper
{
    public static class VideoHearingHelper
    {
        public const string NotRequired = "Not Required";
        public const string NotAllocated = "Not Allocated";
        public static string AllocatedVho(Hearing videoHearing)
        {
            var allocatedVho = NotAllocated;
            var isExcludedVenue = !videoHearing.HearingVenue.IsWorkAllocationEnabled;
            
            if (videoHearing.AllocatedTo == null) {
                if (isExcludedVenue || videoHearing.CaseTypeId == 3) // not required if excluded venue or generic type
                {
                    allocatedVho = NotRequired; 
                }
            } else {
                allocatedVho = videoHearing.AllocatedTo.Username;
            }
            return allocatedVho;
        }
    }
}