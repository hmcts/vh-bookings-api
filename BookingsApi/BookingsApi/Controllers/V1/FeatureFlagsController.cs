using System.Net;
using BookingsApi.Services;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace BookingsApi.Controllers.V1
{
    [Produces("application/json")]
    [Route("feature-flags")]
    [ApiController]
    [ApiVersion("1.0")]
    public class FeatureFlagsController : Controller
    {
        private readonly IFeatureFlagService _featureFlagsService;

        public FeatureFlagsController(IFeatureFlagService featureFlagsService)
        {
            _featureFlagsService = featureFlagsService;
        }

        /// <summary>
        /// returns wheather a feature is enabled or not
        /// </summary>
        /// <param name="featureName"></param>
        /// <returns>bool</returns>
        [HttpGet]
        [OpenApiOperation("GetFeatureFlag")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        public ActionResult<bool> GetFeatureFlag(string featureName)
        {
            return _featureFlagsService.GetFeatureFlag(featureName);
        }
    }
}