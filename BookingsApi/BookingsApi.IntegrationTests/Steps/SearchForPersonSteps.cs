using System;	
using System.Linq;	
using System.Net.Http;	
using TechTalk.SpecFlow;	
using static Testing.Common.Builders.Api.ApiUriFactory.PersonEndpoints;

namespace BookingsApi.IntegrationTests.Steps
{
    [Binding]	
    public class SearchForPersonsSteps : BaseSteps	
    {	
        public SearchForPersonsSteps(Contexts.TestContext apiTestContext) : base(apiTestContext)	
        {	
        }	

        [Given(@"I have a search for a individual request for a judge")]	
        public void GivenIHaveASearchForAIndividualRequestForAJudge()	
        {	
            var hearing = Context.TestData.SeededHearing;	
            var participant = hearing.GetParticipants().First(x => x.HearingRole.UserRole.IsJudge);	
            SetupRequest(participant.Person.ContactEmail);	
        }	

        [Given(@"I have a search for a individual request for an individual")]	
        public void GivenIHaveASearchForAIndividualRequestForAnIndividual()	
        {	
            var hearing = Context.TestData.SeededHearing;	
            var participant = hearing.GetParticipants().First(x => x.HearingRole.UserRole.IsIndividual);	
            SetupRequest(participant.Person.ContactEmail);	
        }	

        [Given(@"I have a search for a individual request for a non existent person")]	
        public void GivenIHaveASearchForAIndividualRequestForANonExistentPerson()	
        {	
            SetupRequest($"do_not_exist_{DateTime.Now.Ticks.ToString()}@hmcts.net");	
        }	

        [Given(@"I have a search for a individual with an empty query request")]	
        public void GivenIHaveASearchForAIndividualWithAnEmptyQueryRequest()	
        {	
            SetupRequest(string.Empty);	
        }	

        private void SetupRequest(string contactEmail)	
        {	
            Context.Uri = SearchForNonJudicialPersonsByContactEmail(contactEmail);	
            Context.HttpMethod = HttpMethod.Get;	
        }	
    }
}