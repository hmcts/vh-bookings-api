using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using BookingsApi.Common.Services;
using FluentAssertions;
using NUnit.Framework;

namespace BookingsApi.IntegrationTests.Services
{
    public class UkPublicHolidayRetrieverTests
    {
        private UkPublicHolidayRetriever _sut;
        private HttpClient _httpClient;

        [SetUp]
        public void Setup()
        {
            _httpClient = new HttpClient();
            _sut = new UkPublicHolidayRetriever(_httpClient);
        }

        [TearDown]
        public void TearDown()
        {
            _httpClient.Dispose();
        }
        
        [Test]
        public async Task should_return_upcoming_holidays()
        {
            var result = await _sut.RetrieveUpcomingHolidays();

            result.Any(x => x.Date < DateTime.Today).Should().BeFalse();
        }
    }
}