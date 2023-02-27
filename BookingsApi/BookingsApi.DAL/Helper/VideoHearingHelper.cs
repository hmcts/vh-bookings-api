﻿using BookingsApi.Domain;
using System.Linq;
using BookingsApi.DAL.Helper;

namespace BookingsApi.DAL.Helper
{
    public static class VideoHearingHelper
    {
        public static string AllocatedVho(Hearing videoHearing)
        {
            var allocatedVho = "Not Allocated";
            var isScottishVenue =
                HearingScottishVenueNames.ScottishHearingVenuesList.Any(venueName =>
                    venueName == videoHearing.HearingVenueName);
            if (videoHearing.AllocatedTo == null) {
                if (isScottishVenue || videoHearing.CaseTypeId == 3) // not required if scottish venue or generic type
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
