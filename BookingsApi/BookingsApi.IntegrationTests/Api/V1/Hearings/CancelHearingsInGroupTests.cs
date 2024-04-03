using BookingsApi.Contract.V1.Requests;
using BookingsApi.DAL.Queries;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Infrastructure.Services.IntegrationEvents.Events;
using BookingsApi.Infrastructure.Services.ServiceBusQueue;
using BookingsApi.Validations.V1;
using Testing.Common.Builders.Domain;

namespace BookingsApi.IntegrationTests.Api.V1.Hearings
{
    public class CancelHearingsInGroupTests : ApiTest
    {
        [Test]
        public async Task should_cancel_hearings_in_group()
        {
            // Arrange
            var seededHearingsInGroup = await SeedHearingsInGroup();
            var groupId = seededHearingsInGroup[0].SourceId.Value;
            var hearingToSkip = seededHearingsInGroup[0]; // For testing that only hearings in the request are cancelled
            var hearingsToCancel = seededHearingsInGroup
                .Where(h => h.Id != hearingToSkip.Id)
                .ToList();
            
            var request = BuildRequest();
            request.HearingIds = hearingsToCancel.Select(h => h.Id).ToList();
            
            // Act
            using var client = Application.CreateClient();
            var result = await client
                .PatchAsync(ApiUriFactory.HearingsEndpoints.CancelHearingsInGroupId(groupId),RequestBody.Set(request));
            
            // Assert
            result.IsSuccessStatusCode.Should().BeTrue();
            result.StatusCode.Should().Be(HttpStatusCode.NoContent, result.Content.ReadAsStringAsync().Result);
            
            await using var db = new BookingsDbContext(BookingsDbContextOptions);
            var hearingsInDb = await new GetHearingsByGroupIdQueryHandler(db).Handle(new GetHearingsByGroupIdQuery(groupId));
            
            var skippedHearingAfterUpdate = hearingsInDb.Find(h => h.Id == hearingToSkip.Id);
            skippedHearingAfterUpdate.Status.Should().Be(hearingToSkip.Status);
            skippedHearingAfterUpdate.UpdatedDate.Should().Be(hearingToSkip.UpdatedDate);
            skippedHearingAfterUpdate.UpdatedBy.Should().Be(hearingToSkip.UpdatedBy);
            skippedHearingAfterUpdate.CancelReason.Should().Be(hearingToSkip.CancelReason);
            skippedHearingAfterUpdate.Allocations.Count.Should().Be(hearingToSkip.Allocations.Count);
            
            foreach (var hearing in hearingsInDb.Where(h => h.Id != hearingToSkip.Id))
            {
                var hearingBeforeUpdate = hearingsToCancel.Find(h => h.Id == hearing.Id);
                
                hearing.Status.Should().Be(BookingStatus.Cancelled);
                hearing.UpdatedDate.Should().BeAfter(hearingBeforeUpdate.UpdatedDate);
                hearing.UpdatedBy.Should().Be(request.UpdatedBy);
                hearing.CancelReason.Should().Be(request.CancelReason);
                hearing.Allocations.Should().BeEmpty();
                
                var serviceBusStub =
                    Application.Services.GetService(typeof(IServiceBusQueueClient)) as ServiceBusQueueClientFake;
                var message = serviceBusStub!.ReadMessageFromQueue();
                message.Should().NotBeNull();
                message.IntegrationEvent.Should()
                    .BeEquivalentTo(new HearingCancelledIntegrationEvent(hearing.Id));
            }
        }
        
        [Test]
        public async Task should_return_not_found_when_no_hearings_found_for_group()
        {
            // Arrange
            var groupId = Guid.NewGuid();
            
            var request = BuildRequest();
            request.HearingIds = new List<Guid> {Guid.NewGuid()};
            
            // Act
            using var client = Application.CreateClient();
            var result = await client
                .PatchAsync(ApiUriFactory.HearingsEndpoints.CancelHearingsInGroupId(groupId),RequestBody.Set(request));
            
            // Assert
            result.IsSuccessStatusCode.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.NotFound, result.Content.ReadAsStringAsync().Result);
        }

        [Test]
        public async Task should_return_bad_request_when_hearings_in_request_do_not_belong_to_group()
        {
            // Arrange
            var seededHearingsInGroup = await SeedHearingsInGroup();
            var groupId = seededHearingsInGroup[0].SourceId.Value;
            
            var request = BuildRequest();
            request.HearingIds = seededHearingsInGroup.Select(h => h.Id).ToList();
            
            var hearingsNotInGroup = new List<Guid>
            {
                Guid.NewGuid(), 
                Guid.NewGuid() 
            };
            
            request.HearingIds.AddRange(hearingsNotInGroup);
            
            // Act
            using var client = Application.CreateClient();
            var result = await client
                .PatchAsync(ApiUriFactory.HearingsEndpoints.CancelHearingsInGroupId(groupId),RequestBody.Set(request));
            
            // Assert
            result.IsSuccessStatusCode.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
            validationProblemDetails.Errors["HearingIds[3]"][0].Should()
                .Be($"Hearing {hearingsNotInGroup[0]} does not belong to group {groupId}");
            validationProblemDetails.Errors["HearingIds[4]"][0].Should()
                .Be($"Hearing {hearingsNotInGroup[1]} does not belong to group {groupId}");
        }

