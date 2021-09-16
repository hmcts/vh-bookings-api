using BookingsApi.Common.Configuration;
using BookingsApi.Services;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using System.Net;

namespace BookingsApi.Controllers
{
    [Produces("application/json")]
    [Route("feature-flags")]
    [ApiController]
    public class FeatureFlagsController : Controller
    {
        private readonly IFeatureFlagsService _featureFlagsService;

        public FeatureFlagsController(IFeatureFlagsService featureFlagsService)
        {
            _featureFlagsService = featureFlagsService;
        }

        /// <summary>
        /// returns the FeatureToggles
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [OpenApiOperation("GetFeatureToggles")]
        [ProducesResponseType(typeof(FeatureToggleConfiguration), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public ActionResult<FeatureToggleConfiguration> GetFeatureToggles()
        {
            return _featureFlagsService.GetFeatureFlags();
        }
    }
}