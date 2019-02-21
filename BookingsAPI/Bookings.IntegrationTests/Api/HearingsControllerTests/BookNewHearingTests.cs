using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Bookings.Api.Contract.Requests;
using Bookings.Api.Contract.Responses;
using Bookings.DAL;
using Bookings.Domain;
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Testing.Common.Builders.Api;

namespace Bookings.IntegrationTests.Api.HearingsControllerTests
{
    public class BookNewHearingTests : ControllerTestsBase
    {
        private readonly HearingsEndpoints _endpoints = new ApiUriFactory().HearingsEndpoints;
        private Guid _newHearingId;
        private Guid _secondHearingId;

        [SetUp]
        public void Setup()
        {
            _newHearingId = Guid.Empty;
            _secondHearingId = Guid.Empty;
        }
        
        [Test]
        public async Task should_return_bad_request_status_code_when_request_is_invalid()
        {
            var request = BuildRequest();
            request.Cases = new List<CaseRequest>();
            request.Participants = new List<ParticipantRequest>();
            request.ScheduledDuration = -100;
            request.CaseTypeName = string.Empty;
            request.HearingTypeName = string.Empty;
            request.HearingVenueName = string.Empty;
            request.ScheduledDateTime = DateTime.Today.AddDays(-5);
            
            var jsonBody = ApiRequestHelper.SerialiseRequestToSnakeCaseJson(request);
            var httpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            
            var uri = _endpoints.BookNewHearing();
            var response = await SendPostRequestAsync(uri, httpContent);
            TestContext.WriteLine($"Status Code: {response.StatusCode}");
            
            response.IsSuccessStatusCode.Should().BeFalse();
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            
            var badRequestJson = await response.Content.ReadAsStringAsync();
            var badRequestObject = JObject.Parse(badRequestJson);
            badRequestObject["Cases"].Should().NotBeEmpty();
            badRequestObject["Participants"].Should().NotBeEmpty();
            badRequestObject["ScheduledDuration"].Should().NotBeEmpty();
            badRequestObject["CaseTypeName"].Should().NotBeEmpty();
            badRequestObject["HearingTypeName"].Should().NotBeEmpty();
            badRequestObject["HearingVenueName"].Should().NotBeEmpty();
        }
        
        [Test]
        public async Task should_return_bad_request_status_code_when_case_type_is_invalid()
        {
            var request = BuildRequest();
            request.CaseTypeName = "Random";
            
            var jsonBody = ApiRequestHelper.SerialiseRequestToSnakeCaseJson(request);
            var httpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            
            var uri = _endpoints.BookNewHearing();
            var response = await SendPostRequestAsync(uri, httpContent);
            TestContext.WriteLine($"Status Code: {response.StatusCode}");
            
            response.IsSuccessStatusCode.Should().BeFalse();
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            
            var badRequestJson = await response.Content.ReadAsStringAsync();
            var badRequestObject = JObject.Parse(badRequestJson);
            badRequestObject["CaseTypeName"].Should().NotBeEmpty();
        }
        
        [Test]
        public async Task should_return_bad_request_status_code_when_hearing_type_is_invalid()
        {
            var request = BuildRequest();
            request.HearingTypeName = "Random";
            
            var jsonBody = ApiRequestHelper.SerialiseRequestToSnakeCaseJson(request);
            var httpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            
            var uri = _endpoints.BookNewHearing();
            var response = await SendPostRequestAsync(uri, httpContent);
            TestContext.WriteLine($"Status Code: {response.StatusCode}");
            
            response.IsSuccessStatusCode.Should().BeFalse();
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            
            var badRequestJson = await response.Content.ReadAsStringAsync();
            var badRequestObject = JObject.Parse(badRequestJson);
            badRequestObject["HearingTypeName"].Should().NotBeEmpty();
        }
        
        [Test]
        public async Task should_return_bad_request_status_code_when_venue_name_is_invalid()
        {
            var request = BuildRequest();
            request.HearingVenueName = "Random";
            
            var jsonBody = ApiRequestHelper.SerialiseRequestToSnakeCaseJson(request);
            var httpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            
            var uri = _endpoints.BookNewHearing();
            var response = await SendPostRequestAsync(uri, httpContent);
            TestContext.WriteLine($"Status Code: {response.StatusCode}");
            
            response.IsSuccessStatusCode.Should().BeFalse();
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            
            var badRequestJson = await response.Content.ReadAsStringAsync();
            var badRequestObject = JObject.Parse(badRequestJson);
            badRequestObject["HearingVenueName"].Should().NotBeEmpty();
        }
        
