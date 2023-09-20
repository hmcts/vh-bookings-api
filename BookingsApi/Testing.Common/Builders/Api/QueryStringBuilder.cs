using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace Testing.Common.Builders.Api
{
    public static class QueryStringBuilder
    {
        public static string ConvertToQueryString(object obj)
        {
            var step1 = JsonConvert.SerializeObject(obj);

            var step2 = JsonConvert.DeserializeObject<IDictionary<string, string>>(step1);

            var step3 = step2.Where(x => !string.IsNullOrEmpty(x.Value))
                .Select(x => HttpUtility.UrlEncode(x.Key) + "=" + HttpUtility.UrlEncode(x.Value));

            return string.Join("&", step3);
        }
    }
}