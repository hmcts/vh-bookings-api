﻿using System.Collections.Generic;
using BookingsApi.Contract.V1.Responses;
using BookingsApi.Controllers;
using BookingsApi.DAL.Dtos;
using BookingsApi.DAL.Queries;
using Microsoft.AspNetCore.Mvc;

namespace BookingsApi.UnitTests.Controllers.Persons
{
    public class GetAnonymisationDataTests : PersonsControllerTest
    {
        
        [Test]
        public async Task GetAnonymisationData_Returns_AnonymisationDataResponse()
        {
            var anonymisationDataDto = new AnonymisationDataDto
            {
                Usernames = new List<string> {"testUser1@hmcts.net", "testUser2@hmcts.net", "testUser3@hmcts.net"},
                HearingIds = new List<Guid> {Guid.NewGuid(), Guid.NewGuid()}
            };
            QueryHandlerMock
                .Setup(x => x.Handle<GetAnonymisationDataQuery, AnonymisationDataDto>(It.IsAny<GetAnonymisationDataQuery>()))
                .ReturnsAsync(anonymisationDataDto);

            var result = await Controller.GetAnonymisationData();
            result.Should().NotBeNull();
            var objectResult = result as ObjectResult;
            var response = (AnonymisationDataResponse)(objectResult.Value);
            response.Usernames.Count.Should().Be(3);
            response.HearingIds.Count.Should().Be(2);
            QueryHandlerMock
                .Verify(x => x.Handle<GetAnonymisationDataQuery, AnonymisationDataDto>(It.IsAny<GetAnonymisationDataQuery>()), Times.Once);
        }
    }
}