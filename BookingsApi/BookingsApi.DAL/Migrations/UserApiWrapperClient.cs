using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserApi.Client;

namespace BookingsApi.DAL.Migrations
{
    public class UserApiWrapperClient
    {
        private static IUserApiClient _userApiClient;
        private static ILogger _logger;

        public UserApiWrapperClient(IUserApiClient userApiClient, ILogger logger)
        {
            _userApiClient = userApiClient;
            _logger = logger;
        }


        public static async Task<List<string>> GetJudgesFromAdAsync()
        {
            List<string> judges = new List<string>();

            try
            {

            judges =  (await _userApiClient.GetJudgesAsync()).Select(x => x.Email).ToList();
            }catch(Exception ex)
            {
                _logger.LogError($"Error in retrieving the judges from AD, Message: {ex.Message}");
            }

            return judges;
        }
    }
}
