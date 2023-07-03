using System;
using System.Collections.Generic;

namespace BookingsApi.Contract.Helper;

[Obsolete("Do not use as it will be replaced by an extra column IsWorkAllocationEnabled in the HearingVenue", false)]
public static class HearingAllocationExcludedVenueList
{
    public static readonly IReadOnlyList<string> ExcludedHearingVenueNames = new List<string> (HearingScottishVenueNames.ScottishHearingVenuesList)
    {
        "Teesside Combined Court Centre",
        "Teesside Magistrates Court",
        "Middlesbrough County Court"
    };
}