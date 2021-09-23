using Autofac.Extras.Moq;
using BookingsApi.Common.Exceptions;
using BookingsApi.Common.Services;
using BookingsApi.Contract.Configuration;
using FluentAssertions;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookingsApi.UnitTests.Common.Services
{
    public class FeatureFlagsServiceTests
    {
        private AutoMock _mocker;
        private FeatureFlagService _service;

        [SetUp]
        public void SetUp()
        {
            _mocker = AutoMock.GetLoose();
        }

        [Test]
        public void GetFeatureFlag_Should_Return_True_for_StaffMember_Feature()
        {
            _mocker.Mock<IOptions<FeatureFlagConfiguration>>().Setup(opt => opt.Value).Returns(new FeatureFlagConfiguration()
            {
                StaffMemberFeature = true
            });
            _service = _mocker.Create<FeatureFlagService>();

            var featureFlag = _service.GetFeatureFlag(nameof(FeatureFlags.StaffMemberFeature));

            featureFlag.Should().BeTrue();
        }

        [Test]
        public void GetFeatureFlag_Should_Return_False_for_StaffMember_Feature()
        {
            _mocker.Mock<IOptions<FeatureFlagConfiguration>>().Setup(opt => opt.Value).Returns(new FeatureFlagConfiguration()
            {
                StaffMemberFeature = false
            });
            _service = _mocker.Create<FeatureFlagService>();

            var featureFlag = _service.GetFeatureFlag(nameof(FeatureFlags.StaffMemberFeature));

            featureFlag.Should().BeFalse();
        }

        [Test]
        public void GetFeatureFlag_Should_Return_True_for_EJud_Feature()
        {
            _mocker.Mock<IOptions<FeatureFlagConfiguration>>().Setup(opt => opt.Value).Returns(new FeatureFlagConfiguration()
            {
                EJudFeature = true
            });
            _service = _mocker.Create<FeatureFlagService>();

            var featureFlag = _service.GetFeatureFlag(nameof(FeatureFlags.EJudFeature));

            featureFlag.Should().BeTrue();
        }

        [Test]
        public void GetFeatureFlag_Should_Return_False_for_EJud_Feature()
        {
            _mocker.Mock<IOptions<FeatureFlagConfiguration>>().Setup(opt => opt.Value).Returns(new FeatureFlagConfiguration()
            {
                EJudFeature = false
            });
            _service = _mocker.Create<FeatureFlagService>();

            var featureFlag = _service.GetFeatureFlag(nameof(FeatureFlags.EJudFeature));

            featureFlag.Should().BeFalse();
        }

        [Test]
        public void GetFeatureFlag_Should_Throw_FeatureFlagNotFoundException()
        {
            _service = _mocker.Create<FeatureFlagService>();

            Assert.Throws<FeatureFlagNotFoundException>(() => _service.GetFeatureFlag("TestFeature"));
        }
    }
}
