using System.Collections.Generic;
using BookingsApi.Contract.V1.Responses;
using BookingsApi.Controllers.V1;
using BookingsApi.DAL.Queries;
using BookingsApi.DAL.Queries.Core;
using BookingsApi.Domain;
using Microsoft.AspNetCore.Mvc;

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
        var venue1 = new HearingVenue(1, "TestHearingVenueName1");
        var venue2 = new HearingVenue(1, "TestHearingVenueName2");
        var expectedResponse = new[]{ venue1.Name, venue2.Name };
        var queryResponse = new List<VideoHearing>
        {
            Mock.Of<VideoHearing>(),
            Mock.Of<VideoHearing>()
        };
        queryResponse[0].SetProtected(nameof(HearingVenue), venue1);
        queryResponse[1].SetProtected(nameof(HearingVenue), venue2);
        
        _queryHandler
            .Setup(x => x.Handle<GetAllocationHearingsBySearchQuery, List<VideoHearing>>(
                It.IsAny<GetAllocationHearingsBySearchQuery>()))
            .ReturnsAsync(queryResponse);

        //Act
        var response = await _controller.GetHearingVenueNamesByAllocatedCso(It.IsAny<Guid[]>()) as OkObjectResult;
        
        //Assert
        response?.Should().NotBeNull().And.BeOfType<OkObjectResult>();
        var responseData = response?.Value as IList<string>;
        responseData?.Should().NotBeNull().And.BeEquivalentTo(expectedResponse);
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
    
        
    [Test]
    public async Task Get_HearingVenues_with_expired_venues_for_Hearings_Today()
    {
        //Arrange
        var mockVenue0 = new HearingVenue(100000,"MockVenue0") { ExpirationDate = DateTime.Today};
        var mockVenue1 = new HearingVenue(100001,"MockVenue1") { ExpirationDate = DateTime.Today.AddDays(-1)};
        var mockVenue2 = new HearingVenue(100002,"MockVenue2") { ExpirationDate = DateTime.Today.AddDays(-1)};
        var mockVenue3 = new HearingVenue(100003,"MockVenue3") { ExpirationDate = DateTime.Today.AddDays(1)};
        var mockVenue4 = new HearingVenue(100004,"MockVenue4") { ExpirationDate = null};
        var mockHearing = Mock.Of<VideoHearing>();
        mockHearing.SetProtected(nameof(HearingVenue), mockVenue1);

        //Mock of hearings today
        _queryHandler
                .Setup(x => x.Handle<GetHearingsForTodayQuery, List<VideoHearing>>(It.IsAny<GetHearingsForTodayQuery>()))
                .ReturnsAsync(new List<VideoHearing>{ mockHearing });

        //Mock of hearing venues
            _queryHandler
                .Setup(x => x.Handle<GetHearingVenuesQuery, List<HearingVenue>>(It.IsAny<GetHearingVenuesQuery>()))
                .ReturnsAsync(() => new List<HearingVenue>{mockVenue0, mockVenue1, mockVenue2, mockVenue3, mockVenue4});

        //Act
        var response = await _controller.GetHearingVenuesIncludingExpiredVenuesForHearingsToday() as OkObjectResult;
        
        //Assert
        response?.StatusCode.Should().Be(200);
        var hearingVenues = response?.Value as List<HearingVenueResponse>;
        hearingVenues.Should().NotBeNull();
        hearingVenues.Should().NotContain(e => e.Id == mockVenue0.Id);
        hearingVenues.Should().Contain(e => e.Id == mockVenue1.Id);
        hearingVenues.Should().NotContain(e => e.Id == mockVenue2.Id);
        hearingVenues.Should().Contain(e => e.Id == mockVenue3.Id);
        hearingVenues.Should().Contain(e => e.Id == mockVenue4.Id);
    }
}
