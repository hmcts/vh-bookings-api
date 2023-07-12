using BookingsApi.Domain;
using System.Linq;
using BookingsApi.Contract.Helper;

namespace BookingsApi.DAL.Helper
{
    public static class VideoHearingHelper
    {
        public static string AllocatedVho(Hearing videoHearing)
        {
            var allocatedVho = "Not Allocated";
            var isExcludedVenue = !videoHearing.HearingVenue.IsWorkAllocationEnabled;
            
            if (videoHearing.AllocatedTo == null) {
                if (isExcludedVenue || videoHearing.CaseTypeId == 3) // not required if excluded venue or generic type
                {
                    allocatedVho = "Not Required";
                }
            } else {
                allocatedVho = videoHearing.AllocatedTo.Username;
            }
            return allocatedVho;
        }
    }
}
