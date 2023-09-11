using System.Text;
using AcceptanceTests.Common.Api.Helpers;
using BookingsApi.Contract.V1.Requests;
using BookingsApi.Contract.V1.Requests.Enums;
using BookingsApi.Contract.V1.Responses;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Infrastructure.Services.IntegrationEvents.Events;
using BookingsApi.Infrastructure.Services.ServiceBusQueue;
using FizzWare.NBuilder;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using TechTalk.SpecFlow;
using static Testing.Common.Builders.Api.ApiUriFactory.HearingsEndpoints;

namespace BookingsApi.IntegrationTests.Steps
{
    [Binding]
    public sealed class HearingsBaseSteps : BaseSteps
    {
        private Guid _hearingId;

        public HearingsBaseSteps(Contexts.TestContext context) : base(context)
        {
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
                        var seededHearing = await Context.TestDataManager.SeedVideoHearing();
                        TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
                        _hearingId = seededHearing.Id;
                        Context.TestData.NewHearingId = seededHearing.Id;
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

            Context.Uri = GetHearingDetailsById(_hearingId.ToString());
            Context.HttpMethod = HttpMethod.Get;
        }

        [Given(@"I have a get booking status for a given request with a valid hearing id")]
        public async Task GivenIHaveAGetBookingStatusForGivenHearingRequest()
        {
            var seededHearing = await Context.TestDataManager.SeedVideoHearing();
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            _hearingId = seededHearing.Id;
            Context.TestData.NewHearingId = seededHearing.Id;
            Context.Uri = GetHearingShellById(_hearingId);
            Context.HttpMethod = HttpMethod.Get;
        }

        [Given(@"I have an hearing older than (.*) months")]
        public async Task GivenIHaveAnHearingOlderThanMonths(int p0)
        {
            var seededHearing = await Context.TestDataManager.SeedPastHearings(DateTime.UtcNow.AddMonths(-p0));
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            _hearingId = seededHearing.Id;
            Context.TestData.NewHearingId = seededHearing.Id;
        }


        [Given(@"I have a (.*) book a new hearing request")]
        [Given(@"I have an (.*) book a new hearing request")]
        public void GivenIHaveAValidBookANewHearingRequest(Scenario scenario)
        {
            var request = BuildRequest(Context.TestData.CaseName);

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

            if (scenario == Scenario.Valid)
            {
                request.ScheduledDateTime = DateTime.Today.AddDays(1);
            }

            CreateTheNewHearingRequest(request);
        }

