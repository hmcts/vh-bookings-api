using BookingsApi.Contract.V1.Configuration;
using BookingsApi.Controllers.V1;
using BookingsApi.Services;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace BookingsApi.UnitTests.Controllers.V1
{
    public class FeatureFlagsControllerTests
    {
        private Mock<IFeatureFlagService> _featureFlagsService;
        private FeatureFlagsController _controller;

        [SetUp]
        public void SetUp()
        {
            _featureFlagsService = new Mock<IFeatureFlagService>();
            _controller = new FeatureFlagsController(_featureFlagsService.Object);
        }

        [Test]
        public void GetFeatureFlag_Should_Return_True_For_StaffMemberFeature()
        {
            _featureFlagsService.Setup(p => p.GetFeatureFlag(It.Is<string>(p => p == nameof(FeatureFlags.StaffMemberFeature)))).Returns(true);

            var result = _controller.GetFeatureFlag(nameof(FeatureFlags.StaffMemberFeature));

            result.Value.Should().BeTrue();
        }
        
        [Test]
        public void GetFeatureFlag_Should_Return_False_For_EjudFeature()
        {
            _featureFlagsService.Setup(p => p.GetFeatureFlag(It.Is<string>(p => p == nameof(FeatureFlags.EJudFeature)))).Returns(false); 

            var result = _controller.GetFeatureFlag(nameof(FeatureFlags.EJudFeature));

            result.Value.Should().BeFalse();
        }
    }
}
