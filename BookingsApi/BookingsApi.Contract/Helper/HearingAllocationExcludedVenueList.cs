using System;
using System.Collections.Generic;

namespace BookingsApi.Contract.Helper;

public static class HearingAllocationExcludedVenueList
{
    [Obsolete("Use the IsWorkAllocationEnabled property on the HearingVenue object instead of this list")]
    public static readonly IReadOnlyList<string> ExcludedHearingVenueNames = new List<string> (HearingScottishVenueNames.ScottishHearingVenuesList)
    {
        "Teesside Combined Court Centre",
        "Teesside Magistrates Court",
        "Middlesbrough County Court"
    };
}