using Bookings.Api.Contract.Responses;
using Bookings.API.Utilities;
using Bookings.Domain;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Testing.Common.Builders.Domain;

namespace Bookings.UnitTests.Utilities
{
    public class PaginationCursorBasedBuilderTest
    {
        private const string HearingTypesUrl = "/hearings/types";

        [Test]
        public void Should_return_next_cursor_zero_if_no_results_exist()
        {
            var items = GetQueryableItems();
            var caseTypes = new List<int> { 1, 2 };
            var response = GetBuilder1()
                .ResourceUrl(HearingTypesUrl)
                .WithSourceItems(items)
                .Cursor("0")
                .CaseTypes(caseTypes)
                .Build();
            var expectedUrl = "/hearings/types?types=1&types=2&cursor=0&limit=100";

            response.NextCursor.Should().Be("0");
            response.Limit.Should().Be(100);
            response.NextPageUrl.Should().Be(expectedUrl);
            response.PrevPageUrl.Should().Be(expectedUrl);
        }

        [Test]
        public void Should_return_response_with_next_cursor()
        {
            var items = GetHearings();
            var caseTypes = new List<int> { 1, 2 };
            var expectedPreviousUrl1 = "/hearings/types?types=1&types=2&cursor=0&limit=2";
            var response = GetBuilder()
                .ResourceUrl(HearingTypesUrl)
                .Limit(2)
                .WithSourceItems(items)
                .Cursor("0")
                .CaseTypes(caseTypes)
                .Build();

            response.Limit.Should().Be(2);
            response.NextCursor.Should().NotBeNullOrEmpty();
            response.PrevPageUrl.Should().Be(expectedPreviousUrl1);
        }
        [Test]
        public void Should_throw_exception_if_limit_is_not_set()
        {
            var items = GetHearings();
            var caseTypes = new List<int> { 1, 2 };
            Assert.Throws<ArgumentException>(() => GetBuilder()
                .ResourceUrl(HearingTypesUrl)
                .Limit(0)
                .WithSourceItems(items)
                .Cursor("0")
                .CaseTypes(caseTypes)
                .Build());
        }

        private PaginationCursorBasedBuilder<StubPagedCursorBasedResponse, VideoHearing> GetBuilder()
        {
            var itemsR = GetHearings();
            var nextCursorExpected = itemsR.ToList()[1].Id.ToString();

            return new PaginationCursorBasedBuilder<StubPagedCursorBasedResponse, VideoHearing>(items => new StubPagedCursorBasedResponse { Items = items.AsQueryable(), NextCursor = nextCursorExpected });
        }

        private PaginationCursorBasedBuilder<StubPagedCursorBasedResponse, VideoHearing> GetBuilder1()
        {
            return new PaginationCursorBasedBuilder<StubPagedCursorBasedResponse, VideoHearing>(items => new StubPagedCursorBasedResponse { Items = items.AsQueryable(), NextCursor = "0" });
        }

        public class StubPagedCursorBasedResponse : PagedCursorBasedResponse
        {
            public IQueryable<VideoHearing> Items { get; set; }
        }

        private IQueryable<VideoHearing> GetHearings()
        {
            var hearing1 = new VideoHearingBuilder().Build();
            var hearing2 = new VideoHearingBuilder().Build();
            var hearing3 = new VideoHearingBuilder().Build();
            var hearing4 = new VideoHearingBuilder().Build();
            var hearing5 = new VideoHearingBuilder().Build();
            var videoHearings = new List<VideoHearing> { hearing1, hearing2, hearing3, hearing4, hearing5 };
            return videoHearings.AsQueryable();
        }

        private IQueryable<VideoHearing> GetQueryableItems(params VideoHearing[] items)
        {
            return items.AsQueryable();
        }
    }
}


