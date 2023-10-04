using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace BookingsApi.Contract.V1.Helper;

[SuppressMessage("Info Code Smell", "S1133:Deprecated code should be removed")]
public static class HearingAllocationExcludedVenueList
{
    [Obsolete("Use the IsWorkAllocationEnabled property on the HearingVenue object instead of this list")]
    public static readonly IReadOnlyList<string> ExcludedHearingVenueNames = new List<string> (HearingScottishVenueNames.ScottishHearingVenuesList)
    {
        "Middlesbrough County Court"
    };
}