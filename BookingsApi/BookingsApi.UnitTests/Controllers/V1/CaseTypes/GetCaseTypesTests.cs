using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BookingsApi.Contract.Responses;
using BookingsApi.DAL.Queries;
using BookingsApi.Domain.RefData;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace BookingsApi.UnitTests.Controllers.V1.CaseTypes
{
    public class GetCaseTypesTests : CaseTypesControllerTests
    {
        [Test]
        public async Task Should_not_return_any_casetyperesponse_wihtout_matching_casetypes()
        {
            var caseTypes = new List<CaseType>();
            QueryHandler.Setup(q => q.Handle<GetAllCaseTypesQuery, List<CaseType>>(It.IsAny<GetAllCaseTypesQuery>())).ReturnsAsync(caseTypes);

            var result = await Controller.GetCaseTypes();

            result.Should().NotBeNull();
            var objectResult = (ObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var response = (IEnumerable<CaseTypeResponse>)objectResult.Value;
            response.Should().BeEmpty();
        }

        [Test]
        public async Task Should_succesfully_return_casetyperesponse()
        {
            var hearingTypes = new List<HearingType> { new HearingType("NewHearing"){ Code = "NewHearingCode", WelshName = "NewHearingCodeInWelsh"} };
            var caseType = new CaseType(1, "Civil") { HearingTypes = hearingTypes };
            var caseTypes = new List<CaseType>() { caseType };
            
            QueryHandler.Setup(q => q.Handle<GetAllCaseTypesQuery, List<CaseType>>(It.IsAny<GetAllCaseTypesQuery>())).ReturnsAsync(caseTypes);

            var result = await Controller.GetCaseTypes();

            result.Should().NotBeNull();
            var objectResult = (ObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            List<CaseTypeResponse> response = ((IEnumerable<CaseTypeResponse>)objectResult.Value).ToList();
            response.Count.Should().Be(1);
            response[0].Name.Should().Be("Civil");
            response[0].HearingTypes[0].Name.Should().Be("NewHearing");
            response[0].HearingTypes[0].Code.Should().Be("NewHearingCode");
        }
    }
}
