using AdminWebsite.Security;
using BookingsApi.Common.Configuration;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace BookingsApi.Common.Security
{
    public class UserApiTokenHandler : BaseServiceTokenHandler
    {
        public UserApiTokenHandler(IOptions<AzureAdConfiguration> azureAdConfiguration,
            IOptions<ServicesConfiguration> serviceSettings,
            IMemoryCache memoryCache,
            ITokenProvider tokenProvider)
            : base(azureAdConfiguration, serviceSettings, memoryCache, tokenProvider)
        {
        }

        protected override string TokenCacheKey => "UserApiServiceToken";
        protected override string ClientResource => ServiceConfiguration.UserApiResourceId;
    }
}