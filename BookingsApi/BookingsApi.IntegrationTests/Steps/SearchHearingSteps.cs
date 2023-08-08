using System.Linq;
using System.Net.Http;
using AcceptanceTests.Common.Api.Helpers;
using BookingsApi.Contract.V1.Queries;
using BookingsApi.Contract.V1.Responses;
using BookingsApi.Domain;
using BookingsApi.IntegrationTests.Contexts;
using TechTalk.SpecFlow;
using static Testing.Common.Builders.Api.ApiUriFactory.HearingsEndpoints;
using TestContext = BookingsApi.IntegrationTests.Contexts.TestContext;

namespace BookingsApi.IntegrationTests.Steps
{
    [Binding]
    public sealed class SearchHearingSteps : BaseSteps
    {
        public SearchHearingSteps(TestContext testContext) : base(testContext)
        {
        }
        
        [Given(@"I have a search hearings by case number request")]
        public void GivenIHaveASearchHearingsByCaseNumberRequest()
        {
            var seededHearing = Context.TestData.SeededHearing;
            var caseData = seededHearing.HearingCases.First();
            var caseNumber = caseData.Case.Number;

            var query =  new SearchForHearingsQuery
            {
                CaseNumber = caseNumber
            };

            Context.Uri = SearchForHearings(query);
            Context.HttpMethod = HttpMethod.Get;
        }
        
        [Given(@"I have a search hearings by date request")]
        public void GivenIHaveASearchHearingsByDateRequest()
        {
            var seededHearing = Context.TestData.SeededHearing;

            var query =  new SearchForHearingsQuery
            {
                Date = seededHearing.ScheduledDateTime
            };

            Context.Uri = SearchForHearings(query);
            Context.HttpMethod = HttpMethod.Get;
        }
        
        [Given(@"I have a search hearings by partial case number and date request")]
        public void GivenIHaveASearchHearingsByCaseNumberDateRequest()
        {
            var seededHearing = Context.TestData.SeededHearing;
            var caseData = seededHearing.HearingCases.First();
            var caseNumber = caseData.Case.Number;
            var query =  new SearchForHearingsQuery
            {
                CaseNumber = caseNumber.Substring(0,3),
                Date = seededHearing.ScheduledDateTime
            };

            Context.Uri = SearchForHearings(query);
            Context.HttpMethod = HttpMethod.Get;
        }
        
        [Given(@"I have a search hearings by partial case number and the wrong date request")]
        public void GivenIHaveASearchHearingsByCaseNumberWrongDateRequest()
        {
            var seededHearing = Context.TestData.SeededHearing;
            var caseData = seededHearing.HearingCases.First();
            var caseNumber = caseData.Case.Number;
            var query =  new SearchForHearingsQuery
            {
                CaseNumber = caseNumber.Substring(0,3),
                Date = seededHearing.ScheduledDateTime.AddDays(1)
            };

            Context.Uri = SearchForHearings(query);
            Context.HttpMethod = HttpMethod.Get;
        }
        
        [Given(@"I have a search hearings by incorrect case number but the correct date request")]
        public void GivenIHaveASearchHearingsByWrongCaseNumberRightDateRequest()
        {
            var seededHearing = Context.TestData.SeededHearing;
            var caseData = seededHearing.HearingCases.First();
            var caseNumber = caseData.Case.Number;
            var query =  new SearchForHearingsQuery
            {
                CaseNumber = caseNumber + "0237ehd",
                Date = seededHearing.ScheduledDateTime
            };

            Context.Uri = SearchForHearings(query);
            Context.HttpMethod = HttpMethod.Get;
        }
        
        [Then(@"hearing search response should contain the given hearing")]
        public async Task ThenHearingSearchResponseShouldContainTheGivenHearing()
        {
            var json = await Context.Response.Content.ReadAsStringAsync();
            var response = RequestHelper.Deserialise<List<AudioRecordedHearingsBySearchResponse>>(json);
            
            var seededHearing = Context.TestData.SeededHearing;
            response.Should().NotBeNullOrEmpty();
            var hearing = response.SingleOrDefault(x => x.Id == seededHearing.Id);
            hearing.Should().NotBeNull();
            AssertHearingByCaseNumberResponse(hearing, seededHearing);
        }
        
        [Then(@"hearing search response should be empty")]
        public async Task ThenHearingSearchResponseShouldBeEmpty()
        {
            var json = await Context.Response.Content.ReadAsStringAsync();
            var response = RequestHelper.Deserialise<List<AudioRecordedHearingsBySearchResponse>>(json);
            response.Should().BeEmpty();
        }
        
        private void AssertHearingByCaseNumberResponse(AudioRecordedHearingsBySearchResponse model, Hearing seededHearing)
        {
            var @case = seededHearing.GetCases()[0];
            model.CaseNumber.Should().Be(@case.Number);
            model.CaseName.Should().Be(@case.Name);
            model.HearingVenueName.Should().Be(seededHearing.HearingVenueName);
            model.ScheduledDateTime.Should().Be(seededHearing.ScheduledDateTime);
        }
    }
}