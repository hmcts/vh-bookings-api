using System;
using System.Net.Http;
using Bookings.Api.Contract.Requests;
using Bookings.DAL;
using Bookings.IntegrationTests.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;

namespace Bookings.IntegrationTests.Contexts
{
    public class ApiTestContext
    {
        public DbContextOptions<BookingsDbContext> BookingsDbContextOptions { get; set; }
        public TestDataManager TestDataManager { get; set; }
        public TestServer Server { get; set; }
        public string DbString { get; set; }
        public string BearerToken { get; set; }
        public string Uri { get; set; }
        public UpdateHearingRequest UpdateHearingRequest { get; set; }
        public HttpMethod HttpMethod { get; set; }
        public StringContent StringContent { get; set; }
        public HttpContent HttpContent { get; set; }
        public Guid NewHearingId { get; set; }
        public HttpResponseMessage ResponseMessage { get; set; }
    }
}