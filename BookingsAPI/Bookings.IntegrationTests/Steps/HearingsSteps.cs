using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Bookings.Api.Contract.Requests;
using Bookings.Api.Contract.Responses;
using Bookings.DAL;
using Bookings.Domain;
using Bookings.IntegrationTests.Contexts;
using Bookings.IntegrationTests.Helper;
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using TechTalk.SpecFlow;
using Testing.Common.Builders.Api;

namespace Bookings.IntegrationTests.Steps
{
    [Binding]
    public sealed class HearingsSteps : StepsBase
    {
        private readonly Contexts.TestContext _apiTestContext;
        private readonly HearingsEndpoints _endpoints = new ApiUriFactory().HearingsEndpoints;
        private Guid _hearingId;

        public HearingsSteps(Contexts.TestContext apiTestContext)
        {
            _apiTestContext = apiTestContext;
            _hearingId = Guid.Empty;
        }

        [Given(@"I have a get details for a given hearing request with a (.*) hearing id")]
        [Given(@"I have a get details for a given hearing request with an (.*) hearing id")]
        public async Task GivenIHaveAGetDetailsForGivenHearingRequest(Scenario scenario)
        {
            switch (scenario)
            {
                case Scenario.Valid:
                {
                    var seededHearing = await _apiTestContext.TestDataManager.SeedVideoHearing();
                        NUnit.Framework.TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
                    _hearingId = seededHearing.Id;
                    break;
                }
                case Scenario.Nonexistent:
                    _hearingId = Guid.NewGuid();
                    break;
                case Scenario.Invalid:
                    _hearingId = Guid.Empty;
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null);
            }
            _apiTestContext.Uri = _endpoints.GetHearingDetailsById(_hearingId);
            _apiTestContext.HttpMethod = HttpMethod.Get;
        }

        [Given(@"I have a (.*) book a new hearing request")]
        [Given(@"I have an (.*) book a new hearing request")]
        public void GivenIHaveAValidBookANewHearingRequest(Scenario scenario)
        {
            var request = BuildRequest();
            if (scenario == Scenario.Invalid)
            {
                request.Cases = new List<CaseRequest>();
                request.Participants = new List<ParticipantRequest>();
                request.ScheduledDuration = -100;
                request.CaseTypeName = string.Empty;
                request.HearingTypeName = string.Empty;
                request.HearingVenueName = string.Empty;
                request.ScheduledDateTime = DateTime.Today.AddDays(-5);
            }
            CreateTheNewHearingRequest(request);
        }

        [Given(@"I have a book a new hearing request with an invalid (.*)")]
        public void GivenIHaveABookANewHearingRequestWithAnInvalidHearingType(string invalidType)
        {
            var request = BuildRequest();
            switch (invalidType)
            {
                case "case type":
                {
                    request.CaseTypeName = "Random";
                    break;
                }
                case "hearing type":
                {
                    request.HearingTypeName = "A nonexistent hearing type";
                    break;
                }
                case "venue name":
                {
                    request.HearingVenueName = "Random";
                    break;
                }
                default: throw new ArgumentOutOfRangeException(invalidType, invalidType, null);
            }
            CreateTheNewHearingRequest(request);
        }

        [Given(@"I have a update hearing request with a nonexistent hearing id")]
        public void GivenIHaveAUpdateHearingRequestWithANonexistentHearingId()
        {
            _hearingId = Guid.NewGuid();
            _apiTestContext.UpdateHearingRequest = BuildUpdateHearingRequestRequest();
            UpdateTheHearingRequest();
        }

        [Given(@"I have a (.*) update hearing request")]
        [Given(@"I have an (.*) update hearing request")]
        public async Task GivenIHaveAUpdateHearingRequest(Scenario scenario)
        {
            var seededHearing = await _apiTestContext.TestDataManager.SeedVideoHearing();
            _hearingId = seededHearing.Id;
            _apiTestContext.UpdateHearingRequest = BuildUpdateHearingRequestRequest();
            if (scenario == Scenario.Invalid)
            {
                _apiTestContext.UpdateHearingRequest.HearingVenueName = string.Empty;
                _apiTestContext.UpdateHearingRequest.ScheduledDuration = 0;
                _apiTestContext.UpdateHearingRequest.ScheduledDateTime = DateTime.Now.AddDays(-5);
            }
            UpdateTheHearingRequest();
        }

