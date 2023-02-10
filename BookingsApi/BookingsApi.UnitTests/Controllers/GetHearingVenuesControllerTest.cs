using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using BookingsApi.Contract.Responses;
using BookingsApi.Controllers;
using BookingsApi.DAL.Queries;
using BookingsApi.DAL.Queries.Core;
using BookingsApi.Domain;
using BookingsApi.Domain.RefData;
using BookingsApi.Mappings;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace BookingsApi.UnitTests.Controllers;

public class GetHearingVenuesControllerTest
{        
    private readonly HearingVenuesController _controller;
    private readonly Mock<IQueryHandler> _queryHandler;
    public GetHearingVenuesControllerTest()
    {
        
        _queryHandler = new Mock<IQueryHandler>();
        _controller = new HearingVenuesController(_queryHandler.Object);
    }

    [Test]
    public async Task GetHearingVenueNamesByAllocatedCso_should_return_list_of_hearing_venue_names()
    {
        //Arrange
        var venueName = "TestVenueName";
        var justiceUser = new JusticeUser { Username = "TestUser", UserRole = new UserRole(9, "team-lead")};
        var expectedResponse = new List<VenueWithAllocatedCsoResponse>
        {
            new() { Cso = JusticeUserToResponseMapper.Map(justiceUser), HearingVenueName = venueName }
        };
        var mockHearing = new Mock<VideoHearing>();
        mockHearing.SetupGet(e => e.HearingVenueName).Returns(venueName);
        mockHearing.Setup(e => e.AllocatedTo).Returns(justiceUser);
        _queryHandler
            .Setup(x => x.Handle<GetAllocationHearingsBySearchQuery, List<VideoHearing>>(It.IsAny<GetAllocationHearingsBySearchQuery>()))
            .ReturnsAsync(new List<VideoHearing>{ mockHearing.Object });

        //Act
        var response = await _controller.GetHearingVenueNamesByAllocatedCso(It.IsAny<Guid[]>()) as OkObjectResult;
        
        //Assert
        response.Should().NotBeNull().And.BeOfType<OkObjectResult>();
        var responseData = response?.Value as IEnumerable<VenueWithAllocatedCsoResponse>;
        responseData.Should().NotBeNull().And.HaveCount(1);
        responseData.Should().BeEquivalentTo(expectedResponse);
    }
    
    [TestCase("query returns empty")]
    [TestCase("query returns null")]
    public async Task GetHearingVenueNamesByAllocatedCso_should_return_empty_list_when_query_return_null_or_empty(string queryReturn)
    {
        //Arrange
        if(queryReturn == "query returns empty")
            _queryHandler
                .Setup(x => x.Handle<GetAllocationHearingsBySearchQuery, List<VideoHearing>>(It.IsAny<GetAllocationHearingsBySearchQuery>()))
                .ReturnsAsync(new List<VideoHearing>());
        else //query returns null
            _queryHandler
                .Setup(x => x.Handle<GetAllocationHearingsBySearchQuery, List<VideoHearing>>(It.IsAny<GetAllocationHearingsBySearchQuery>()))
                .ReturnsAsync(() => null);

        //Act
        var response = await _controller.GetHearingVenueNamesByAllocatedCso(It.IsAny<Guid[]>()) as OkObjectResult;
        
        //Assert
        response.Should().NotBeNull();
        response?.StatusCode.Should().Be(200);
        response?.Value.Should().BeEquivalentTo(Array.Empty<VenueWithAllocatedCsoResponse>());
    }
}
