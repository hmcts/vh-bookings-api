using VhListings.Api.Contract.Requests;

namespace Testing.Common.Builders.Api.Request
{
    public static class CaseRequestBuilder
    {
        public static CaseRequest BuildRequest()
        {
            return new CaseRequest
            {
                Number = "1",
                Name = "Case1"
            };
        }
    }
}