        [Test]
        public async Task should_return_bad_request_when_duplicate_hearing_ids_in_request()
        {
            // Arrange
            var seededHearingsInGroup = await SeedHearingsInGroup();
            var groupId = seededHearingsInGroup[0].SourceId.Value;
            
            var request = BuildRequest();
            request.HearingIds = seededHearingsInGroup.Select(h => h.Id).ToList();
            request.HearingIds[1] = request.HearingIds[0];
            
            // Act
            using var client = Application.CreateClient();
            var result = await client
                .PatchAsync(ApiUriFactory.HearingsEndpoints.CancelHearingsInGroupId(groupId),RequestBody.Set(request));
            
            // Assert
            result.IsSuccessStatusCode.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
            validationProblemDetails.Errors[nameof(request.HearingIds)][0].Should()
                .Be(CancelHearingsInGroupRequestInputValidation.DuplicateHearingIdsMessage);
        }

        [Test]
        public async Task should_return_bad_request_when_empty_hearings_list_in_request()
        {
            // Arrange
            var groupId = Guid.NewGuid();
            
            var request = BuildRequest();
            request.HearingIds = new List<Guid>();
            
            // Act
            using var client = Application.CreateClient();
            var result = await client
                .PatchAsync(ApiUriFactory.HearingsEndpoints.CancelHearingsInGroupId(groupId),RequestBody.Set(request));
            
            // Assert
            result.IsSuccessStatusCode.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
            validationProblemDetails.Errors[nameof(request.HearingIds)][0].Should()
                .Be(CancelHearingsInGroupRequestInputValidation.NoHearingsErrorMessage);
        }

        [Test]
        public async Task should_return_bad_request_when_null_hearings_list_in_request()
        {
            // Arrange
            var groupId = Guid.NewGuid();
            
            var request = BuildRequest();
            request.HearingIds = null;
            
            // Act
            using var client = Application.CreateClient();
            var result = await client
                .PatchAsync(ApiUriFactory.HearingsEndpoints.CancelHearingsInGroupId(groupId),RequestBody.Set(request));
            
            // Assert
            result.IsSuccessStatusCode.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
            validationProblemDetails.Errors[nameof(request.HearingIds)][0].Should()
                .Be(CancelHearingsInGroupRequestInputValidation.NoHearingsErrorMessage);
        }

        [Test]
        public async Task should_return_bad_request_for_invalid_status_transition()
        {
            // Arrange
            var seededHearingsInGroup = await SeedHearingsInGroup();
            var groupId = seededHearingsInGroup[0].SourceId.Value;

            await using var db = new BookingsDbContext(BookingsDbContextOptions);

            var day1Hearing = seededHearingsInGroup[0];
            var day2Hearing = await db.VideoHearings.FirstAsync(h => h.Id == seededHearingsInGroup[1].Id);
            var day3Hearing = await db.VideoHearings.FirstAsync(h => h.Id == seededHearingsInGroup[2].Id);

            // Prompt an invalid status transition - can't cancel a hearing that is already cancelled or failed
            day2Hearing.SetProtected(nameof(day2Hearing.Status), BookingStatus.Cancelled);
            day3Hearing.SetProtected(nameof(day3Hearing.Status), BookingStatus.Failed);

            await db.SaveChangesAsync();
            
            var request = BuildRequest();
            var hearingsToCancel = new List<VideoHearing> {  day1Hearing, day2Hearing, day3Hearing};
            request.HearingIds = hearingsToCancel.Select(h => h.Id).ToList();
            
            // Act
            using var client = Application.CreateClient();
            var result = await client
                .PatchAsync(ApiUriFactory.HearingsEndpoints.CancelHearingsInGroupId(groupId),RequestBody.Set(request));
            
            // Assert
            result.IsSuccessStatusCode.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
            validationProblemDetails.Errors["requestHearings[1]"][0].Should()
                .Be($"Cannot change the booking status from {day2Hearing.Status} to {BookingStatus.Cancelled} for hearing {day2Hearing.Id}");
            validationProblemDetails.Errors["requestHearings[2]"][0].Should()
                .Be($"Cannot change the booking status from {day3Hearing.Status} to {BookingStatus.Cancelled} for hearing {day3Hearing.Id}");
            
            var hearingsInGroup = await new GetHearingsByGroupIdQueryHandler(db).Handle(new GetHearingsByGroupIdQuery(groupId));
            var hearingsInDb = hearingsInGroup.Where(h => request.HearingIds.Contains(h.Id)).ToList();

            // Verify that no updates were made
            foreach (var hearing in hearingsInDb)
            {
                var hearingBeforeUpdate = hearingsToCancel.Find(h => h.Id == hearing.Id);
                
                hearing.Status.Should().Be(hearingBeforeUpdate.Status);
                hearing.UpdatedDate.Should().Be(hearingBeforeUpdate.UpdatedDate);
            }
        }
        
        private static CancelHearingsInGroupRequest BuildRequest() =>
            new()
            {
                UpdatedBy = "updatedBy@email.com",
                CancelReason = "Cancellation reason"
            };
        
        private async Task<List<VideoHearing>> SeedHearingsInGroup()
        {
            var dates = new List<DateTime>
            {
                DateTime.Today.AddDays(5).AddHours(10),
                DateTime.Today.AddDays(6).AddHours(10),
                DateTime.Today.AddDays(7).AddHours(10)
            };

            var multiDayHearings = await Hooks.SeedMultiDayHearing(useV2: false, dates, addPanelMember: true);
            
            await using var db = new BookingsDbContext(BookingsDbContextOptions);
            
            var hearings = new List<VideoHearing>();
            
            // Allocate justice users
            var user = await Hooks.SeedJusticeUser($"{Guid.NewGuid():N}@test.com", "testfirstname1", "testsurname1");
            foreach (var hearing in multiDayHearings)
            {
                await Hooks.AddAllocation(hearing, user);
                
                var hearingFromDb = await new GetHearingByIdQueryHandler(db).Handle(new GetHearingByIdQuery(hearing.Id));
                hearings.Add(hearingFromDb);
            }

            return hearings;
        }
    }
}
