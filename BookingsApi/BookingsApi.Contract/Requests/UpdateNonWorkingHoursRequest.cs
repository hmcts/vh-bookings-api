using System.Collections.Generic;

namespace BookingsApi.Contract.Requests;

public class UpdateNonWorkingHoursRequest
{
    public IList<NonWorkingHours> Hours { get; set; }
}
