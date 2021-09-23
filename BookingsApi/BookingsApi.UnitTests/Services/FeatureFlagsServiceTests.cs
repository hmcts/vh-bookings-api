using Autofac.Extras.Moq;
using BookingsApi.Common.Exceptions;
using BookingsApi.Common.Services;
using BookingsApi.Contract.Configuration;
using FluentAssertions;
using Microsoft.Extensions.Options;
using NUnit.Framework;

namespace BookingsApi.UnitTests.Services
{
    public class FeatureFlagsServiceTests
    {
        private AutoMock _mocker;
        private FeatureFlagService _service;

        [SetUp]
        public void SetUp()
        {
            _mocker = AutoMock.GetLoose();
            _mocker.Mock<IOptions<FeatureFlagConfiguration>>().Setup(opt => opt.Value).Returns(new FeatureFlagConfiguration()
            {
                StaffMemberFeature = true,
                EJudFeature = false
            });

            _service = _mocker.Create<FeatureFlagService>();
        }

        [Test]
        public void GetFeatureFlag_Should_Return_True_for_StaffMember_Feature()
        {
            var staffMemberFeature = _service.GetFeatureFlag(nameof(FeatureFlags.StaffMemberFeature));

            staffMemberFeature.Should().BeTrue();
        }

        [Test]
        public void GetFeatureFlag_Should_Return_True_for_EJud_Feature()
        {
            var staffMemberFeature = _service.GetFeatureFlag(nameof(FeatureFlags.EJudFeature));

            staffMemberFeature.Should().BeFalse();
        }

        [Test]
        public void GetFeatureFlag_Should_Throw_FeatureFlagNotFoundException()
        {
            Assert.Throws<FeatureFlagNotFoundException>(() => _service.GetFeatureFlag("TestFeature"));
        }
    }
}
