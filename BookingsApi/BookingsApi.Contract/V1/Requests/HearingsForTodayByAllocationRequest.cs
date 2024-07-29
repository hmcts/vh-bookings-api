using System;
using System.Collections.Generic;

namespace BookingsApi.Contract.V1.Requests;

public class HearingsForTodayByAllocationRequest
{
    public List<Guid> CsoIds { get; set; } = [];
    public bool? Unallocated { get; set; } = null;
}