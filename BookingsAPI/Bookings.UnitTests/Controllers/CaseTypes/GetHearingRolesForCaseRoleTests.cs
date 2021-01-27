using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Bookings.Api.Contract.Responses;
using Bookings.DAL.Queries;
using Bookings.Domain.RefData;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace Bookings.UnitTests.Controllers.CaseTypes
{
    public class GetHearingRolesForCaseRoleTests : CaseTypesControllerTests
    {
        [Test]
        public async Task Should_succesfully_return_HearingRoleResponses()
        {
            var caseTypeName = "test";
            var caseRoleName = "TestRole";
            var caseRole = new CaseRole(1, "TestRole")
            {
                HearingRoles = new List<HearingRole>
                {
                    new HearingRole(1,"HearingRoleTest") {UserRole = new UserRole(1, "judicial office holder")}
                }
            };
            
            var caseType = new CaseType(1, "Civil")
            {
                CaseRoles = new List<CaseRole> { caseRole }
            };
            
            QueryHandler.Setup(q => q.Handle<GetCaseTypeQuery, CaseType>(It.IsAny<GetCaseTypeQuery>())).ReturnsAsync(caseType);

            var result = await Controller.GetHearingRolesForCaseRole(caseTypeName, caseRoleName);

            result.Should().NotBeNull();
            var objectResult = (ObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var response = (List<HearingRoleResponse>)objectResult.Value;
            response.Count.Should().Be(1);
            response[0].Name.Should().Be("HearingRoleTest");
            response[0].UserRole.Should().Be("judicial office holder");
        }

        [Test]
        public async Task Should_succesfully_return_notfound_without_matching_casetype()
        {
            var caseTypeName = "test";
            var caseRoleName = "TestRole";
            QueryHandler.Setup(q => q.Handle<GetCaseTypeQuery, CaseType>(It.IsAny<GetCaseTypeQuery>())).ReturnsAsync((CaseType)null);

            var result = await Controller.GetHearingRolesForCaseRole(caseTypeName, caseRoleName);

            result.Should().NotBeNull();
            var objectResult = (NotFoundResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }

        [Test]
        public async Task Should_succesfully_return_notfound_without_matching_caserole()
        {
            var caseTypeName = "test";
            var caseRoleName = "TestRole";
            var caseType = new CaseType(1, "Civil") { CaseRoles = new List<CaseRole>() };
            QueryHandler.Setup(q => q.Handle<GetCaseTypeQuery, CaseType>(It.IsAny<GetCaseTypeQuery>())).ReturnsAsync(caseType);

            var result = await Controller.GetHearingRolesForCaseRole(caseTypeName, caseRoleName);

            result.Should().NotBeNull();
            var objectResult = (NotFoundResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }
    }
}
