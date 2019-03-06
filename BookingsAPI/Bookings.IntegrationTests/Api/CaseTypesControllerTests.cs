using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Bookings.Api.Contract.Responses;
using FluentAssertions;
using NUnit.Framework;
using Testing.Common.Builders.Api;

namespace Bookings.IntegrationTests.Api
{
    public class CaseTypesControllerTests : ControllerTestsBase
    {
        private readonly CaseTypesEndpoints _endpoints = new ApiUriFactory().CaseTypesEndpoints;
        
        [Test]
        public async Task should_get_case_roles_for_case_type_ok_status()
        {
            var uri = _endpoints.GetCaseRolesForCaseType("Civil Money Claims");
            var response = await SendGetRequestAsync(uri);
            TestContext.WriteLine($"Status Code: {response.StatusCode}");
            response.IsSuccessStatusCode.Should().BeTrue();
            var json = await response.Content.ReadAsStringAsync();
            var model = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<List<CaseRoleResponse>>(json);

            model.Should().NotBeEmpty();
        }
        
        [Test]
        public async Task should_get_case_roles_for_case_type_not_found_status()
        {
            var uri = _endpoints.GetCaseRolesForCaseType("Not Exist");
            var response = await SendGetRequestAsync(uri);
            TestContext.WriteLine($"Status Code: {response.StatusCode}");
            response.IsSuccessStatusCode.Should().BeFalse();
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
        
        [Test]
        public async Task should_get_hearing_roles_for_case_role_ok_status()
        {
            var uri = _endpoints.GetHearingRolesForCaseRole("Civil Money Claims", "Claimant");
            var response = await SendGetRequestAsync(uri);
            TestContext.WriteLine($"Status Code: {response.StatusCode}");
            response.IsSuccessStatusCode.Should().BeTrue();
            var json = await response.Content.ReadAsStringAsync();
            var model = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<List<CaseRoleResponse>>(json);
            model.Should().NotBeEmpty();
        }
        
        [Test]
        public async Task should_get_hearing_roles_for_case_role_not_found_status()
        {
            var uri = _endpoints.GetHearingRolesForCaseRole("Civil Money Claims", "Not Exist");
            var response = await SendGetRequestAsync(uri);
            TestContext.WriteLine($"Status Code: {response.StatusCode}");
            response.IsSuccessStatusCode.Should().BeFalse();
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Test]
        public async Task should_get_case_and_hearing_types()
        {
            var uri = _endpoints.GetCaseTypes();
            var response = await SendGetRequestAsync(uri);
            TestContext.WriteLine($"Status Code: {response.StatusCode}");
            response.IsSuccessStatusCode.Should().BeTrue();
            var json = await response.Content.ReadAsStringAsync();
            var model = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<List<CaseTypeResponse>>(json);
            model.Count.Should().BePositive();
            model.First().HearingTypes.Count.Should().BePositive();

        }
    }
}