        [Given(@"I have a update hearing request with an invalid (.*)")]
        public async Task GivenIHaveAUpdateHearingRequestWithANonexistentHearingId(string invalidType)
        {
            var seededHearing = await _apiTestContext.TestDataManager.SeedVideoHearing();
            _hearingId = invalidType.Equals("hearing id") ? Guid.Empty : seededHearing.Id;
            _apiTestContext.UpdateHearingRequest = BuildUpdateHearingRequestRequest();
            if (invalidType.Equals("venue"))
            {
                _apiTestContext.UpdateHearingRequest.HearingVenueName = "Random";
            }
            UpdateTheHearingRequest();
        }

        [Given(@"I have a (.*) remove hearing request")]
        [Given(@"I have an (.*) remove hearing request")]
        public async Task GivenIHaveAValidRemoveHearingRequest(Scenario scenario)
        {
            switch (scenario)
            {
                case Scenario.Valid:
                    var seededHearing = await _apiTestContext.TestDataManager.SeedVideoHearing();
                    NUnit.Framework.TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
                    _hearingId = seededHearing.Id;
                    break;
                case Scenario.Invalid:
                    _hearingId = Guid.Empty;
                    break;
                case Scenario.Nonexistent:
                    _hearingId = Guid.NewGuid();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null);
            }
            
