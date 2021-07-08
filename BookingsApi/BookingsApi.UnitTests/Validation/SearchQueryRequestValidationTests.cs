using BookingsApi.Contract.Requests;
using BookingsApi.Validations;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingsApi.UnitTests.Validation
{
    public class SearchQueryRequestValidationTests
    {
        private SearchQueryRequestValidation _validator;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _validator = new SearchQueryRequestValidation();
        }

        [Test]
        public async Task Valid_Request_Passes_Validation()
        {
            var request = new SearchQueryRequest 
                            { 
                                Term = "search_term", 
                                JudiciaryUsernamesFromAd = new List<string> 
                                { 
                                    "user1@judiciary.net", 
                                    "user2@judiciary.net" 
                                } 
                            };

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeTrue();
        }

        [Test]
        public async Task Invalid_Term_Fails_Validation()
        {
            var request = new SearchQueryRequest
            {
                Term = null,
                JudiciaryUsernamesFromAd = new List<string>
                                {
                                    "user1@judiciary.net",
                                    "user2@judiciary.net"
                                }
            };

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == SearchQueryRequestValidation.TermErrorMessage)
                .Should().BeTrue();
        }

        [Test]
        public async Task Corrupted_List_Of_Ad_Users_Fails_Validation()
        {
            var request = new SearchQueryRequest
            {
                Term = "search_term",
                JudiciaryUsernamesFromAd = null
            };

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == SearchQueryRequestValidation.JudiciaryUsernamesFromAdErrorMessage)
                .Should().BeTrue();
        }
    }
}
