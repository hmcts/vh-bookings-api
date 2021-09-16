using Autofac.Extras.Moq;
using BookingsApi.Common.Configuration;
using BookingsApi.Services;
using FluentAssertions;
using Microsoft.Extensions.Options;
using NUnit.Framework;

namespace BookingsApi.UnitTests.Services
{
    public class FeatureFlagsServiceTests
    {
        private AutoMock _mocker;

        [Test]
        public void GetFeatureToggles_Should_Return_All_Feature_Toggles()
        {
            _mocker = AutoMock.GetLoose();
            _mocker.Mock<IOptions<FeatureToggleConfiguration>>().Setup(opt => opt.Value).Returns(new FeatureToggleConfiguration()
            {
                StaffMemberFeature = true,
                EJudFeature = false
            });

            var service = _mocker.Create<FeatureFlagsService>();
            var result = service.GetFeatureFlags();

            result.StaffMemberFeature.Should().BeTrue();
            result.EJudFeature.Should().BeFalse();
        }
    }
}
