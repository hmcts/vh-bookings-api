using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using BookingsApi.Contract.V1.Responses;
using BookingsApi.Domain.RefData;
using BookingsApi.DAL.Queries;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace BookingsApi.UnitTests.Controllers.CaseTypes
{
    public class GetCaseRolesForCaseTypeTests : CaseTypesControllerTests
    {
        [Test]
        public async Task Should_succesfully_return_CaseRoleResponses()
        {
            var caseTypeName = "test";
            var casetType = new CaseType(1, "Civil") { CaseRoles = new List<CaseRole> { new CaseRole(1, "TestRole") } };
            QueryHandler.Setup(q => q.Handle<GetCaseTypeQuery, CaseType>(It.IsAny<GetCaseTypeQuery>())).ReturnsAsync(casetType);

            var result = await Controller.GetCaseRolesForCaseType(caseTypeName);

            result.Should().NotBeNull();
            var objectResult = (ObjectResult)result;  
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var response = (List<CaseRoleResponse>)objectResult.Value;
            response.Count.Should().Be(1);
            response[0].Name.Should().Be("TestRole");
        }   

        [Test]
        public async Task Should_succesfully_return_notfound_without_matching_casetype()
        {
            var caseTypeName = "test";
            QueryHandler.Setup(q => q.Handle<GetCaseTypeQuery, CaseType>(It.IsAny<GetCaseTypeQuery>())).ReturnsAsync((CaseType)null);

            var result = await Controller.GetCaseRolesForCaseType(caseTypeName);

            result.Should().NotBeNull();
            var objectResult = (NotFoundResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }
    }
}
