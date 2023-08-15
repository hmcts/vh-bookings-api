using System.Collections.Generic;

namespace BookingsApi.Contract.V1.Requests;

public class UpdateNonWorkingHoursRequest
{
    public IList<NonWorkingHours> Hours { get; set; }
}