        [Given(@"I have a book a new hearing request with an invalid (.*)")]
        public void GivenIHaveABookANewHearingRequestWithAnInvalidHearingType(string invalidType)
        {
            var request = BuildRequest(Context.TestData.CaseName);
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

        [Given(@"I have a (.*) update hearing request")]
        [Given(@"I have an (.*) update hearing request")]
        public async Task GivenIHaveAUpdateHearingRequest(Scenario scenario)
        {
            var seededHearing = await Context.TestDataManager.SeedVideoHearing();
            _hearingId = seededHearing.Id;
            Context.TestData.NewHearingId = seededHearing.Id;
            Context.TestData.UpdateHearingRequest = BuildUpdateHearingRequestRequest(Context.TestData.CaseName,
                Context.Config.TestSettings.UsernameStem);
            
            if (scenario == Scenario.Invalid)
            {
                Context.TestData.UpdateHearingRequest.HearingVenueName = string.Empty;
                Context.TestData.UpdateHearingRequest.ScheduledDuration = 0;
                Context.TestData.UpdateHearingRequest.ScheduledDateTime = DateTime.Now.AddDays(-5);
                Context.TestData.UpdateHearingRequest.QuestionnaireNotRequired = false;
                Context.TestData.UpdateHearingRequest.AudioRecordingRequired = true;
            }

            UpdateTheHearingRequest();
        }

        [Given(@"I have a update hearing request with a nonexistent hearing id")]
        public void GivenIHaveAUpdateHearingRequestWithANonexistentHearingId()
        {
            _hearingId = Guid.NewGuid();
            Context.TestData.UpdateHearingRequest = BuildUpdateHearingRequestRequest(Context.TestData.CaseName,
                Context.Config.TestSettings.UsernameStem);
            UpdateTheHearingRequest();
        }

        [Given(@"I have a update hearing request with an invalid (.*)")]
        public async Task GivenIHaveAUpdateHearingRequestWithANonexistentHearingId(string invalidType)
        {
            var seededHearing = await Context.TestDataManager.SeedVideoHearing();
            Context.TestData.NewHearingId = seededHearing.Id;
            _hearingId = invalidType.Equals("hearing id") ? Guid.Empty : seededHearing.Id;
            Context.TestData.UpdateHearingRequest = BuildUpdateHearingRequestRequest(Context.TestData.CaseName,
                Context.Config.TestSettings.UsernameStem);
            if (invalidType.Equals("venue"))
            {
                Context.TestData.UpdateHearingRequest.HearingVenueName = "Random";
            }

            UpdateTheHearingRequest();
        }

        [Given(@"I have a (.*) remove hearing request")]
        [Given(@"I have an (.*) remove hearing request")]
        public async Task GivenIHaveAValidRemoveHearingRequest(Scenario scenario)
        {
            await SetHearingIdForGivenScenario(scenario);
            Context.Uri = RemoveHearing(_hearingId);
            Context.HttpMethod = HttpMethod.Delete;
        }

        [Given(@"I have a remove a hearing request")]
        public void GivenIHaveARemoveHearingRequest()
        {
            var seededHearing = Context.TestData.SeededHearing;
            Context.Uri = RemoveHearing(seededHearing.Id);
            Context.HttpMethod = HttpMethod.Delete;
        }

        [Given(@"I have a (.*) get hearings by username request")]
        [Given(@"I have an (.*) get hearings by username request")]
        public async Task GivenIHaveAGetHearingByUsernameRequest(Scenario scenario)
        {
            var seededHearing = await Context.TestDataManager.SeedVideoHearing();
            Context.TestData.NewHearingId = seededHearing.Id;
            _hearingId = seededHearing.Id;
            var username = scenario switch
            {
                Scenario.Valid => seededHearing.GetParticipants()[0].Person.Username,
                Scenario.Nonexistent => "madeupusername@hmcts.net",
                _ => throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null)
            };

            Context.Uri = GetHearingsByUsername(username);
            Context.HttpMethod = HttpMethod.Get;
        }

        [Given(@"I have a request to the get booked hearings endpoint")]
        public async Task GivenIHaveARequestToTheGetBookedHearingsEndpoint()
        {
            var seededHearing = await Context.TestDataManager.SeedVideoHearing();
            var request = new GetHearingRequest { Limit = 1 };
            Context.TestData.NewHearingId = seededHearing.Id;
            Context.Uri = GetHearingsByTypes;
            Context.HttpMethod = HttpMethod.Post;
            Context.HttpContent = GetHttpContent(request);
        }

        [Given(@"I have a valid hearing request")]
        public async Task GivenIHaveAValidHearingRequest()
        {
            var seededHearing = await Context.TestDataManager.SeedVideoHearing();
            Context.TestData.NewHearingId = seededHearing.Id;
            _hearingId = seededHearing.Id;
        }

        [Given(@"set the booking status to (.*)")]
        [When(@"set the booking status to (.*)")]
        public void SetTheBookingStatus(UpdateBookingStatus bookingStatus)
        {
            UpdateTheHearingStatus(bookingStatus);
        }

        [Given(@"I have an empty status in a hearing status request")]
        public async Task GivenIHaveAnEmptyStatusHearingStatusRequest()
        {
            var seededHearing = await Context.TestDataManager.SeedVideoHearing();
            Context.TestData.NewHearingId = seededHearing.Id;
            UpdateTheHearingStatus(null);
        }

        [Given(@"I have an empty username in a hearing status request")]
        public async Task GivenIHaveAnEmptyUsernameHearingStatusRequest()
        {
            var seededHearing = await Context.TestDataManager.SeedVideoHearing();
            Context.TestData.NewHearingId = seededHearing.Id;
            _hearingId = seededHearing.Id;
            UpdateTheHearingStatus(UpdateBookingStatus.Cancelled, null);
        }

