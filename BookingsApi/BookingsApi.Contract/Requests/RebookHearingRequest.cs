using System;
using System.ComponentModel;

namespace BookingsApi.Contract.Requests
{
    public class RebookHearingRequest
    {
        [DefaultValue(false)]
        public bool IsMultiDayHearing { get; set; } = false;
    }
}
