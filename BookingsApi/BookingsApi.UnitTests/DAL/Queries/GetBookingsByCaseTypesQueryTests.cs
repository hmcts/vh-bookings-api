using System;
using BookingsApi.Common.Services;
using BookingsApi.DAL;
using BookingsApi.DAL.Queries;
using BookingsApi.UnitTests.Utilities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
// ReSharper disable ObjectCreationAsStatement

namespace BookingsApi.UnitTests.DAL.Queries
{
    public class GetBookingsByCaseTypesQueryTests : TestBase
    {
        private Mock<IFeatureToggles> FeatureTogglesMock;
        
        [Test]
        public void Should_throw_exception_if_setting_an_invalid_limit()
        {
            When(() => new GetBookingsByCaseTypesQuery {Limit = 0})
                .Should().Throw<ArgumentException>();
            
            When(() => new GetBookingsByCaseTypesQuery {Limit = -1})
                .Should().Throw<ArgumentException>();
        }
        
        [Test]
        public void Should_create_handler()
        {
            FeatureTogglesMock = new Mock<IFeatureToggles>();

            FeatureTogglesMock.Setup(r => r.AdminSearchToggle()).Returns(false);
            var connectionstring = "Connection string";

            var optionsBuilder = new DbContextOptionsBuilder<BookingsDbContext>();
            optionsBuilder.UseSqlServer(connectionstring);


            var context = new BookingsDbContext(optionsBuilder.Options);

            var handler = new GetBookingsByCaseTypesQueryHandler(context, FeatureTogglesMock.Object);
            handler.Should().NotBeNull();
        }
        
    }
}