        [Test]
        public async Task should_return_created_status_code_when_request_is_valid()
        {
            var request = BuildRequest();
            var jsonBody = ApiRequestHelper.SerialiseRequestToSnakeCaseJson(request);
            var httpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            
            var uri = _endpoints.BookNewHearing();
            var response = await SendPostRequestAsync(uri, httpContent);
            TestContext.WriteLine($"Status Code: {response.StatusCode}");
            
            response.IsSuccessStatusCode.Should().BeTrue();
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var json = await response.Content.ReadAsStringAsync();
            var model = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<HearingDetailsResponse>(json);

            model.Should().NotBeNull();
            _newHearingId = model.Id;
            
            model.CaseTypeName.Should().NotBeNull();
            model.HearingTypeName.Should().NotBeNull();
            model.HearingVenueName.Should().NotBeNull();
            model.ScheduledDuration.Should().BeGreaterThan(0);
            model.ScheduledDateTime.Should().BeAfter(DateTime.MinValue);
            
            model.Cases.Should().NotBeEmpty();
            foreach (var caseResponse in model.Cases)
            {
                caseResponse.Name.Should().NotBeEmpty();
                caseResponse.Number.Should().NotBeEmpty();
            }
            
            model.Participants.Should().NotBeEmpty();
            foreach (var participant in model.Participants)
            {
                participant.Id.Should().NotBeEmpty();
                
                participant.Title.Should().NotBeEmpty();
                participant.FirstName.Should().NotBeEmpty();
                participant.LastName.Should().NotBeEmpty();
                participant.MiddleNames.Should().NotBeEmpty();
                
                participant.DisplayName.Should().NotBeEmpty();
                participant.Username.Should().NotBeEmpty();
                participant.ContactEmail.Should().NotBeEmpty();
                participant.TelephoneNumber.Should().NotBeEmpty();
                
                participant.CaseRoleName.Should().NotBeEmpty();
                participant.HearingRoleName.Should().NotBeEmpty();
                participant.UserRoleName.Should().NotBeEmpty();
            }

            Hearing hearingFromDb;
            using (var db = new BookingsDbContext(BookingsDbContextOptions))
            {
                hearingFromDb = db.VideoHearings.AsNoTracking().SingleOrDefault(x => x.Id == _newHearingId);
            }

            hearingFromDb.Should().NotBeNull();
        }

        [Test]
        public async Task should_use_existing_persons_when_booking_into_a_new_hearing()
        {
            int personCountBefore;
            int personCountAfter;
            
            var request = BuildRequest();
            var jsonBody = ApiRequestHelper.SerialiseRequestToSnakeCaseJson(request);
            var httpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            
            var uri = _endpoints.BookNewHearing();
            var response = await SendPostRequestAsync(uri, httpContent);
            TestContext.WriteLine($"Status Code: {response.StatusCode}");
            
            response.IsSuccessStatusCode.Should().BeTrue();
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            
            var json = await response.Content.ReadAsStringAsync();
            var model = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<HearingDetailsResponse>(json);

            model.Should().NotBeNull();
            _newHearingId = model.Id;
            
            using (var db = new BookingsDbContext(BookingsDbContextOptions))
            {
                personCountBefore = await db.Persons.CountAsync();
            }
            
            var response2 = await SendPostRequestAsync(uri, httpContent);
            TestContext.WriteLine($"Status Code: {response2.StatusCode}");
            
            response2.IsSuccessStatusCode.Should().BeTrue();
            response2.StatusCode.Should().Be(HttpStatusCode.Created);
            
            var secondJson = await response.Content.ReadAsStringAsync();
            var secondModel = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<HearingDetailsResponse>(secondJson);

            secondModel.Should().NotBeNull();
            _secondHearingId = secondModel.Id;
            
            using (var db = new BookingsDbContext(BookingsDbContextOptions))
            {
                personCountAfter = await db.Persons.CountAsync();
            }

            personCountAfter.Should().Be(personCountBefore);
        }

        [TearDown]
        public async Task TearDown()
        {
            if (_newHearingId != Guid.Empty)
            {
                TestContext.WriteLine($"Removing test hearing {_newHearingId}");
                await Hooks.RemoveVideoHearing(_newHearingId);
            }
            
            if (_secondHearingId != Guid.Empty)
            {
                TestContext.WriteLine($"Removing test hearing {_secondHearingId}");
                await Hooks.RemoveVideoHearing(_secondHearingId);
            }
        }
        
        private BookNewHearingRequest BuildRequest()
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
    }
}