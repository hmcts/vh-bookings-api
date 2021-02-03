using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using BookingsApi.Domain.Enumerations;
using FluentAssertions;
using TechTalk.SpecFlow;

namespace Bookings.IntegrationTests.Steps
{
    [Binding]
    public sealed class CommonBaseSteps : BaseSteps
    {
        public CommonBaseSteps(Contexts.TestContext _context) : base(_context)
        {
        }

        [When(@"I send the request to the endpoint")]
        [When(@"I send the same request twice")]
        public async Task WhenISendTheRequestToTheEndpoint()
        {
            Context.Response = new HttpResponseMessage();
            Context.Response = Context.HttpMethod.Method switch
            {
                "GET" => await SendGetRequestAsync(Context),
                "POST" => await SendPostRequestAsync(Context),
                "PATCH" => await SendPatchRequestAsync(Context),
                "PUT" => await SendPutRequestAsync(Context),
                "DELETE" => await SendDeleteRequestAsync(Context),
                _ => throw new ArgumentOutOfRangeException(Context.HttpMethod.ToString(), Context.HttpMethod.ToString(),
                    null)
            };
        }

        [Then(@"the response should have the status (.*) and success status (.*)")]
        public async Task ThenTheResponseShouldHaveStatus(HttpStatusCode statusCode, bool isSuccess)
        {
            var json = await Context.Response.Content.ReadAsStringAsync();
            var hearingId = Context.TestData.NewHearingId;
            Context.Response.StatusCode.Should().Be(statusCode, $"hearing: {hearingId} ===== {json}");
            Context.Response.IsSuccessStatusCode.Should().Be(isSuccess, $"hearing: {hearingId} ===== {json}");
            NUnit.Framework.TestContext.WriteLine($"Status Code: {Context.Response.StatusCode}");
        }

        [Then(@"the response message should read '(.*)'")]
        [Then(@"the error response message should contain '(.*)'")]
        [Then(@"the error response message should also contain '(.*)'")]
        public void ThenTheResponseShouldContain(string errorMessage)
        {
            Context.Response.Content.ReadAsStringAsync().Result.Should().Contain(errorMessage);
        }

        [Given(@"I have a hearing")]
        public async Task GivenIHaveAHearing()
        {
            var seededHearing = await Context.TestDataManager.SeedVideoHearing();
            Context.TestData.NewHearingId = seededHearing.Id;
            Context.TestData.SeededHearing = seededHearing;
            NUnit.Framework.TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
        }
        
        [Given(@"I have a confirmed hearing")]
        public async Task GivenIHaveAConfirmedHearing()
        {
            var seededHearing = await Context.TestDataManager.SeedVideoHearing(false,BookingStatus.Created);
            Context.TestData.NewHearingId = seededHearing.Id;
            Context.TestData.SeededHearing = seededHearing;
            NUnit.Framework.TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
        }
    }
}
