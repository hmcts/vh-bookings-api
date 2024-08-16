using BookingsApi.Contract.V2.Responses;

namespace BookingsApi.Contract.V1.Responses;

public class AllocatedCsoResponse
{
    public HearingDetailsResponseV2 Hearing { get; set; }
    public JusticeUserResponse Cso { get; set; }
    public bool SupportsWorkAllocation { get; set; }
}