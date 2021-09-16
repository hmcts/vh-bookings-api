using BookingsApi.Common.Configuration;
using BookingsApi.Controllers;
using BookingsApi.Services;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace BookingsApi.UnitTests.Controllers
{
    public class FeatureFlagsControllerTests
    {
        private Mock<IFeatureFlagsService> _featureFlagsService;
        [Test]
        public void GetFeatureToggles_Should_Return_All_Feature_Toggles()
        {
            _featureFlagsService = new Mock<IFeatureFlagsService>();

            _featureFlagsService.Setup(p => p.GetFeatureFlags()).Returns(new FeatureToggleConfiguration()
            {
                StaffMemberFeature = true,
                EJudFeature = false
            });

            var _controller = new FeatureFlagsController(_featureFlagsService.Object);
            var result = _controller.GetFeatureToggles();

            result.Value.StaffMemberFeature.Should().BeTrue();
            result.Value.EJudFeature.Should().BeFalse();
        }
    }
}