            _apiTestContext.Uri = _endpoints.RemoveHearing(_hearingId);
            _apiTestContext.HttpMethod = HttpMethod.Delete;
        }
    
        [Then(@"hearing details should be retrieved")]
        public async Task ThenAHearingDetailsShouldBeRetrieved()
        {
            var json = await _apiTestContext.ResponseMessage.Content.ReadAsStringAsync();
            var model = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<HearingDetailsResponse>(json);
            model.Should().NotBeNull();
            _hearingId = model.Id;

            model.CaseTypeName.Should().NotBeNullOrEmpty();
            foreach (var theCase in model.Cases)
            {
                theCase.Name.Should().NotBeNullOrEmpty();
                theCase.Number.Should().NotBeNullOrEmpty();
            }
            model.HearingTypeName.Should().NotBeNullOrEmpty();
            model.HearingVenueName.Should().NotBeNullOrEmpty();
            foreach (var participant in model.Participants)
            {
                participant.CaseRoleName.Should().NotBeNullOrEmpty();
                participant.ContactEmail.Should().NotBeNullOrEmpty();
                participant.DisplayName.Should().NotBeNullOrEmpty();
                participant.FirstName.Should().NotBeNullOrEmpty();
                participant.HearingRoleName.Should().NotBeNullOrEmpty();
                participant.Id.Should().NotBeEmpty();
                participant.LastName.Should().NotBeNullOrEmpty();
                participant.MiddleNames.Should().NotBeNullOrEmpty();
                participant.TelephoneNumber.Should().NotBeNullOrEmpty();
                participant.Title.Should().NotBeNullOrEmpty();
                participant.UserRoleName.Should().NotBeNullOrEmpty();
            }
            model.ScheduledDateTime.Should().BeAfter(DateTime.MinValue);
            model.ScheduledDuration.Should().BePositive();
            model.HearingRoomName.Should().NotBeNullOrEmpty();
            model.OtherInformation.Should().NotBeNullOrEmpty();

            Hearing hearingFromDb;
            using (var db = new BookingsDbContext(_apiTestContext.BookingsDbContextOptions))
            {
                hearingFromDb = db.VideoHearings.AsNoTracking().SingleOrDefault(x => x.Id == _hearingId);
            }
            hearingFromDb.Should().NotBeNull();
        }

        [Then(@"hearing details should be updated")]
        public async Task ThenHearingDetailsShouldBeUpdated()
        {
            var json = await _apiTestContext.ResponseMessage.Content.ReadAsStringAsync();
            var model = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<HearingDetailsResponse>(json);

            model.ScheduledDuration.Should().Be(_apiTestContext.UpdateHearingRequest.ScheduledDuration);
            model.HearingVenueName.Should().Be(_apiTestContext.UpdateHearingRequest.HearingVenueName);
            model.ScheduledDateTime.Should().Be(_apiTestContext.UpdateHearingRequest.ScheduledDateTime);
            model.HearingRoomName.Should().Be(_apiTestContext.UpdateHearingRequest.HearingRoomName);
            model.OtherInformation.Should().Be(_apiTestContext.UpdateHearingRequest.OtherInformation);
        }

        [Then(@"the hearing should be removed")]
        public void ThenTheHearingShouldBeRemoved()
        {
            Hearing hearingFromDb;
            using (var db = new BookingsDbContext(_apiTestContext.BookingsDbContextOptions))
            {
                hearingFromDb = db.VideoHearings.AsNoTracking().SingleOrDefault(x => x.Id == _hearingId);
            }
            hearingFromDb.Should().BeNull();
        }

        [TearDown]
        public async Task TearDown()
        {
            if (_hearingId != Guid.Empty)
            {
                NUnit.Framework.TestContext.WriteLine($"Removing test hearing {_hearingId}");
                await Hooks.RemoveVideoHearing(_hearingId);
            }
        }

        private static BookNewHearingRequest BuildRequest()
        {
            var participants = Builder<ParticipantRequest>.CreateListOfSize(4).All()
                .With(x => x.ContactEmail = Faker.Internet.Email())
                .With(x => x.Username = Faker.Internet.Email())
                .Build().ToList();
            participants[0].CaseRoleName = "Claimant";
            participants[0].HearingRoleName = "Claimant LIP";

            participants[1].CaseRoleName = "Claimant";
            participants[1].HearingRoleName = "Solicitor";

            participants[2].CaseRoleName = "Defendant";
            participants[2].HearingRoleName = "Defendant LIP";

            participants[3].CaseRoleName = "Defendant";
            participants[3].HearingRoleName = "Solicitor";
            var cases = Builder<CaseRequest>.CreateListOfSize(2).Build().ToList();

            return Builder<BookNewHearingRequest>.CreateNew()
                .With(x => x.CaseTypeName = "Civil Money Claims")
                .With(x => x.HearingTypeName = "Application to Set Judgment Aside")
                .With(x => x.HearingVenueName = "Birmingham Civil and Family Justice Centre")
                .With(x => x.Participants = participants)
                .With(x => x.Cases = cases)
                .Build();
        }

        private static UpdateHearingRequest BuildUpdateHearingRequestRequest()
        {
            return new UpdateHearingRequest
            {
                ScheduledDateTime = DateTime.Today.AddDays(3).AddHours(11).AddMinutes(45),
                ScheduledDuration = 100,
                HearingVenueName = "Manchester Civil and Family Justice Centre"
            };
        }

        private void CreateTheNewHearingRequest(BookNewHearingRequest request)
        {
            var jsonBody = ApiRequestHelper.SerialiseRequestToSnakeCaseJson(request);
            _apiTestContext.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            _apiTestContext.Uri = _endpoints.BookNewHearing();
            _apiTestContext.HttpMethod = HttpMethod.Post;
        }

        private void UpdateTheHearingRequest()
        {
            var jsonBody = ApiRequestHelper.SerialiseRequestToSnakeCaseJson(_apiTestContext.UpdateHearingRequest);
            _apiTestContext.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            _apiTestContext.Uri = _endpoints.UpdateHearingDetails(_hearingId);
            _apiTestContext.HttpMethod = HttpMethod.Put;
        }       
    }
}
