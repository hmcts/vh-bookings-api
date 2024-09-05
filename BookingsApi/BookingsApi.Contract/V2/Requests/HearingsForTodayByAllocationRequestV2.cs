using System;
using System.Collections.Generic;

namespace BookingsApi.Contract.V2.Requests;

public class HearingsForTodayByAllocationRequestV2
{
    public List<Guid> CsoIds { get; set; } = [];
    public bool? Unallocated { get; set; } = null;
}