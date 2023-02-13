using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookingsApi.Controllers;
using BookingsApi.DAL.Queries;
using BookingsApi.DAL.Queries.Core;
using BookingsApi.Domain;
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
        var expectedResponse = new[]{"TestHearingVenueName1", "TestHearingVenueName2"};
        var queryResponse = new List<VideoHearing>
        {
            Mock.Of<VideoHearing>(),
            Mock.Of<VideoHearing>()
        };
        queryResponse[0].HearingVenueName = expectedResponse[0];
        queryResponse[1].HearingVenueName = expectedResponse[1];
        
        _queryHandler
            .Setup(x => x.Handle<GetAllocationHearingsBySearchQuery, List<VideoHearing>>(It.IsAny<GetAllocationHearingsBySearchQuery>()))
            .ReturnsAsync(queryResponse);

        //Act
        var response = await _controller.GetHearingVenueNamesByAllocatedCso(It.IsAny<Guid[]>()) as OkObjectResult;
        
        //Assert
        response?.Should().NotBeNull().And.BeOfType<OkObjectResult>();
        var responseData = response?.Value as IList<string>;
        responseData?.Should().NotBeNull().And.Should().BeEquivalentTo(expectedResponse);
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
        response?.StatusCode.Should().Be(200);
        response?.Value.Should().NotBeNull().And.BeEquivalentTo(Array.Empty<string>());
    }
}