        [Given(@"I have a request to the second page of booked hearings")]
        public async Task GivenIHaveARequestToTheSecondPageOfBookedHearings()
        {
            var firstHearing = await Context.TestDataManager.SeedVideoHearing();
            Context.TestData.OldHearingId = firstHearing.Id;

            var secondHearing = await Context.TestDataManager.SeedVideoHearing();
            Context.TestData.NewHearingId = secondHearing.Id;

            var request = new GetHearingRequest { Limit = 1 };

            Context.Uri = GetHearingsByTypes;
            Context.HttpMethod = HttpMethod.Get;
            Context.HttpContent = GetHttpContent(request);

            var response = await SendRequestAsync(Context);

            var json = await response.Content.ReadAsStringAsync();
            var bookings = RequestHelper.Deserialise<BookingsResponse>(json);

            request.Cursor = bookings.NextCursor;
            Context.Uri = GetHearingsByTypes;
            Context.HttpMethod = HttpMethod.Post;
            Context.HttpContent = GetHttpContent(request);
        }

        [Given(@"I have a request to the get booked hearings endpoint with a limit of one")]
        public async Task GivenIHaveARequestToTheGetBookedHearingsEndpointWithALimitOfOne()
        {
            var seededHearing = await Context.TestDataManager.SeedVideoHearing();
            Context.TestData.OldHearingId = seededHearing.Id;
            var secondSeededHearing = await Context.TestDataManager.SeedVideoHearing();

            var request = new GetHearingRequest { Limit = 1 };

            Context.TestData.NewHearingId = secondSeededHearing.Id;
            Context.Uri = GetHearingsByTypes;
            Context.HttpMethod = HttpMethod.Post;
            Context.HttpContent = GetHttpContent(request);
        }

        [Given(@"I have a request to the get booked hearings endpoint filtered on a (.*) case type")]
        [Given(@"I have a request to the get booked hearings endpoint filtered on an (.*) case type")]
        public async Task GivenIHaveARequestToTheGetBookedHearingsEndpointFilteredOnAValidCaseType(Scenario scenario)
        {
            var seededHearing = await Context.TestDataManager.SeedVideoHearing();
            Context.TestData.NewHearingId = seededHearing.Id;
            var caseType = scenario switch
            {
                Scenario.Valid => seededHearing.CaseTypeId,
                Scenario.Invalid => 99,
                _ => throw new InvalidOperationException("Unexpected type of case type: " + scenario)
            };

            var request = new GetHearingRequest { Types = new List<int> { caseType } };

            Context.HttpContent = GetHttpContent(request);
            Context.Uri = GetHearingsByTypes;
            Context.HttpMethod = HttpMethod.Post;
        }

        [Given(@"I have a (.*) hearing cancellation request")]
        public async Task GivenIHaveAHearingCancellationRequest(Scenario scenario)
        {
            await SeedHearingForScenarioAsync(scenario);
            CancelBooking();
        }

        [Given(@"I have a (.*) hearing failed confirmation request")]
        public async Task GivenIHaveAHearingFailedConfirmationRequest(Scenario scenario)
        {
            await SeedHearingForScenarioAsync(scenario);

            UpdateTheHearingStatus(UpdateBookingStatus.Failed);
        }

        [Then(@"hearing details should be retrieved")]
        public async Task ThenAHearingDetailsShouldBeRetrieved()
        {
            var json = await Context.Response.Content.ReadAsStringAsync();
            var response = RequestHelper.Deserialise<HearingDetailsResponse>(json);
            response.Should().NotBeNull();
            AssertHearingDetailsResponse(response);
            Context.TestData.NewHearingId = response.Id;
        }



        [Then(@"a list of hearing details should be retrieved")]
        public async Task ThenAListOfHearingDetailsShouldBeRetrieved()
        {
            var json = await Context.Response.Content.ReadAsStringAsync();
            var response = RequestHelper.Deserialise<List<HearingDetailsResponse>>(json);
            response.Should().NotBeNull();
            foreach (var hearingDetailsResponse in response)
            {
                AssertHearingDetailsResponse(hearingDetailsResponse);
            }
        }

