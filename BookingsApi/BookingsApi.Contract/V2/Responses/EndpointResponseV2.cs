using System;

namespace BookingsApi.Contract.V2.Responses
{
    public class EndpointResponseV2
    {
        public Guid Id { get; set; }
        public string DisplayName { get; set; }
        public string Sip { get; set; }
        public string Pin { get; set; }
        public Guid? DefenceAdvocateId { get; set; }
    }
}