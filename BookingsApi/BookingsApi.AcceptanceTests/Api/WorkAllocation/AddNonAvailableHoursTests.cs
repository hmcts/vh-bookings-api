using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.Contract.Requests;
using BookingsApi.Contract.Responses;
using FluentAssertions;
using NUnit.Framework;

namespace BookingsApi.AcceptanceTests.Api.WorkAllocation;

public class AddNonAvailableHoursTests : ApiTest
{
    private JusticeUserResponse _cso;
    private long? _newNonWorkingHoursId;
    
    [SetUp]
    public async Task Setup()
    {
        _cso = await GetJusticeUserForTest();
        TestContext.WriteLine($"Using justice user {_cso.Id} - {_cso.Username}");
        _cso.Should().NotBeNull("We need a test justice user to modify for testing");
    }

    [TearDown]
    public async Task TearDown()
    {
        if (_newNonWorkingHoursId.HasValue)
        {
            await BookingsApiClient.DeleteVhoNonAvailabilityHoursAsync(_newNonWorkingHoursId.Value);
            TestContext.WriteLine($"Removed non working hours (id: {_newNonWorkingHoursId}) for user {_cso.Username}");
            _newNonWorkingHoursId = null;
            
        }
    }

    [Test]
    public async Task AddNonavailableHoursForUser()
    {
        // arrange
        var request = new UpdateNonWorkingHoursRequest()
        {
            Hours = new List<NonWorkingHours>()
            {
                new NonWorkingHours()
                {
                    StartTime = new DateTime(2099, 7, 1, 9, 0, 0, DateTimeKind.Utc),
                    EndTime = new DateTime(2099, 7, 1, 10, 0, 0, DateTimeKind.Utc)
                }
            }
        };

        // act
        await BookingsApiClient.UpdateVhoNonAvailabilityHoursAsync(_cso.Username, request);

        // assert
        var response = await BookingsApiClient.GetVhoNonAvailabilityHoursAsync(_cso.Username);
        var newNonWorkHours = response.FirstOrDefault(x =>
            x.StartTime == request.Hours.First().StartTime && x.EndTime == request.Hours.First().EndTime);
        newNonWorkHours.Should().NotBeNull();
        _newNonWorkingHoursId = newNonWorkHours!.Id;
        Assert.Pass();
    }

    private async Task<JusticeUserResponse> GetJusticeUserForTest()
    {
        // get all justice users, including deleted ones
        var users = await BookingsApiClient.GetJusticeUserListAsync("automation", true);
            
        // get the first user that contains the word test or Auto else create one
        var cso = users.FirstOrDefault(x =>
                      x.FirstName.Contains("automation", StringComparison.CurrentCultureIgnoreCase) ||x.FirstName.Contains("Auto", StringComparison.CurrentCultureIgnoreCase)) ??
                  await CreateJusticeUser();

        // if a user was returned and it was deleted, restore it before you use it for a test
        if (!cso.Deleted) return cso;
        
        TestContext.WriteLine($"Restoring justice user {cso.Id} - {cso.Username}");
        await BookingsApiClient.RestoreJusticeUserAsync(new RestoreJusticeUserRequest()
        {
            Id = cso.Id, Username = cso.Username
        });

        return cso;
    }
}