        [Then(@"hearing details should be updated")]
        public async Task ThenHearingDetailsShouldBeUpdated()
        {
            var json = await Context.Response.Content.ReadAsStringAsync();
            var model = RequestHelper.Deserialise<HearingDetailsResponse>(json);

            model.ScheduledDuration.Should().Be(Context.TestData.UpdateHearingRequest.ScheduledDuration);
            model.HearingVenueName.Should().Be(Context.TestData.UpdateHearingRequest.HearingVenueName);
            model.ScheduledDateTime.Should()
                .Be(Context.TestData.UpdateHearingRequest.ScheduledDateTime.ToUniversalTime());
            model.HearingRoomName.Should().Be(Context.TestData.UpdateHearingRequest.HearingRoomName);
            model.OtherInformation.Should().Be(Context.TestData.UpdateHearingRequest.OtherInformation);
            model.QuestionnaireNotRequired.Should()
                .Be(Context.TestData.UpdateHearingRequest.QuestionnaireNotRequired.GetValueOrDefault());
            model.AudioRecordingRequired.Should()
                .Be(Context.TestData.UpdateHearingRequest.AudioRecordingRequired.GetValueOrDefault());

            var updatedCases = model.Cases;
            var caseRequest = Context.TestData.UpdateHearingRequest.Cases.FirstOrDefault();
            updatedCases[0].Name.Should().Be(caseRequest?.Name);
            updatedCases[0].Number.Should().Be(caseRequest?.Number);
        }

        [Then(@"the hearing should be removed")]
        public async Task ThenTheHearingShouldBeRemoved()
        {
            await using var db = new BookingsDbContext(BookingsDbContextOptions);
            var hearingFromDb = db.VideoHearings.AsNoTracking().SingleOrDefault(x => x.Id == _hearingId);
            hearingFromDb.Should().BeNull();
        }

        [Then(@"the response should contain a list of booked hearings")]
        public async Task ThenTheResponseShouldContainAListOfBookedHearings()
        {
            var json = await Context.Response.Content.ReadAsStringAsync();
            var model = RequestHelper.Deserialise<BookingsResponse>(json);
            model.Hearings.Count.Should().BeGreaterThan(0);

            var aHearing = model.Hearings[0].Hearings[0];
            aHearing.HearingNumber.Should().NotBeNullOrEmpty();
            aHearing.HearingName.Should().NotBeNullOrEmpty();
        }

        [Then(@"the response should contain a list of one booked hearing")]
        public async Task ThenTheResponseShouldContainAListOfOneBookedHearing()
        {
            var json = await Context.Response.Content.ReadAsStringAsync();
            var model = RequestHelper.Deserialise<BookingsResponse>(json);
            model.Hearings.Count.Should().Be(1);
        }

        [Then(@"hearing status should be (.*)")]
        public void ThenHearingDetailsShouldBeX(UpdateBookingStatus bookingStatus)
        {
            ThenHearingBookingStatusIs(GetMappedBookingStatus(bookingStatus));
        }

        [Then(@"hearing status should be unchanged")]
        public void ThenHearingDetailsShouldBeUnchanged()
        {
            ThenHearingBookingStatusIs(BookingStatus.Booked);
        }

        [Then(@"booking status should be retrieved")]
        public void ThenBookingStatusShouldBeRetrieved()
        {
            var responseValue = Context.Response.Content.ReadAsStringAsync();
            responseValue.Result.Should().Contain(BookingStatus.Booked.ToString());
        }

        [Then(@"the response should be an empty list")]
        public async Task ThenTheResponseShouldBeAnEmptyList()
        {
            var json = await Context.Response.Content.ReadAsStringAsync();
            json.Should().BeEquivalentTo("[]");
        }

        [Then(@"the service bus should have been queued with a new bookings message")]
        public async Task ThenTheServiceBusShouldHaveBeenQueuedWithAMessage()
        {
            var serviceBusQueueClient =
                (ServiceBusQueueClientFake)Context.Server.Host.Services.GetRequiredService<IServiceBusQueueClient>();
            var eventMessage = serviceBusQueueClient.ReadMessageFromQueue();
            eventMessage.Should().NotBeNull();
            eventMessage.Timestamp.Should().NotBeBefore(DateTime.Today);
            eventMessage.Timestamp.Should().NotBeAfter(DateTime.UtcNow);
            eventMessage.Id.Should().NotBeEmpty();

            var json = await Context.Response.Content.ReadAsStringAsync();
            var response = RequestHelper.Deserialise<HearingDetailsResponse>(json);

            var hearingReadyForVideoEvent = eventMessage.IntegrationEvent.As<HearingIsReadyForVideoIntegrationEvent>();
            hearingReadyForVideoEvent.Hearing.HearingId.Should().Be(response.Id);
            hearingReadyForVideoEvent.Hearing.GroupId.Should().Be(response.GroupId.GetValueOrDefault());
            hearingReadyForVideoEvent.Hearing.CaseName.Should().Be(response.Cases[0].Name);
            hearingReadyForVideoEvent.Hearing.CaseNumber.Should().Be(response.Cases[0].Number);
            hearingReadyForVideoEvent.Hearing.CaseType.Should().Be(response.CaseTypeName);
            hearingReadyForVideoEvent.Hearing.ScheduledDuration.Should().Be(response.ScheduledDuration);
            hearingReadyForVideoEvent.Hearing.ScheduledDateTime.Should().Be(response.ScheduledDateTime);
            hearingReadyForVideoEvent.Hearing.HearingVenueName.Should().Be(response.HearingVenueName);
        }

        private static StringContent GetHttpContent<T>(T data) where T : class
            => new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");

        private async Task SeedHearingForScenarioAsync(Scenario scenario)
        {
            var seededHearing = await Context.TestDataManager.SeedVideoHearing();
            Context.TestData.NewHearingId = seededHearing.Id;
            switch (scenario)
            {
                case Scenario.Valid:
                    TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
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
        }

        private void ThenHearingBookingStatusIs(BookingStatus status)
        {
            Hearing hearingFromDb;
            using (var db = new BookingsDbContext(Context.BookingsDbContextOptions))
            {
                hearingFromDb = db.VideoHearings.AsNoTracking().Single(x => x.Id == Context.TestData.NewHearingId);
            }

            hearingFromDb.Should().NotBeNull();

            hearingFromDb.Status.Should().Be(status);
        }

        private static BookingStatus GetMappedBookingStatus(UpdateBookingStatus status)
        {
            return status switch
            {
                UpdateBookingStatus.Created => BookingStatus.Created,
                UpdateBookingStatus.Cancelled => BookingStatus.Cancelled,
                UpdateBookingStatus.Failed => BookingStatus.Failed,
                _ => throw new ArgumentException("Invalid booking status type")
            };
        }

        private void AssertHearingDetailsResponse(HearingDetailsResponse model)
        {
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
            model.CreatedBy.Should().NotBeNullOrEmpty();
            model.QuestionnaireNotRequired.Should().BeFalse();
            model.AudioRecordingRequired.Should().BeTrue();
            model.Endpoints.Should().NotBeNullOrEmpty();
            foreach (var endpointResponse in model.Endpoints)
            {
                endpointResponse.Id.Should().NotBeEmpty();
                endpointResponse.DisplayName.Should().NotBeNullOrEmpty();
                endpointResponse.Sip.Should().NotBeNullOrEmpty();
                endpointResponse.Pin.Should().NotBeNullOrEmpty();
            }

            Hearing hearingFromDb;
            using (var db = new BookingsDbContext(Context.BookingsDbContextOptions))
            {
                hearingFromDb = db.VideoHearings.AsNoTracking().SingleOrDefault(x => x.Id == _hearingId);
            }

            hearingFromDb.Should().NotBeNull();
        }

        [TearDown]
        public async Task TearDown()
        {
            await TestDataManager.ClearSeededHearings();
        }

        private static BookNewHearingRequest BuildRequest(string caseName)
        {
            var participants = Builder<ParticipantRequest>.CreateListOfSize(5).All()
                .With(x => x.ContactEmail = $"Automation_{Faker.RandomNumber.Next()}@hmcts.net")
                .With(x => x.Username = $"Automation_{Faker.RandomNumber.Next()}@hmcts.net")
                .With(x => x.FirstName = $"Automation_{Faker.Name.First()}")
                .With(x => x.LastName = $"Automation_{Faker.Name.Last()}")
                .With(x => x.OrganisationName = "Automation Organisation")
                .With(x => x.TelephoneNumber = "01234567890")
                .Build().ToList();
            participants[0].CaseRoleName = "Applicant";
            participants[0].HearingRoleName = "Litigant in person";

            participants[1].CaseRoleName = "Applicant";
            participants[1].HearingRoleName = "Representative";

            participants[2].CaseRoleName = "Respondent";
            participants[2].HearingRoleName = "Litigant in person";

            participants[3].CaseRoleName = "Respondent";
            participants[3].HearingRoleName = "Representative";

            participants[4].CaseRoleName = "Judge";
            participants[4].HearingRoleName = "Judge";
            var cases = Builder<CaseRequest>.CreateListOfSize(1).Build().ToList();
            cases[0].IsLeadCase = true;
            cases[0].Name = $"{caseName} {Faker.RandomNumber.Next(0, 9999999)}";
            cases[0].Number = $"{Faker.RandomNumber.Next(0, 9999)}/{Faker.RandomNumber.Next(0, 9999)}";

            const string createdBy = "UserAdmin";

            var endpoints = new List<EndpointRequest>
            {
                new EndpointRequest {DisplayName = "Cool endpoint 1"},
                new EndpointRequest
                    {DisplayName = "Cool endpoint 2", DefenceAdvocateContactEmail = participants[3].ContactEmail}
            };
            return Builder<BookNewHearingRequest>.CreateNew()
                .With(x => x.CaseTypeName = "Generic")
                .With(x => x.HearingTypeName = "Automated Test")
                .With(x => x.HearingVenueName = "Birmingham Civil and Family Justice Centre")
                .With(x => x.Participants = participants)
                .With(x => x.Cases = cases)
                .With(x => x.CreatedBy = createdBy)
                .With(x => x.AudioRecordingRequired = true)
                .With(x => x.QuestionnaireNotRequired = false)
                .With(x => x.Endpoints = endpoints)
                .Build();
        }

        private static UpdateHearingRequest BuildUpdateHearingRequestRequest(string caseName, string usernameStem)
        {
            var caseList = new List<CaseRequest>
            {
                new CaseRequest()
                {
                    Name = $"{caseName} {Faker.RandomNumber.Next(900000, 999999)}",
                    Number = $"{Faker.RandomNumber.Next(1000, 9999)}/{Faker.RandomNumber.Next(1000, 9999)}"
                }
            };
            return new UpdateHearingRequest
            {
                ScheduledDateTime = DateTime.Today.AddDays(3).AddHours(11).AddMinutes(45),
                ScheduledDuration = 100,
                HearingVenueName = "Manchester County and Family Court",
                OtherInformation = "OtherInfo",
                QuestionnaireNotRequired = false,
                AudioRecordingRequired = true,
                HearingRoomName = "20",
                UpdatedBy = $"admin{usernameStem}",
                Cases = caseList
            };
        }

        private void CreateTheNewHearingRequest(BookNewHearingRequest request)
        {
            var jsonBody = RequestHelper.Serialise(request);
            Context.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            Context.Uri = BookNewHearing;
            Context.HttpMethod = HttpMethod.Post;
        }

        private void UpdateTheHearingRequest()
        {
            var jsonBody = RequestHelper.Serialise(Context.TestData.UpdateHearingRequest);
            Context.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            Context.Uri = UpdateHearingDetails(_hearingId);
            Context.HttpMethod = HttpMethod.Put;
        }

        private void UpdateTheHearingStatus(UpdateBookingStatus? status, string updatedBy = "testuser")
        {
            var jsonBody = RequestHelper.Serialise(new UpdateBookingStatusRequest
            {
                Status = status.GetValueOrDefault(),
                UpdatedBy = updatedBy,
                CancelReason = "cancelled due to covid-19"
            });
            Context.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            Context.Uri = UpdateHearingDetails(_hearingId);
            Context.HttpMethod = HttpMethod.Patch;
        }

        private void CancelBooking(string updatedBy = "testuser")
        {
            var jsonBody = RequestHelper.Serialise(new CancelBookingRequest
            {
                UpdatedBy = updatedBy,
                CancelReason = "cancelled due to covid-19"
            });
            Context.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            Context.Uri = CancelBookingUri(_hearingId);
            Context.HttpMethod = HttpMethod.Patch;
        }

        private async Task SetHearingIdForGivenScenario(Scenario scenario)
        {
            switch (scenario)
            {
                case Scenario.Valid:
                    {
                        var seededHearing = await Context.TestDataManager.SeedVideoHearing();
                        TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
                        _hearingId = seededHearing.Id;
                        Context.TestData.NewHearingId = seededHearing.Id;
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
        }
    }
}
